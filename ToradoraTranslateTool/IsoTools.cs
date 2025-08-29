using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using SevenZip;

namespace ToradoraTranslateTool;

class IsoTools {
    private static void SetUpSz() {
        SevenZipBase.SetLibraryPath(Path.Combine(Application.StartupPath /*, "Bin"*/, RuntimeInformation.OSArchitecture == Architecture.X64 ? "7z X64.dll" : "7z X86.dll"));
    }

    public static void ExtractIso(string isoPath, Action<byte> progressCallback = null) {
        if (!Directory.Exists(Path.Combine(Application.StartupPath, "Data", "Iso")))
            Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Data", "Iso"));

        SetUpSz();
        SevenZipExtractor mySze = new(isoPath);
        if (progressCallback != null)
            mySze.Extracting += (_, args) => { progressCallback(args.PercentDone); };

        mySze.ExtractArchive(Path.Combine(Application.StartupPath, "Data", "Iso"));
        mySze.Dispose();
    }

    public static void RepackIso(string isoDirectory, string outputPath, Action<float> progressCallback = null) {
        string command = "-iso-level 4 -xa -A \"PSP GAME\" -V \"Toradora\" -sysid \"PSP GAME\" -volset \"Toradora\" -p \"\" -publisher \"\" -o \"" + outputPath + "\" \"" + isoDirectory + "\"";
        using Process myProc = new();
        myProc.StartInfo.FileName = Path.Combine(Application.StartupPath, "Resources", "!!Tools", "Mkisofs", "mkisofs.exe");
        myProc.StartInfo.Arguments = command;
        myProc.StartInfo.CreateNoWindow = true;
        myProc.StartInfo.WorkingDirectory = Path.Combine(Application.StartupPath, "Resources", "!!Tools", "Mkisofs");
        myProc.StartInfo.RedirectStandardError = progressCallback != null;
        myProc.StartInfo.UseShellExecute = false;

        if (progressCallback != null)
            myProc.ErrorDataReceived += (_, args) => {
                /*Console.WriteLine(args.Data);*/
                if (args.Data == null) return;
                if (float.TryParse(args.Data.Trim().Split(' ')[0].Trim('%'), out float result))
                    progressCallback(Math.Clamp(100, 0, result));
            };

        myProc.Start();
        if (progressCallback != null) myProc.BeginErrorReadLine();
        myProc.WaitForExit();
    }

    public static async Task RepackZip(string isoDirectory, string outputPath, Action<float> progressCallback = null) {
        SetUpSz();
        SevenZipCompressor compressor = new() {
            ArchiveFormat = OutArchiveFormat.Zip,
            CompressionMethod = CompressionMethod.Copy,
            CompressionLevel = CompressionLevel.Fast,
        };
        compressor.CustomParameters.Add("mt", "on");
        if (progressCallback != null) compressor.Compressing += (_, args) => { progressCallback(args.PercentDone); };
        await compressor.CompressDirectoryAsync(isoDirectory, outputPath);
    }
}