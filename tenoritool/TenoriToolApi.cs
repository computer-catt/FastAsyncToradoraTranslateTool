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
        public readonly List<string> FileNames = [];
        public bool SpacePadded;
        public string MakeGpdaFileContent;
    }
    
    public class EventTextWriter : TextWriter
    {
        public event Action<string> TextWritten;

        public override Encoding Encoding => Encoding.UTF8;
        public override void WriteLine(string value) => TextWritten?.Invoke(value + Environment.NewLine);
    }
    
    public class TenoriCallbacks {
        public Action<ulong> ArchiveSize;
        public Action<uint> Entries;
        public Action<bool> PaddedWithSpace;
        public Action<string> FormatMask;
        public Action<(int index, ArchiveEntryInfo entry)> EntriesInfo;
        public Action<string> ProcessingFilepathCallback;
        public Action<string> ProcessedFilepathCallback;

        public static TenoriCallbacks None() {
            return new TenoriCallbacks {
                ArchiveSize = _ => { },
                Entries = _ => { },
                EntriesInfo = _ => { },
                FormatMask = _ => { },
                PaddedWithSpace = _ => { },
                ProcessedFilepathCallback = _ => { },
                ProcessingFilepathCallback = _ => { }
            };
        }

        public static TenoriCallbacks Default(TextWriter output = null, ConsoleColor highlightColor = ConsoleColor.Yellow) {
            output ??= Console.Error;
            return new TenoriCallbacks{
                ArchiveSize = size => _ = output.WriteLineAsync(string.Format(new FileSizeFormatProvider(), "    Declared size: {0} ({0:fs})", size)),
                Entries = entries => _ = output.WriteLineAsync(string.Format(new FileSizeFormatProvider(), "    Entries: {0}", entries)),
                PaddedWithSpace = padded => _ = output.WriteLineAsync(string.Format(new FileSizeFormatProvider(), "    Is padded with space: {0}", padded ? "Yes" : "No")),
                FormatMask = formatMask => _ = output.WriteLineAsync($"    Format Mask: {formatMask}"),
                EntriesInfo = args => _ = output.WriteLineAsync($" {args.index + 1,4} # {args.entry}"),
                ProcessingFilepathCallback = path => {
                    Console.ForegroundColor = highlightColor;
                    _ = output.WriteLineAsync($"    ^ {path}");
                    Console.ResetColor();
                },
                ProcessedFilepathCallback = _ => {}
            };
        }
    }
    
    private const uint TenoriEntrySize = 0x10;
    private static readonly uint TenoriSigConst = BinaryIO.ReadUInt32("GPDA"u8.ToArray()); // "GPDA", Guyzware Packed Data Archive
    
    public delegate Task<string> ExtractDelegate(string outputDirectory, bool verbose, string baseSubdirectory, Stream inputStream, ArchiveEntryInfo entryinfo, Action<string> processingFilepath, Action<string> processedFilepath);

    public static async Task<ProcessReturn> ProcessIndividualExtract(string listPath, string outputDirectory, bool use32Mode, bool verbose, string baseSubdirectory, string path, TenoriCallbacks output = null, ExtractDelegate extract = null) {
        extract ??= ExtractEntry;
        output ??= TenoriCallbacks.None();
        
        ProcessReturn processReturn = new(); 
        
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
            ArchiveSize = use32Mode ? BinaryIO.ReadUInt32(header, 0x04) : BinaryIO.ReadUInt64(header, 0x04),
            EntriesCount = BinaryIO.ReadUInt32(header, 0x0C)
        };

        if (verbose) output.ArchiveSize(arcinfo.ArchiveSize);
        if (verbose) output.Entries(arcinfo.EntriesCount);

        if (arcinfo.EntriesCount <= 0) {
            processReturn.Error = $"No entries in Gpda";
            processReturn.Success = false;
            return processReturn;
        }

        byte[] entriesInfoBytes = reader.ReadBytes(Convert.ToInt32(TenoriEntrySize * arcinfo.EntriesCount));

        //UInt32 headerPartialSize = 0x10 + TenoriEntrySize * arcinfo.EntriesCount;

        // -- Detect generic cabinets
        // They are archives with several entries of the same name
        // In this case, generate a name of the form orgfile.####.ext.

        Dictionary<string, int> usedFileNames = new();
        List<ArchiveEntryInfo> entriesInfo = [];

        // TODO: should normally check if input is seekable beforehand
        // When reading from a pipe, this wouldn't be possible

        for (int i = 0; i < arcinfo.EntriesCount; ++i) {
            ArchiveEntryInfo entryinfo = new() {
                EntryOffset = use32Mode ? 
                    BinaryIO.ReadUInt32(entriesInfoBytes, Convert.ToInt32(i * TenoriEntrySize + 0x00)) : 
                    BinaryIO.ReadUInt64(entriesInfoBytes, Convert.ToInt32(i * TenoriEntrySize + 0x00)),
                EntrySize = BinaryIO.ReadUInt32(entriesInfoBytes, Convert.ToInt32(i * TenoriEntrySize + 0x08)),
                EntryNameOffset = BinaryIO.ReadUInt32(entriesInfoBytes, Convert.ToInt32(i * TenoriEntrySize + 0x0C))
            };

            reader.BaseStream.Seek(entryinfo.EntryNameOffset, SeekOrigin.Begin);

            uint len = reader.ReadUInt32();
            byte[] stringBytes = reader.ReadBytes(Convert.ToInt32(len));
            entryinfo.EntryName = Encoding.GetEncoding(932).GetString(stringBytes);
            if (!usedFileNames.TryAdd(entryinfo.EntryName, 1))
                usedFileNames[entryinfo.EntryName]++;

            entriesInfo.Add(entryinfo);
        }

        bool spacePadded = processReturn.SpacePadded = entriesInfo[0].EntryName.EndsWith(' ');
        if (verbose) output.PaddedWithSpace(spacePadded);
        StringBuilder makeGpdaFileContent = new();
        makeGpdaFileContent.Append(spacePadded ? "Y\n" : "N\n");
        
        
        bool genericEntries = false;
        string genericNameFormat = string.Empty;
        for (int i = 0; i < arcinfo.EntriesCount; ++i) {
            if (usedFileNames[entriesInfo[i].EntryName] <= 1) continue;
            genericEntries = true;
            break;
        }

        if (genericEntries) {
            genericNameFormat = "{0}.{1:d4}.{2}";
            if (verbose) output.FormatMask(genericNameFormat);
        }


        for (int i = 0; i < arcinfo.EntriesCount; ++i) {
            string originalName = entriesInfo[i].EntryName;
            if (usedFileNames[entriesInfo[i].EntryName] > 1) {
                string fileName = Path.GetFileNameWithoutExtension(entriesInfo[i].EntryName);
                string fileExtension = Path.GetExtension(entriesInfo[i].EntryName).TrimStart('.');
                entriesInfo[i].EntryName = string.Format(genericNameFormat, fileName, i + 1, fileExtension);
            }

            arcinfo.Entries.Add(entriesInfo[i]);
            if (verbose)
                output.EntriesInfo((i, entriesInfo[i]));
            
            processReturn.FileNames.Add(entriesInfo[i].EntryName);
            string extractedPath = await extract(outputDirectory, verbose, baseSubdirectory, reader.BaseStream, entriesInfo[i], output.ProcessingFilepathCallback, output.ProcessedFilepathCallback);
            makeGpdaFileContent.Append($"{extractedPath.Trim(' ')}\t{originalName}\n");
        }

        processReturn.MakeGpdaFileContent = makeGpdaFileContent.ToString();
        return processReturn;
        /*TextWriter writer;

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

        return processReturn;*/
    }

    /// <returns>The path of the extracted file, or null</returns>
    public static async Task<string> ExtractEntry(string outputDirectory, bool verbose, string baseSubdirectory, Stream inputStream, ArchiveEntryInfo entryinfo, Action<string> processingFilepath, Action<string> processedFilepath) {
        string target = Path.Combine(outputDirectory, baseSubdirectory);
        if (!Directory.Exists(target)) Directory.CreateDirectory(target);

        target = Path.Combine(outputDirectory, entryinfo.EntryName);

        if (verbose) processingFilepath(target);

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

        if (verbose) processedFilepath(target);
        
        return target;
    }
}