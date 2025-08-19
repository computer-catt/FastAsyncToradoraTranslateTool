using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace tenoriTool;

public static class TenoriToolApi {

    public class ProcessReturn {
        public string Error = "";
        public bool Success = true;
    }
    
    public class EventTextWriter : TextWriter
    {
        public event Action<string> TextWritten;

        public override Encoding Encoding => Encoding.UTF8;
        public override void WriteLine(string value) => TextWritten?.Invoke(value + Environment.NewLine);
    }
    
    private const uint TenoriEntrySize = 0x10;
    private static readonly uint TenoriSigConst = BinaryIO.ReadUInt32("GPDA"u8.ToArray()); // "GPDA", Guyzware Packed Data Archive
    
    public delegate Task<bool> ExtractDelegate(string outputDirectory, bool verbose, ConsoleColor highlightColor, string baseSubdirectory, Stream inputStream, ArchiveEntryInfo entryinfo, TextWriter output = null);

    public static async Task<ProcessReturn> ProcessIndividualExtract(string listPath, string outputDirectory, bool use32Mode, bool verbose, ConsoleColor highlightColor, string baseSubdirectory, string path, ExtractDelegate extract = null, TextWriter output = null ) {
        extract ??= ExtractEntry;
        output ??= Console.Error;
        
        ProcessReturn processReturn = new(); 
        List<string> filenames = [];

        // Only try catching when reading is not possible (not enough input/space)
        using BinaryReader reader = new(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), Encoding.Default);
        
        byte[] header = reader.ReadBytes(0x10);
        uint sig = BinaryIO.ReadUInt32(header, 0x00);
        if (sig != TenoriSigConst) {
            processReturn.Error = $"magic constant mismatch, found 0x{sig:08X}";
            processReturn.Success = false;
            return processReturn;
        }

        ArchiveInfo arcinfo = new() {
            ArchiveSize = use32Mode ? BinaryIO.ReadUInt32(header, 0x04) : BinaryIO.ReadUInt64(header, 0x04)
        };

        if (verbose)
            _ = output.WriteLineAsync(string.Format(new FileSizeFormatProvider(), "    Declared size: {0} ({0:fs})", arcinfo.ArchiveSize));

        arcinfo.EntriesCount = BinaryIO.ReadUInt32(header, 0x0C);
        if (verbose)
            _ = output.WriteLineAsync(string.Format(new FileSizeFormatProvider(), "    Entries: {0}", arcinfo.EntriesCount));

        if (arcinfo.EntriesCount <= 0) return processReturn;

        byte[] entriesinfobytes = reader.ReadBytes(Convert.ToInt32(TenoriEntrySize * arcinfo.EntriesCount));

        //UInt32 headerPartialSize = 0x10 + TenoriEntrySize * arcinfo.EntriesCount;

        // -- Detect generic cabinets
        // They are archives with several entries of the same name
        // In this case, generate a name of the form orgfile.####.ext.

        Dictionary<string, int> usedfilenames = new();
        List<ArchiveEntryInfo> entriesinfo = new();

        // TODO: should normally check if input is seekable beforehand
        // When reading from a pipe, this wouldn't be possible

        for (int i = 0; i < arcinfo.EntriesCount; ++i) {
            ArchiveEntryInfo einfo = new() {
                EntryOffset = use32Mode ? 
                    BinaryIO.ReadUInt32(entriesinfobytes, Convert.ToInt32(i * TenoriEntrySize + 0x00)) : 
                    BinaryIO.ReadUInt64(entriesinfobytes, Convert.ToInt32(i * TenoriEntrySize + 0x00)),
                EntrySize = BinaryIO.ReadUInt32(entriesinfobytes, Convert.ToInt32(i * TenoriEntrySize + 0x08)),
                EntryNameOffset = BinaryIO.ReadUInt32(entriesinfobytes, Convert.ToInt32(i * TenoriEntrySize + 0x0C))
            };

            reader.BaseStream.Seek(einfo.EntryNameOffset, SeekOrigin.Begin);

            uint len = reader.ReadUInt32();
            byte[] strbytes = reader.ReadBytes(Convert.ToInt32(len));
            einfo.EntryName = Encoding.GetEncoding(932).GetString(strbytes);
            if (!usedfilenames.TryAdd(einfo.EntryName, 1))
                usedfilenames[einfo.EntryName]++;

            entriesinfo.Add(einfo);
        }

