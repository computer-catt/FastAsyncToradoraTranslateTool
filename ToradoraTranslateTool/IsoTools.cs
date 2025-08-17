using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SevenZip;

namespace ToradoraTranslateTool
{
    class IsoTools
    {
        public static void ExtractIso(string isoPath, Action<byte> progressCallback = null)
        {
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Data", "Iso")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Data", "Iso"));
            
            SevenZipBase.SetLibraryPath(Path.Combine(Application.StartupPath/*, "Bin"*/, RuntimeInformation.OSArchitecture == Architecture.X64 ? "7z X64.dll" : "7z X86.dll"));
            SevenZipExtractor mySze = new(isoPath);
            if (progressCallback != null)
                mySze.Extracting += (_, args) => { progressCallback.Invoke(args.PercentDone); };
            
            mySze.ExtractArchive(Path.Combine(Application.StartupPath, "Data", "Iso"));
            mySze.Dispose();
        }

        public static void RepackIso(string isoDirectory)
        {
            string isoPath = Path.Combine(Application.StartupPath, "Toradora.iso");
            string command = "-iso-level 4 -xa -A \"PSP GAME\" -V \"Toradora\" -sysid \"PSP GAME\" -volset \"Toradora\" -p \"\" -publisher \"\" -o \"" + isoPath + "\" \"" + isoDirectory + "\"";
            Process myProc = new();
            myProc.StartInfo.FileName = Path.Combine(Application.StartupPath, "Data", "Mkisofs", "mkisofs.exe");
            myProc.StartInfo.Arguments = command;
            myProc.StartInfo.WorkingDirectory = Path.Combine(Application.StartupPath, "Data", "Mkisofs");
            myProc.Start();

            myProc.WaitForExit();
        }
    }
}