using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DatWorker;

public class DatWorker(string workingDir) {
    public async Task Process(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("### Fast Async Datworker ####");
            Console.WriteLine("Toradora DAT Automation Tool");
            Console.WriteLine("Drop Dat files");
            return;
        }

        foreach (string arg in args) {
            if (arg.ToLower().EndsWith("-LstOrder.lst".ToLower())) { // Repack
                await SaveDat(workingDir, await File.ReadAllLinesAsync(arg));
                continue;
            }
            
            // unpack
            //ResetWorkspace(workingDir);
            await OpenDat(arg);
            lstOrder.Reverse();
            File.WriteAllLines(arg + "-LstOrder.lst", lstOrder.ToArray());
        }
        //Console.ReadLine();
    }

    private static void ResetWorkspace(string workingDir) {
        Console.WriteLine("Starting...");
        string[] files = Directory.GetFiles(Path.Combine(workingDir, ".\\Workspace"), "*.dat", SearchOption.TopDirectoryOnly);
        foreach (string dat in files) {
            if (File.Exists(Path.Combine(workingDir, ".\\", Path.GetFileName(dat))))
                File.Delete(dat);
            else
                File.Move(dat, Path.Combine(workingDir, ".\\", Path.GetFileName(dat)));
        }
    }

    private List<string> lstOrder = [];

    #region EXTRACT

    private async Task<bool> OpenDat(string dat) {
        try {
            Console.WriteLine("Extracting: {0}", Path.GetFileName(dat));
            string[] files = await ExtractDatContent(dat);

            List<Task> taskList = [];
            
            foreach (string file in files) {
                if (Path.GetExtension(file).ToLower().Trim(' ', '.') == "dat") {
                    await OpenDat(file);
                }
            }

            return true;
        }
        catch (Exception e) {
            Console.WriteLine(e.Message);
            throw;
            //return false;
        }
    }

    private async Task<string[]> ExtractDatContent(string dat) {

        string tmpPath = Path.Combine(workingDir, @"Workspace\", Path.GetFileName(dat));
        if (File.Exists(tmpPath))
            File.Delete(tmpPath);

        File.Move(dat, tmpPath);

        string newDir = Path.Combine(workingDir, Path.GetDirectoryName(dat), Path.GetFileNameWithoutExtension(dat) + "\\");
        if (newDir.StartsWith("\\"))
            newDir = "." + newDir;

        string xpBat = Path.Combine(workingDir, "Workspace\\!xp.bat");
        var startinfo = new ProcessStartInfo("cmd.exe", $"/C {xpBat}") {
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        List<string> flist = new();
        string isPadded = "N";
        var process = new Process();
        process.StartInfo = startinfo;
        process.ErrorDataReceived += (_, args) => {
            if (args.Data == null)
                return;

            string line = args.Data.Trim();

            if (line.StartsWith("Is padded with space: ")) {
                isPadded = line.Replace("Is padded with space: ", "").Substring(0, 1);
            }

            if (!line.StartsWith("^")) {
                if (!line.Contains("#") || !line.Contains("@")) {
                    return;
                }

                string dfn = GetDatFn(line.Split('#')[1].Split('@')[0].Trim('\t', ' ', ','));
                flist.Add(dfn);
                //Console.Title = "Processing: " + DFN; // TODO
                return;
            }

            flist[^1] = ".\\" + line.Trim('^', ' ', '\t') + '\t' + flist.Last();
        };
        process.Start();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();


        File.Move(tmpPath, dat);

        string directory = Path.Combine(workingDir, @"Workspace\", Path.GetFileNameWithoutExtension(dat));
        bool directoryExist = Directory.Exists(directory);
        if (!directoryExist) return [];

        if (Directory.Exists(newDir))
            Directory.Delete(newDir, true);
        Directory.Move(Path.Combine(workingDir, @"Workspace\", Path.GetFileNameWithoutExtension(dat)), newDir);

        string txt = isPadded;
        foreach (string file in flist.ToArray())
            txt += "\r\n" + file;

        string lst = GetDatLfn(dat);
        if (lst.StartsWith("\\"))
            lst = "." + lst;

        if (File.Exists(lst)) File.Delete(lst);

        await File.WriteAllTextAsync(lst, txt);
        lstOrder.Add(lst);

        return Directory.GetFiles(newDir, "*", SearchOption.AllDirectories);
    }

    private string GetDatFn(string file) {
        string[] splits = Path.GetFileName(file).Split('.');
        
        // if the file has less than 3 dots or cannot be properly parsed
        if (splits.Length < 3 || !int.TryParse(splits[splits.Length - 2], out int _)) return Path.GetFileName(file);
        
        string fn = string.Empty;
        foreach (string section in splits) {
            if (int.TryParse(section, out int _) && section.StartsWith("0"))
                continue;
            fn += section + ".";
        }

        return fn.TrimEnd('.');

    }

    private string GetDatLfn(string file) {
        return Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".lst";
    }

    #endregion

    #region REPACK

    private static async Task<bool> SaveDat(string workingDir, string[] files) {
        foreach (string file in files) {
            if (!File.Exists(file))
                continue;

            Console.WriteLine("Repacking: {0}", Path.GetFileName(file));
            await RepackDat(workingDir, file);
        }

        return true;
    }

    private static async Task<bool> RepackDat(string workingDir, string dat) {
        try {
            string datDir = Path.GetDirectoryName(dat) + "\\";
            if (datDir.StartsWith("\\"))
                datDir = '.' + datDir;

            if (datDir.StartsWith(".\\")) {
                datDir = Path.Combine(workingDir, datDir.Substring(2, datDir.Length - 2));
            }

            Process proc = new() {
                StartInfo = new ProcessStartInfo {
                    FileName = Path.Combine(workingDir, "Workspace\\makeGDP.exe"),
                    Arguments = "\"" + datDir + Path.GetFileNameWithoutExtension(dat) + "\"",
                    WorkingDirectory = datDir,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            proc.Start();
            await proc.WaitForExitAsync();

            return true;
        }
        catch (Exception e) {
            Console.WriteLine(e.Message);
            throw;
            //return false;
        }
    }

    #endregion
}