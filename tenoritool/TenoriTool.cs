#region Copyright (C) 2008 Giacomo Stelluti Scala
//
// Copy Directory Tree: Program.cs
//
// Author:
//   Giacomo Stelluti Scala (giacomo.stelluti@gmail.com)
//
// Copyright (C) 2008 Giacomo Stelluti Scala
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

namespace tenoritool;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using CommandLine;
using CommandLine.Text;

static partial class TenoriTool {
    #region Exit Code Constants

    private const int ExitSuccess = 0;
    private const int ExitFailure = 1;
    private const int ExitFailureCritical = 2;

    #endregion

    private static readonly HeadingInfo Heading = new(ThisAssembly.Name, ThisAssembly.MajorMinorVersion);
    private static readonly ConsoleColor HighlightColor = ConsoleColor.Yellow;
    private static readonly UInt32 TenoriSigConst = BinaryIO.ReadUInt32("GPDA"u8.ToArray()); // "GPDA", Guyzware Packed Data Archive
    private static readonly UInt32 TenoriEntrySize = 0x10;


    private static void Main(string[] args) {
        Options options = new();
        ICommandLineParser parser = new CommandLineParser();
        if (!parser.ParseArguments(args, options, Console.Error)) {
            Environment.Exit(ExitFailure);
        }

        if (!options.Validate()) {
            Console.Error.WriteLine("Try '{0} --help' for more information.", ThisAssembly.Name);
            Environment.Exit(ExitFailure);
        }

        if (options.Verbose) {
            Console.ForegroundColor = HighlightColor;
            Console.Error.WriteLine(Heading.ToString());
            Console.ResetColor();
        }

        bool success = true;
        if (!options.UsePackMode) {
            if (options.UseExtractMode)
                options.ExtractStub = ExtractEntry; // use real function
            success = ProcessIterationExtract(options);
        }

        #region Helper Code while running inside an IDE (uncomment when needed)

        //Console.ForegroundColor = ConsoleColor.Green;
        //Console.Write(">>>press any key<<<");
        //Console.ResetColor();
        //Console.ReadKey();

        #endregion

        Environment.Exit(success ? ExitSuccess : ExitFailureCritical);
    }


    private static bool ProcessIterationExtract(Options options) {
        bool hasError = false;
        foreach (string inpath in options.Paths) {
            string displayFilename;

            bool isStdInput = inpath.Equals("-", StringComparison.InvariantCulture);
            if (isStdInput) {
                displayFilename = "<STDIN>";
            }
            else {
                displayFilename = Path.GetFileName(inpath);
            }

            if (options.Verbose) {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Error.WriteLine($"* {displayFilename}");
                Console.ResetColor();
            }

            if (!isStdInput) {
                string ioerror = String.Empty;
                try {
                    FileAttributes attributes = File.GetAttributes(inpath);
                    FileInfo infile = new(inpath);
                    if ((attributes & FileAttributes.Directory) == FileAttributes.Directory || !infile.Exists) {
                        ioerror = "does not exist or is a directory";
                    }
                    else {
                        if (options.Verbose) {
                            Console.Error.WriteLine($"    Reported size: {infile.Length}");
                        }

                        string errorString;
                        if (!ProcessIndividualExtract(options, "", inpath, out errorString)) {
                            ioerror = errorString;
                        }
                    }
                }
                catch (IOException ex) {
                    ioerror = ex.Message;
                }
                catch (UnauthorizedAccessException ex) {
                    ioerror = ex.Message;
                }
                catch (Exception ex) {
                    ReportError($"{inpath}: unexpected exception occured ({ex.Message})");
                    throw;
                }

                if (ioerror.Length <= 0) continue;
                ReportError($"cannot open {inpath}: {ioerror}");
                hasError = true;
            }
        }

        return !hasError;
    }

