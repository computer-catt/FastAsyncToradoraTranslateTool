using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

#pragma warning disable CS0162 // Unreachable code detected

namespace ToradoraTranslateTool;

static class DatTools
{
    private const bool ProcessMethod = false;
    public static async Task ExtractDat(string datPath) {
        string baseDirectory = Path.Combine(Application.StartupPath, @"Data\Extracted\");
        if (!Directory.Exists(baseDirectory)) Directory.CreateDirectory(baseDirectory);
            
        string newPath = Path.Combine(baseDirectory, Path.GetFileName(datPath));
        File.Copy(datPath, newPath, true);

        string workingDir = Path.Combine(Application.StartupPath, "Resources", "!!Tools", "DatWorker");
        if (!ProcessMethod) {
            await new DatWorker.DatWorker(workingDir).Process([newPath]);
            return;
        }

        using Process myProc = new();
        myProc.StartInfo.FileName = Path.Combine(Application.StartupPath, @"DatWorker.exe");
        myProc.StartInfo.Arguments = '"' + newPath + '"';
        myProc.StartInfo.WorkingDirectory = workingDir;
        myProc.Start();

        await myProc.WaitForExitAsync();
    }

    public static async Task RepackDat(string lstPath)
    {
        string workingDir = Path.Combine(Application.StartupPath, "Resources", "!!Tools", "DatWorker");
        if (!ProcessMethod) {
            await new DatWorker.DatWorker(workingDir).Process([lstPath]);
            return;
        }
            
        using Process myProc = new();
        myProc.StartInfo.FileName = Path.Combine(Application.StartupPath, "DatWorker.exe");
        myProc.StartInfo.Arguments = '"' + lstPath + '"';
        myProc.StartInfo.WorkingDirectory = workingDir;
        myProc.Start();

        await myProc.WaitForExitAsync();
    }
}