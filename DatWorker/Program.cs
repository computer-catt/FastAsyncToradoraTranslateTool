using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DatWorker;

public static class Program {
    public static void Main(string[] args) => Process(Directory.GetCurrentDirectory(), args);

    public static void Process(string workingDir, string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("-------- fatdtt --------");
            Console.WriteLine("Toradora DAT Automation Tool");
            Console.WriteLine("Drop Dat files");
        }

        foreach (string arg in args) {
            if (arg.ToLower().EndsWith("-LstOrder.lst".ToLower())) {
                SaveDat(workingDir, File.ReadAllLines(arg));
            }
            else {
                LSTOrder = new List<string>();
                ResetWorkspace(workingDir);
                OpenDat(workingDir, arg);
                LSTOrder.Reverse();
                File.WriteAllLines(arg + "-LstOrder.lst", LSTOrder.ToArray());
            }
        }
        //Console.ReadLine();
    }

    private static void ResetWorkspace(string workingDir) {
        Console.WriteLine("Starting...");
        string[] Files = Directory.GetFiles(Path.Combine(workingDir, ".\\Workspace"), "*.dat", SearchOption.TopDirectoryOnly);
        foreach (string DAT in Files) {
            if (File.Exists(Path.Combine(workingDir, ".\\", Path.GetFileName(DAT))))
                File.Delete(DAT);
            else
                File.Move(DAT, Path.Combine(workingDir, ".\\", Path.GetFileName(DAT)));
        }
    }

    static List<string> LSTOrder = [];
    const string TMP = @".\Workspace\{0}";

    #region EXTRACT

    static bool OpenDat(string workingDir, string DAT) {
        try {
            Console.WriteLine("Extracting: {0}", Path.GetFileName(DAT));
            string[] Files = ExtractDatContent(workingDir, DAT);
            foreach (string File in Files) {
                if (Path.GetExtension(File).ToLower().Trim(' ', '.') == "dat") {
                    OpenDat(workingDir, File);
                }
            }

            return true;
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
            throw;
            return false;
        }
    }

    static string[] ExtractDatContent(string workingDir, string DAT) {
        
        string TMPPath = Path.Combine(workingDir, @"Workspace\", Path.GetFileName(DAT));
        if (File.Exists(TMPPath))
            File.Delete(TMPPath);

        File.Move(DAT, TMPPath);

        string NewDir = Path.Combine(workingDir, Path.GetDirectoryName(DAT), Path.GetFileNameWithoutExtension(DAT) + "\\");
        if (NewDir.StartsWith("\\"))
            NewDir = "." + NewDir;

        string xpBat = Path.Combine(workingDir, "Workspace\\!xp.bat");
        var startinfo = new ProcessStartInfo("cmd.exe", $"/C {xpBat}") {
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        List<string> FLIST = new();
        string isPadded = "N";
        var process = new Process();
        process.StartInfo = startinfo;
        process.ErrorDataReceived += (_, args) => {
            if (args.Data == null)
                return;

            string Line = args.Data.Trim();

            if (Line.StartsWith("Is padded with space: ")) {
                isPadded = Line.Replace("Is padded with space: ", "").Substring(0, 1);
            }

            if (!Line.StartsWith("^")) {
                if (!Line.Contains("#") || !Line.Contains("@")) {
                    return;
                }

                string DFN = GetDatFN(Line.Split('#')[1].Split('@')[0].Trim('\t', ' ', ','));
                FLIST.Add(DFN);
                //Console.Title = "Processing: " + DFN; // TODO
                return;
            }

            FLIST[FLIST.Count - 1] = ".\\" + Line.Trim('^', ' ', '\t') + '\t' + FLIST.Last();
        };
        process.Start();
        process.BeginErrorReadLine();
        process.WaitForExit();


        File.Move(TMPPath, DAT);

        string directory = Path.Combine(workingDir, @"Workspace\", Path.GetFileNameWithoutExtension(DAT));
        bool directoryExist = Directory.Exists(directory);
        if (!directoryExist) return [];
        
        if (Directory.Exists(NewDir))
            Directory.Delete(NewDir, true);
        Directory.Move(Path.Combine(workingDir, @"Workspace\", Path.GetFileNameWithoutExtension(DAT)), NewDir);

        string TXT = isPadded;
        foreach (string File in FLIST.ToArray())
            TXT += "\r\n" + File;

        string lst = GetDatLFN(DAT);
        if (lst.StartsWith("\\"))
            lst = "." + lst;

        if (File.Exists(lst)) File.Delete(lst);

        File.WriteAllText(lst, TXT);
        LSTOrder.Add(lst);

        return Directory.GetFiles(NewDir, "*", SearchOption.AllDirectories);

    }

    private static string GetDatFN(string file) {
        string[] Splits = Path.GetFileName(file).Split('.');
        if (Splits.Length >= 3 && int.TryParse(Splits[Splits.Length - 2], out int tmp)) {
            string FN = string.Empty;
            for (int i = 0; i < Splits.Length; i++) {
                if (int.TryParse(Splits[i], out int TMP) && Splits[i].StartsWith("0"))
                    continue;
                FN += Splits[i] + ".";
            }

            return FN.TrimEnd('.');
        }

        return Path.GetFileName(file);
    }

    private static string GetDatLFN(string file) {
        return Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".lst";
    }

    #endregion

    #region REPACK

    static bool SaveDat(string workingDir, string[] Files) {
        foreach (string File in Files) {
            if (!System.IO.File.Exists(File))
                continue;

            Console.WriteLine("Repacking: {0}", Path.GetFileName(File));
            RepackDat(workingDir, File);
        }

        return true;
    }

    static bool RepackDat(string workingDir, string DAT) {
        try {
            string DatDir = Path.GetDirectoryName(DAT) + "\\";
            if (DatDir.StartsWith("\\"))
                DatDir = '.' + DatDir;

            if (DatDir.StartsWith(".\\")) {
                DatDir = Path.Combine(workingDir, DatDir.Substring(2, DatDir.Length - 2));
            }

            Process Proc = new() {
                StartInfo = new ProcessStartInfo {
                    FileName = Path.Combine(workingDir, "Workspace\\makeGDP.exe"),
                    Arguments = "\"" + DatDir + Path.GetFileNameWithoutExtension(DAT) + "\"",
                    WorkingDirectory = DatDir,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            Proc.Start();
            Proc.WaitForExit();

            return true;
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
            throw;
            //return false;
        }
    }

    #endregion
}