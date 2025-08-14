﻿using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using SevenZipExtractor;

namespace ToradoraTranslateTool
{
    class IsoTools
    {
        public static void ExtractIso(string isoPath)
        {
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Data", "Iso")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Data", "Iso"));
            
            //SevenZipExtractor.SetLibraryPath(Path.Combine(Application.StartupPath, "Bin" ,"7z.dll"));
            ArchiveFile mySze = new(isoPath);
            mySze.Extract(Path.Combine(Application.StartupPath, "Data", "Iso"));
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