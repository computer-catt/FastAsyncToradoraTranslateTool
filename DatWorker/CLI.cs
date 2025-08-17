using System.IO;

namespace DatWorker;

public static class Cli {
    private static void Main(string[] args) => new DatWorker(Directory.GetCurrentDirectory()).Process(args).Wait();
}