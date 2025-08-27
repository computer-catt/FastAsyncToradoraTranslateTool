using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MakeGpda;
using Newtonsoft.Json;
using tenoriTool;

namespace DatWorker;

public class DatWorker(string workingDir) {
    private readonly JsonSerializerSettings settings = new() {
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore,
        //Formatting = Formatting.Indented
    };
    
    public async Task Process(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("### Fast Async Dat Worker ####");
            Console.WriteLine("Toradora DAT Automation Tool");
            Console.WriteLine("Drop Dat files");
            return;
        }

        foreach (string arg in args) {
            if (arg.ToLower().EndsWith("-LstOrder.lst".ToLower())) { // Repack
                string lstOrder = await File.ReadAllTextAsync(arg);
                DatTree repackDatTree = JsonConvert.DeserializeObject<DatTree>(lstOrder);
                await SaveDat(workingDir, repackDatTree);
                Console.WriteLine($"Done repacking {arg}");
                continue;
            }
            
            // unpack
            DatTree datTree = new(GetDatLstDir(arg));
            await OpenDat(arg, datTree);
            await File.WriteAllTextAsync(arg + "-LstOrder.lst", JsonConvert.SerializeObject(datTree, settings));
            Console.WriteLine($"Done unpacking {arg}");
        }
        //Console.ReadLine();
    }
    
    public class DatTree(string name) {
        [JsonProperty("n")] public string Name = name;
        [JsonProperty("c")] public List<DatTree> Children = null;
    }
    
    
    #region EXTRACT

    private async Task<DatTree> OpenDat(string dat, DatTree parent) {
        try {
            _ = Console.Out.WriteLineAsync($"Extracting: {dat}"); // Don't wait for me guys
            var files = await ExtractDatContent(dat);
            if (files == null) return new DatTree(null);
            List<Task<DatTree>> taskList = [];
            foreach (string file in files)
                if (Path.GetExtension(file).ToLower().Trim(' ', '.') == "dat")
                    taskList.Add(OpenDat(file, new (GetDatLstDir(file))));

            await Task.WhenAll(taskList);

            if (taskList.Count == 0) return parent;
            
            parent.Children ??= [];
            foreach (Task<DatTree> task in taskList)
                if (task.Result.Name != null) 
                    parent.Children.Add(task.Result);
            
            return parent;
        }
        catch (Exception e) {
            Console.WriteLine(e.Message);
            throw;
            //return false;
        }
    }

    private async Task<string[]> ExtractDatContent(string dat) {
        string newDir = Path.Combine(workingDir, Path.GetDirectoryName(dat)!, Path.GetFileNameWithoutExtension(dat));
        if (newDir.StartsWith("\\"))
            newDir = "." + newDir;

        TenoriToolApi.TenoriCallbacks callbacks = TenoriToolApi.TenoriCallbacks.None();
        TenoriToolApi.ProcessReturn processReturn = await TenoriToolApi.ProcessIndividualExtract("", newDir, false, true, "", dat, callbacks);
        bool directoryExist = Directory.Exists(newDir);
        if (!directoryExist) return null;
        //File.Delete(dat);
        string txt = processReturn.MakeGpdaFileContent;

        string lst = GetDatLstDir(dat);
        if (lst.StartsWith('\\'))
            lst = "." + lst;

        if (File.Exists(lst)) File.Delete(lst);
        await File.WriteAllTextAsync(lst, txt);

        return Directory.GetFiles(newDir, "*", SearchOption.AllDirectories);
    }

    private static string GetDatLstDir(string file) => Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".lst";

    #endregion

    #region REPACK
    private static async Task<bool> SaveDat(string workingDir, DatTree datTree) {
        if (datTree.Children != null) {
            List<Task> tasks = [];
            foreach (DatTree tree in datTree.Children)
                tasks.Add(SaveDat(workingDir, tree));
            await Task.WhenAll(tasks);
        }
        
        if (!File.Exists(datTree.Name)) return false; 
        _ = Console.Out.WriteLineAsync($"Repacking: {datTree.Name}"); // It's okay! il catch you guys later
        await RepackDat(workingDir, datTree.Name);
        return true;
    }

    private static async Task<bool> RepackDat(string workingDir, string lst) {
        try {
            string lstDir = Path.GetDirectoryName(lst) + "\\";
            if (lstDir.StartsWith("\\"))
                lstDir = '.' + lstDir;

            if (lstDir.StartsWith(".\\"))
                lstDir = Path.Combine(workingDir, lstDir.Substring(2, lstDir.Length - 2));

            string file = lstDir + Path.GetFileNameWithoutExtension(lst);
            
            await MakeGpdaApi.MakeGpda(file, lstDir);
            /*Process proc = new() {
                StartInfo = new ProcessStartInfo {
                    FileName = Path.Combine(workingDir, "makeGDP.exe"),
                    Arguments = "\"" + file + "\"",
                    WorkingDirectory = lstDir,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            proc.Start();
            await proc.WaitForExitAsync();*/

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