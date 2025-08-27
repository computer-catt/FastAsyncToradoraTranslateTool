using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using OBJEditor;

namespace ToradoraTranslateTool
{
    class ObjTools
    {
        static string mainFilePath = Path.Combine(Application.StartupPath, "Translation.json");
        static string toolsDirectory = Path.Combine(Application.StartupPath, "Resources", "!!Tools", "DatWorker");

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
            string outPath = Path.Combine(toolsDirectory, "seekmap.txt");

            await using FileStream input = File.OpenRead(sourcePath);
            await using FileStream output = File.Create(outPath);
            await using GZipStream gzip = new(input, CompressionMode.Decompress);
            await gzip.CopyToAsync(output);
        }
        
        public static async Task RepackObjs(bool debugMode)
        {
            List<string> directories = [];
            directories.AddRange(Directory.GetDirectories(Path.Combine(Application.StartupPath, "Data", "Obj")).Select(Path.GetFileName));

            JObject mainFile = JObject.Parse(await File.ReadAllTextAsync(mainFilePath));

            Dictionary<string, string> translatedNames = new(); // Dictionary with pairs of original and translated names
            if (mainFile["names"] != null)
            {
                foreach (JProperty name in mainFile["names"].Children().ToArray())
                {
                    if (name.Value.ToString() != "") // If a translation for that name exists
                        translatedNames.Add(name.Name, name.Value.ToString());
                }
            }

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
            OBJHelper myHelper = new(await File.ReadAllBytesAsync(filepath));
            string[] scriptStrings = myHelper.Import();
            Dictionary<int, string> scriptNames = myHelper.actors;

            bool haveTranslation = translation != null;
            for (int i = 0; i < scriptStrings.Length; i++)
            {
                if (haveTranslation)
                {
                    string translatedString = translation![i.ToString()]!.ToString(); // not null bleeehhh
                    if (translatedString != "")
                        scriptStrings[i] = translatedString;
                }

                if (scriptNames[i] != null && translatedNames.TryGetValue(scriptNames[i], out string value))
                    scriptNames[i] = value;
            }

            byte[] data = myHelper.Export(scriptStrings);
            await using FileStream fileStream = File.Create(Application.StartupPath + File.ReadAllText(filepath + ".txt"));
            await using GZipStream gzip = new(fileStream, CompressionLevel.Optimal);
            await gzip.WriteAsync(data);
        }
        
        public static void RepackTxt()
        {
            List<String> directories = new();
            directories.AddRange(Directory.GetDirectories(Path.Combine(Application.StartupPath, "Data", "Txt")).Select(Path.GetFileName));

            JObject mainFile = JObject.Parse(File.ReadAllText(mainFilePath));
            foreach (string name in directories)
            {
                if (mainFile[name] != null)  // If json have translation for that file
                {
                    string filepath = Path.Combine(Application.StartupPath, "Data", "Txt", name, name);
                    string[] fileLines = File.ReadAllLines(filepath, new UnicodeEncoding(false, false)); ;

                    for (int i = 0; i < fileLines.Length; i++)
                    {
                        string translatedString = mainFile[name][i.ToString()].ToString();
                        if (translatedString != "")
                            fileLines[i] = translatedString;
                    }

                    File.WriteAllLines(Path.Combine(toolsDirectory, name), fileLines, new UnicodeEncoding(false, false));

                    Process myProc = new();
                    myProc.StartInfo.FileName = Path.Combine(toolsDirectory, "gzip.exe");
                    myProc.StartInfo.Arguments = "-n9 -f " + name; // Without -n9 the game will freeze
                    myProc.StartInfo.WorkingDirectory = toolsDirectory;
                    myProc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    myProc.Start();
                    myProc.WaitForExit();

                    File.Delete(Path.Combine(toolsDirectory, name));

                    File.Replace(Path.Combine(toolsDirectory, name + ".gz"), Application.StartupPath + File.ReadAllText(filepath + ".txt"), null);
                }
            }
        }

        public static void RepackSeekmap(string resourcePath, string firstDirectory)
        {
            File.Copy(resourcePath, Path.Combine(toolsDirectory, "RES.dat"), true); // RES.dat and seekmap.txt required for modseekmap.exe
            Process myProc = new();
            myProc.StartInfo.FileName = Path.Combine(toolsDirectory, "modseekmap.exe"); // modseekmap generates seekmap.new file
            myProc.StartInfo.WorkingDirectory = toolsDirectory;
            myProc.Start();
            myProc.WaitForExit();
            File.Move(Path.Combine(toolsDirectory, "seekmap.new"), Path.Combine(toolsDirectory, "seekmap.dat")); // rename seekmap.new to seekmap.dat

            myProc = new Process(); // compress seekmap.dat to seekmap.dat.gz
            myProc.StartInfo.FileName = Path.Combine(toolsDirectory, "gzip.exe");
            myProc.StartInfo.Arguments = "-n9 -f seekmap.dat"; // Without -n9 the game will freeze
            myProc.StartInfo.WorkingDirectory = toolsDirectory;
            myProc.Start();
            myProc.WaitForExit();
            File.Copy(Path.Combine(toolsDirectory, "seekmap.dat.gz"), Path.Combine(firstDirectory, "seekmap.dat"), true); // rename seekmap.dat.gz to seekmap.dat and move it to the directory where first.dat was unpacked

            File.Delete(Path.Combine(toolsDirectory, "RES.dat")); // Remove temp files
            File.Delete(Path.Combine(toolsDirectory, "seekmap.dat.gz"));
        }

    }
}