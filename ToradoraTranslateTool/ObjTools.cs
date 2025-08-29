using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using OBJEditor;

namespace ToradoraTranslateTool;

class ObjTools
{
    private static readonly string MainFilePath = Path.Combine(Application.StartupPath,"Data", "Translation.json");
    private static readonly string ToolsDirectory = Path.Combine(Application.StartupPath, "Resources", "!!Tools", "DatWorker");

    public static async Task ProcessObjGz(string directoryPath)
    {
        string objDir = Path.Combine(Application.StartupPath, "Data", "Obj");
        if (!Directory.Exists(objDir))
            Directory.CreateDirectory(objDir);

        string[] archives = Directory.GetFiles(directoryPath, "*.obj.gz", SearchOption.AllDirectories);

        List<Task> taskList = [];
        foreach (string archive in archives)
            taskList.Add(ProcessArchive(archive, objDir));
            
        await Task.WhenAll(taskList);
    }

    public static async Task ProcessArchive(string archive, string objDir) {
        if (Path.GetFileName(archive) == "STARTPOINT.obj.gz") // Save the original debug menu because it will be replaced by the one I translated
        {
            File.Copy(archive, Path.Combine(Application.StartupPath, "Resources", "DebugMode", "original_STARTPOINT.obj.gz"), true);
            return;
        }

        string objFolder = Path.Combine(objDir, Path.GetFileNameWithoutExtension(archive));
        Directory.CreateDirectory(objFolder);

        string gzCopyPath = Path.Combine(objFolder, Path.GetFileName(archive));
        File.Copy(archive, gzCopyPath, true);

        string relTxtPath = Path.Combine(objFolder, Path.GetFileNameWithoutExtension(archive) + ".txt");
        await File.WriteAllTextAsync(relTxtPath, archive.Replace(Application.StartupPath, ""));  // Write relative path to the original file in Data\Obj\%obj name%\%obj name%.txt

        string decompressedPath = Path.Combine(objFolder, Path.GetFileNameWithoutExtension(archive));
        await using FileStream input = File.OpenRead(gzCopyPath);
        await using FileStream output = File.Create(decompressedPath);
        await using GZipStream gzip = new (input, CompressionMode.Decompress);
        await gzip.CopyToAsync(output);
    }

    public static async Task ProcessTxtGz(string directoryPath)
    {
        string txtDir = Path.Combine(Application.StartupPath, "Data", "Txt");
        if (!Directory.Exists(txtDir))
            Directory.CreateDirectory(txtDir);

        string archive = Path.Combine(directoryPath, "text", "utf16.txt.gz");

        string txtFolder = Path.Combine(txtDir, Path.GetFileNameWithoutExtension(archive));
        Directory.CreateDirectory(txtFolder);

        string gzCopyPath = Path.Combine(txtFolder, Path.GetFileName(archive));
        File.Copy(archive, gzCopyPath, true);

        string relTxtPath = Path.Combine(txtFolder, Path.GetFileNameWithoutExtension(archive) + ".txt");
        await File.WriteAllTextAsync(relTxtPath, archive.Replace(Application.StartupPath, ""));

        string decompressedPath = Path.Combine(txtFolder, Path.GetFileNameWithoutExtension(archive));
        await using FileStream input = File.OpenRead(gzCopyPath);
        await using FileStream output = File.Create(decompressedPath);
        await using GZipStream gzip = new (input, CompressionMode.Decompress);
        await gzip.CopyToAsync(output);
    }

    public static async Task ProcessSeekmap(string firstDirectory)
    {
        string sourcePath = Path.Combine(firstDirectory, "seekmap.dat");
        string outPath = Path.Combine(ToolsDirectory, "seekmap.txt");

        await using FileStream input = File.OpenRead(sourcePath);
        await using FileStream output = File.Create(outPath);
        await using GZipStream gzip = new(input, CompressionMode.Decompress);
        await gzip.CopyToAsync(output);
    }
        
    public static async Task RepackObjs(bool debugMode)
    {
        List<string> directories = [];
        foreach (string path in Directory.GetDirectories(Path.Combine(Application.StartupPath, "Data", "Obj")))
            directories.Add(Path.GetFileName(path));

        if (!File.Exists(MainFilePath)) await File.WriteAllTextAsync(MainFilePath, "{ }");
        JObject mainFile = JObject.Parse(await File.ReadAllTextAsync(MainFilePath));

        Dictionary<string, string> translatedNames = new(); // Dictionary with pairs of original and translated names
        if (mainFile["names"] != null)
            foreach (JProperty name in mainFile["names"].Children())
                if (name.Value.ToString() != "") // If a translation for that name exists
                    translatedNames.Add(name.Name, name.Value.ToString());

        List<Task> taskList = [];
        foreach (string name in directories) 
            taskList.Add(RepackObj(name, mainFile[name], translatedNames));

        await Task.WhenAll(taskList);
            
        if (debugMode)
        {
            File.Copy(Path.Combine(Application.StartupPath, "Resources", "DebugMode", "_0000ESS1.obj.gz"), Path.Combine(Application.StartupPath, "Data", "Extracted", "resource", "script", "_0000ESS1", "_0000ESS1.0001", "_0000ESS1.obj.gz"), true); // This file enables debug mode
            File.Copy(Path.Combine(Application.StartupPath, "Resources", "DebugMode", "STARTPOINT.obj.gz"), Path.Combine(Application.StartupPath, "Data", "Extracted", "resource", "script", "STARTPOINT", "STARTPOINT.0001", "STARTPOINT.obj.gz"), true); // This is pretranslated debug menu 
        }
        else
            File.Copy(Path.Combine(Application.StartupPath, "Resources", "DebugMode", "original_STARTPOINT.obj.gz"), Path.Combine(Application.StartupPath, "Data", "Extracted", "resource", "script", "STARTPOINT", "STARTPOINT.0001", "STARTPOINT.obj.gz"), true); // Restore original debug menu        
    }