    private static bool ProcessIndividualExtract(Options options, string baseSubdirectory, string path, out string stringError) {
        stringError = "";
        List<string> filenames = new();

        // Only try catching when reading is not possible (not enough input/space)
        using (BinaryReader reader = new(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), Encoding.Default)) {
            byte[] header = reader.ReadBytes(0x10);
            UInt32 sig = BinaryIO.ReadUInt32(header, 0x00);
            if (sig != TenoriSigConst) {
                stringError = $"magic constant mismatch, found 0x{sig:08X}";
                return false;
            }

            ArchiveInfo arcinfo = new();

            if (options.Use32Mode) {
                arcinfo.ArchiveSize = BinaryIO.ReadUInt32(header, 0x04);
            }
            else {
                arcinfo.ArchiveSize = BinaryIO.ReadUInt64(header, 0x04);
            }

            if (options.Verbose) {
                Console.Error.WriteLine(String.Format(new FileSizeFormatProvider(), "    Declared size: {0} ({0:fs})",
                    arcinfo.ArchiveSize));
            }

            arcinfo.EntriesCount = BinaryIO.ReadUInt32(header, 0x0C);
            if (options.Verbose) {
                Console.Error.WriteLine(String.Format(new FileSizeFormatProvider(), "    Entries: {0}", arcinfo.EntriesCount));
            }

            if (arcinfo.EntriesCount <= 0) return true;

            byte[] entriesinfobytes = reader.ReadBytes(Convert.ToInt32(TenoriEntrySize * arcinfo.EntriesCount));

            UInt32 headerPartialSize = 0x10 + TenoriEntrySize * arcinfo.EntriesCount;

            // -- Detect generic cabinets
            // They are archives with several entries of the same name
            // In this case, generate a name of the form orgfile.####.ext.

            Dictionary<string, int> usedfilenames = new();
            List<ArchiveEntryInfo> entriesinfo = new();

            // TODO: should normally check if input is seekable beforehand
            // When reading from a pipe, this wouldn't be possible

            for (int i = 0; i < arcinfo.EntriesCount; ++i) {
                ArchiveEntryInfo einfo = new();

                if (options.Use32Mode) {
                    einfo.EntryOffset = BinaryIO.ReadUInt32(entriesinfobytes, Convert.ToInt32(i * TenoriEntrySize + 0x00));
                }
                else {
                    einfo.EntryOffset = BinaryIO.ReadUInt64(entriesinfobytes, Convert.ToInt32(i * TenoriEntrySize + 0x00));
                }

                einfo.EntrySize = BinaryIO.ReadUInt32(entriesinfobytes, Convert.ToInt32(i * TenoriEntrySize + 0x08));
                einfo.EntryNameOffset = BinaryIO.ReadUInt32(entriesinfobytes, Convert.ToInt32(i * TenoriEntrySize + 0x0C));
                reader.BaseStream.Seek(einfo.EntryNameOffset, SeekOrigin.Begin);

                UInt32 len = reader.ReadUInt32();
                byte[] strbytes = reader.ReadBytes(Convert.ToInt32(len));
                einfo.EntryName = Encoding.GetEncoding(932).GetString(strbytes);
                if (usedfilenames.ContainsKey(einfo.EntryName)) {
                    usedfilenames[einfo.EntryName]++;
                }
                else {
                    usedfilenames.Add(einfo.EntryName, 1);
                }

                entriesinfo.Add(einfo);
            }

            if (options.Verbose) {
                Console.Error.WriteLine(String.Format(new FileSizeFormatProvider(), "    Is padded with space: {0}",
                    entriesinfo[0].EntryName.EndsWith(" ") ? "Yes" : "No"));
            }

            bool genericEntries = false;
            string genericNameFormat = String.Empty;
            for (int i = 0; i < arcinfo.EntriesCount; ++i) {
                if (usedfilenames[entriesinfo[i].EntryName] > 1) {
                    genericEntries = true;
                    break;
                }
            }

            if (genericEntries) {
                genericNameFormat = "{0}.{1:d4}.{2}";
                if (options.Verbose) {
                    Console.Error.WriteLine($"    Format Mask: {genericNameFormat}");
                }
            }


            for (int i = 0; i < arcinfo.EntriesCount; ++i) {
                if (usedfilenames[entriesinfo[i].EntryName] > 1) {
                    string fileName = Path.GetFileNameWithoutExtension(entriesinfo[i].EntryName);
                    string fileExtension = Path.GetExtension(entriesinfo[i].EntryName).TrimStart('.');
                    entriesinfo[i].EntryName = String.Format(genericNameFormat, fileName, i + 1, fileExtension);
                }

                arcinfo.Entries.Add(entriesinfo[i]);
                if (options.Verbose) {
                    Console.Error.WriteLine($" {i + 1,4} # {entriesinfo[i]}");
                }

                filenames.Add(entriesinfo[i].EntryName);
                options.ExtractStub(options, baseSubdirectory, reader.BaseStream, entriesinfo[i]);
            }

            TextWriter writer;

            if (options.ListPath != "-") {
                if (options.ListPath.Length == 0) {
                    writer = new StreamWriter(new MemoryStream()); // silently discard
                }
                else {
                    writer = new StreamWriter(File.Create(options.ListPath), Encoding.UTF8);
                }
            }
            else {
                writer = Console.Out; // .Dispose() seems ok ?
            }

            using (writer) {
                // Get current output (normally, stdout)
                TextWriter originalWriter = Console.Out;
                try {
                    Console.SetOut(writer);
                    writer.Write(String.Join(Environment.NewLine, filenames.ToArray()));
                }
                finally {
                    Console.SetOut(originalWriter);
                }
            }

        }

        return true;
    }

    static bool DummyExtractEntry(Options options, string baseSubdirectory, Stream inputStream, ArchiveEntryInfo entryinfo) {
        return true;
    }

    static bool ExtractEntry(Options options, string baseSubdirectory, Stream inputStream, ArchiveEntryInfo entryinfo) {
        string target = Path.Combine(options.OutputDirectory, baseSubdirectory);
        if (!Directory.Exists(target)) {
            Directory.CreateDirectory(target);
        }

        target = Path.Combine(options.OutputDirectory, entryinfo.EntryName);

        if (options.Verbose) {
            Console.ForegroundColor = HighlightColor;
            Console.Error.WriteLine("    ^ {0}", target);
            Console.ResetColor();
        }

        inputStream.Seek(Convert.ToInt64(entryinfo.EntryOffset), SeekOrigin.Begin);
        Int32 remainingBytes = Convert.ToInt32(entryinfo.EntrySize);
        const Int32 bufferSize = 4096;
        byte[] readBuffer = new byte[4096];

        using (FileStream destinationStream = new(target, FileMode.Create, FileAccess.Write)) {
            while (remainingBytes > 0) {
                int readSize = remainingBytes < bufferSize ? remainingBytes : bufferSize;
                inputStream.Read(readBuffer, 0, readSize);
                destinationStream.Write(readBuffer, 0, readSize);
                remainingBytes -= readSize;
            }
        }

        return true;
    }
}