using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ToradoraTranslateTool
{
    class DatTools
    {
        const bool ProcessMethod = false;
        public static void ExtractDat(string datPath) {
            var a = new Stopwatch();
            a.Start();
            string newPath = Path.Combine(Application.StartupPath, @"Data\DatWorker\", Path.GetFileName(datPath));
            File.Copy(datPath, newPath, true);

            if (!ProcessMethod) {
                DatWorker.Program.Process(Path.Combine(Application.StartupPath, @"Data\DatWorker\"), [newPath]);
                a.Stop();
                Console.WriteLine(a.ElapsedMilliseconds);
                return;
            }
            
            Process myProc = new ();
            myProc.StartInfo.FileName = Path.Combine(Application.StartupPath, @"DatWorker.exe");
            myProc.StartInfo.Arguments = '"' + newPath + '"'; // Add commas to ignore spaces in path
            myProc.StartInfo.WorkingDirectory = Path.Combine(Application.StartupPath, @"Data\DatWorker\");
            myProc.Start();

            myProc.WaitForExit();
            
            a.Stop();
            Console.WriteLine(a.ElapsedMilliseconds);
        }

        public static void RepackDat(string lstPath)
        {
            if (!ProcessMethod) {
                DatWorker.Program.Process(Path.Combine(Application.StartupPath, @"Data\DatWorker\"), [lstPath]);
                return;
            }
            
            Process myProc = new ();
            myProc.StartInfo.FileName = Path.Combine(Application.StartupPath, "DatWorker.exe");//@"Data\DatWorker\Dat Worker.exe");
            myProc.StartInfo.Arguments = '"' + lstPath + '"'; // Add commas to ignore spaces in path
            myProc.StartInfo.WorkingDirectory = Path.Combine(Application.StartupPath, @"Data\DatWorker\");
            myProc.Start();

            myProc.WaitForExit();
        }
    }
}