        if (verbose)
            _ = output.WriteLineAsync(string.Format(new FileSizeFormatProvider(), "    Is padded with space: {0}", entriesinfo[0].EntryName.EndsWith(" ") ? "Yes" : "No"));

        bool genericEntries = false;
        string genericNameFormat = string.Empty;
        for (int i = 0; i < arcinfo.EntriesCount; ++i) {
            if (usedfilenames[entriesinfo[i].EntryName] <= 1) continue;
            genericEntries = true;
            break;
        }

        if (genericEntries) {
            genericNameFormat = "{0}.{1:d4}.{2}";
            if (verbose) 
                _ = output.WriteLineAsync($"    Format Mask: {genericNameFormat}");
        }


        for (int i = 0; i < arcinfo.EntriesCount; ++i) {
            if (usedfilenames[entriesinfo[i].EntryName] > 1) {
                string fileName = Path.GetFileNameWithoutExtension(entriesinfo[i].EntryName);
                string fileExtension = Path.GetExtension(entriesinfo[i].EntryName).TrimStart('.');
                entriesinfo[i].EntryName = string.Format(genericNameFormat, fileName, i + 1, fileExtension);
            }

            arcinfo.Entries.Add(entriesinfo[i]);
            if (verbose)
                _ = output.WriteLineAsync($" {i + 1,4} # {entriesinfo[i]}");

            filenames.Add(entriesinfo[i].EntryName);
            await extract(outputDirectory, verbose, highlightColor, baseSubdirectory, reader.BaseStream, entriesinfo[i], output);
        }
        
        return processReturn;
        TextWriter writer;

        if (listPath != "-")
            writer = listPath.Length == 0 ? new StreamWriter(new MemoryStream()) : // silently discard
                new StreamWriter(File.Create(listPath), Encoding.UTF8);
        else 
            writer = Console.Out; // .Dispose() seems ok ?

        await using (writer) {
            // Get current output (normally, stdout)
            TextWriter originalWriter = Console.Out;
            try {
                Console.SetOut(writer);
                await writer.WriteAsync(string.Join(Environment.NewLine, filenames.ToArray()));
            }
            finally {
                Console.SetOut(originalWriter);
            }
        }

        return processReturn;
    }

    public static async Task<bool> ExtractEntry(string outputDirectory, bool verbose, ConsoleColor highlightColor, string baseSubdirectory, Stream inputStream, ArchiveEntryInfo entryinfo, TextWriter output = null) {
        output ??= Console.Error;
        
        string target = Path.Combine(outputDirectory, baseSubdirectory);
        if (!Directory.Exists(target)) {
            Directory.CreateDirectory(target);
        }

        target = Path.Combine(outputDirectory, entryinfo.EntryName);

        if (verbose) {
            Console.ForegroundColor = highlightColor;
            _ = output.WriteLineAsync($"    ^ {target}");
            Console.ResetColor();
        }

        inputStream.Seek(Convert.ToInt64(entryinfo.EntryOffset), SeekOrigin.Begin);
        int remainingBytes = Convert.ToInt32(entryinfo.EntrySize);
        const int bufferSize = 4096;
        byte[] readBuffer = new byte[4096];

        await using FileStream destinationStream = new(target, FileMode.Create, FileAccess.Write);
        
        while (remainingBytes > 0) {
            int readSize = remainingBytes < bufferSize ? remainingBytes : bufferSize;
            await inputStream.ReadAsync(readBuffer, 0, readSize);
            await destinationStream.WriteAsync(readBuffer, 0, readSize);
            remainingBytes -= readSize;
        }

        return true;
    }
}