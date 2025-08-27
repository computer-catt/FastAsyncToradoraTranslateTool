using System;

namespace MakeGpda;

public class Cli {
    public static void Main(string[] args) {
        if (args.Length != 1) {
            _ = Console.Out.WriteLineAsync("usage: makeGPDA [target file]");
            return;
        }
        
        MakeGpdaApi.MakeGpda(args[0]);
    }
}