using CppPorts;

if (args.Length != 1) Console.Out.WriteLine("usage: makeGPDA [target file]");
else MakeGpda.Process(args[0]);