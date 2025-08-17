using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToradoraTranslateTool
{
    class DatTools
    {
        const bool ProcessMethod = false;
        public static async Task ExtractDat(string datPath) {
            string newPath = Path.Combine(Application.StartupPath, @"Data\DatWorker\", Path.GetFileName(datPath));
            File.Copy(datPath, newPath, true);

            if (!ProcessMethod) {
                await new DatWorker.DatWorker(Path.Combine(Application.StartupPath, @"Data\DatWorker\")).Process([newPath]);
                return;
            }
            
            Process myProc = new ();
            myProc.StartInfo.FileName = Path.Combine(Application.StartupPath, @"DatWorker.exe");
            myProc.StartInfo.Arguments = '"' + newPath + '"'; // Add commas to ignore spaces in path
            myProc.StartInfo.WorkingDirectory = Path.Combine(Application.StartupPath, @"Data\DatWorker\");
            myProc.Start();

            await myProc.WaitForExitAsync();
        }

        public static async Task RepackDat(string lstPath)
        {
            if (!ProcessMethod) {
                await new DatWorker.DatWorker(Path.Combine(Application.StartupPath, @"Data\DatWorker\")).Process([lstPath]);
                return;
            }
            
            Process myProc = new ();
            myProc.StartInfo.FileName = Path.Combine(Application.StartupPath, "DatWorker.exe");//@"Data\DatWorker\Dat Worker.exe");
            myProc.StartInfo.Arguments = '"' + lstPath + '"'; // Add commas to ignore spaces in path
            myProc.StartInfo.WorkingDirectory = Path.Combine(Application.StartupPath, @"Data\DatWorker\");
            myProc.Start();

            await myProc.WaitForExitAsync();
        }
    }
}