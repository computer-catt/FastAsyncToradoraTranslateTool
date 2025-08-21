using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using tenoriTool;

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
            await OpenDat(arg);
            lstOrder.Reverse();
            File.WriteAllLines(arg + "-LstOrder.lst", lstOrder.ToArray());
        }
        //Console.ReadLine();
    }

    private List<string> lstOrder = [];
    
    #region EXTRACT

    private async Task<bool> OpenDat(string dat) {
        try {
            _ = Console.Out.WriteLineAsync($"Extracting: {dat}"); // dont wait for me guys
            string[] files = await ExtractDatContent(dat);
            List<Task> taskList = [];
            foreach (string file in files)
                if (Path.GetExtension(file).ToLower().Trim(' ', '.') == "dat")
                    taskList.Add(OpenDat(file));

            await Task.WhenAll(taskList);
            return true;
        }
        catch (Exception e) {
            Console.WriteLine(e.Message);
            throw;
            //return false;
        }
    }

    private async Task<string[]> ExtractDatContent(string dat) {
        string newDir = Path.Combine(workingDir, Path.GetDirectoryName(dat), Path.GetFileNameWithoutExtension(dat));
        if (newDir.StartsWith("\\"))
            newDir = "." + newDir;

        TenoriToolApi.TenoriCallbacks callbacks = TenoriToolApi.TenoriCallbacks.None();
        TenoriToolApi.ProcessReturn processReturn = await TenoriToolApi.ProcessIndividualExtract("", newDir, false, true, "", dat, callbacks);
        bool directoryExist = Directory.Exists(newDir);
        if (!directoryExist) return [];
        string txt = processReturn.MakeGpdaFileContent;

        string lst = GetDatLfn(dat);
        if (lst.StartsWith("\\"))
            lst = "." + lst;

        if (File.Exists(lst)) File.Delete(lst);

        await File.WriteAllTextAsync(lst, txt);
        lstOrder.Add(lst);

        return Directory.GetFiles(newDir, "*", SearchOption.AllDirectories);
    }

    private string GetDatLfn(string file) => Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".lst";

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
                    FileName = Path.Combine(workingDir, "makeGDP.exe"),
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