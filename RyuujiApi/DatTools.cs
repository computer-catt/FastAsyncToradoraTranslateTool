using System.Diagnostics;

namespace RyuujiApi;

#pragma warning disable CS0162 // Unreachable code detected

static class DatTools {
    private const bool ProcessMethod = false;

    public static async Task ExtractDat(string startupPath, string datPath) {
        string baseDirectory = Path.Combine(startupPath, "Data", "Extracted");
        if (!Directory.Exists(baseDirectory)) Directory.CreateDirectory(baseDirectory);

        string newPath = Path.Combine(baseDirectory, Path.GetFileName(datPath));
        File.Copy(datPath, newPath, true);

        if (!ProcessMethod) {
            await new DatWorker.DatWorker(baseDirectory).Process([newPath]);
            return;
        }

        using Process myProc = new();
        myProc.StartInfo.FileName = Path.Combine(startupPath, "DatWorker.exe");
        myProc.StartInfo.Arguments = '"' + newPath + '"';
        myProc.StartInfo.WorkingDirectory = baseDirectory;
        myProc.Start();

        await myProc.WaitForExitAsync();
    }

    public static async Task RepackDat(string startupPath, string lstPath) {
        string workingDir = Path.Combine(startupPath, "Data", "Extracted");
        if (!ProcessMethod) {
            await new DatWorker.DatWorker(workingDir).Process([lstPath]);
            return;
        }

        using Process myProc = new();
        myProc.StartInfo.FileName = Path.Combine(startupPath, "DatWorker.exe");
        myProc.StartInfo.Arguments = '"' + lstPath + '"';
        myProc.StartInfo.WorkingDirectory = workingDir;
        myProc.Start();

        await myProc.WaitForExitAsync();
    }
}