    public static async Task RepackObj(string name, JToken translation, Dictionary<string, string> translatedNames) {
        string filepath = Path.Combine(Application.StartupPath, "Data", "Obj", name, name);
        ObjHelper myHelper = new(await File.ReadAllBytesAsync(filepath));
        string[] scriptStrings = myHelper.Import();
        Dictionary<int, string> scriptNames = myHelper.Actors;

        bool haveTranslation = translation != null;
        for (int i = 0; i < scriptStrings.Length; i++)
        {
            if (haveTranslation && translation[i.ToString()] is {} words) {
                string translatedString = words.ToString(); // not null bleeehhh
                if (translatedString != "") 
                    scriptStrings[i] = translatedString;
            }

            if (scriptNames[i] != null && translatedNames.TryGetValue(scriptNames[i], out string value))
                scriptNames[i] = value;
        }

        byte[] data = myHelper.Export(scriptStrings);
        await using FileStream fileStream = File.Create(Application.StartupPath + await File.ReadAllTextAsync(filepath + ".txt"));
        await using GZipStream gzip = new(fileStream, CompressionLevel.Optimal);
        await gzip.WriteAsync(data);
    }
        
    public static async Task RepackTxts()
    {
        List<String> directories = [];
        foreach (string path in Directory.GetDirectories(Path.Combine(Application.StartupPath, "Data", "Txt")))
            directories.Add(Path.GetFileName(path));
        
        JObject mainFile = JObject.Parse(await File.ReadAllTextAsync(MainFilePath));
            
        List<Task> tasklist = [];
        foreach (string name in directories)
            if (mainFile[name] != null)
                tasklist.Add(RepackTxt(name, mainFile[name])); // If json have translation for that file

        await Task.WhenAll(tasklist);
    }

    public static async Task RepackTxt(string name, JToken translation) {
        string filepath = Path.Combine(Application.StartupPath, "Data", "Txt", name, name);
        string[] fileLines = await File.ReadAllLinesAsync(filepath, new UnicodeEncoding(false, false));

        for (int i = 0; i < fileLines.Length; i++)
        {
            string translatedString = translation[i.ToString()]!.ToString(); // not nullll blehhhh
            if (!string.IsNullOrEmpty(translatedString))
                fileLines[i] = translatedString;
        }
                
        string destFile = Application.StartupPath + await File.ReadAllTextAsync(filepath + ".txt");
        byte[] data = new UnicodeEncoding(false, false).GetBytes(string.Join("\r\n", fileLines));
        await using FileStream file = File.Create(destFile);
        await using GZipStream gzip = new(file, CompressionLevel.Optimal);
        await gzip.WriteAsync(data);
    }

    public static async Task RepackSeekmap(string resourcePath, string firstDirectory) {
        File.Copy(resourcePath, Path.Combine(ToolsDirectory, "RES.dat"), true); // RES.dat and seekmap.txt required for modseekmap.exe
        using (Process myProc = new()) {
            myProc.StartInfo.FileName = Path.Combine(ToolsDirectory, "modseekmap.exe"); // modseekmap generates seekmap.new file
            myProc.StartInfo.WorkingDirectory = ToolsDirectory;
            myProc.StartInfo.CreateNoWindow = true;
            myProc.StartInfo.UseShellExecute = false;
            myProc.StartInfo.RedirectStandardError = true;
            myProc.StartInfo.RedirectStandardOutput = true;
            myProc.ErrorDataReceived += (_, args) => { if (args.Data != null) Console.WriteLine(args.Data); };
            myProc.OutputDataReceived += (_, args) => { if (args.Data != null) Console.WriteLine(args.Data); };
            
            myProc.Start();
            await myProc.WaitForExitAsync();
        }

        // rename seekmap.new to seekmap.dat
        string oldName = Path.Combine(ToolsDirectory, "seekmap.new");
        string newName = Path.Combine(ToolsDirectory, "seekmap.dat");
        File.Move(oldName, newName, true);

        string inputFile = Path.Combine(ToolsDirectory, "seekmap.dat");
        string outputFile = Path.Combine(Path.Combine(firstDirectory, "seekmap.dat"));

        if (File.Exists(outputFile)) File.Delete(outputFile);

        await using FileStream input = File.OpenRead(inputFile);
        await using FileStream output = File.Create(outputFile);
        await using GZipStream gzip = new(output, CompressionLevel.Optimal);
        await input.CopyToAsync(gzip);
    }
}