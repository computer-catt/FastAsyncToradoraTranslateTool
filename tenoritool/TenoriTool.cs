#region Copyright (C) 2008 Giacomo Stelluti Scala
//
// Copy Directory Tree: Program.cs
//
// Author:
//   Giacomo Stelluti Scala (giacomo.stelluti@gmail.com)
//
// Copyright (C) 2008 Giacomo Stelluti Scala
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using tenoritool.Properties;

namespace tenoriTool;

internal static partial class TenoriTool {
    #region Exit Code Constants

    private const int ExitSuccess = 0;
    private const int ExitFailure = 1;
    private const int ExitFailureCritical = 2;

    #endregion

    private static readonly HeadingInfo Heading = new(ThisAssembly.Name, ThisAssembly.MajorMinorVersion);
    private static readonly ConsoleColor HighlightColor = ConsoleColor.Yellow;
    
    private static void Main(string[] args) {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Options options = new();
        
        ICommandLineParser parser = new CommandLineParser();
        if (!parser.ParseArguments(args, options, Console.Error)) {
            Environment.Exit(ExitFailure);
        }

        if (!options.Validate()) {
            Console.Error.WriteLine("Try '{0} --help' for more information.", ThisAssembly.Name);
            Environment.Exit(ExitFailure);
        }

        if (options.Verbose) {
            Console.ForegroundColor = HighlightColor;
            Console.Error.WriteLine(Heading.ToString());
            Console.ResetColor();
        }

        bool success;// = true;
        //if (!options.UsePackMode) {
            if (options.UseExtractMode)
                options.ExtractStub = TenoriToolApi.ExtractEntry; // use real function
            success = ProcessIterationExtract(options).Result;
        //}

        #region Helper Code while running inside an IDE (uncomment when needed)

        //Console.ForegroundColor = ConsoleColor.Green;
        //Console.Write(">>>press any key<<<");
        //Console.ResetColor();
        //Console.ReadKey();

        #endregion

        Environment.Exit(success ? ExitSuccess : ExitFailureCritical);
    }

    private static async Task<bool> ProcessIterationExtract(Options options) {
        bool hasError = false;
        foreach (string inpath in options.Paths) {
            string displayFilename;

            bool isStdInput = inpath.Equals("-", StringComparison.InvariantCulture);
            displayFilename = isStdInput ? "<STDIN>" : Path.GetFileName(inpath);

            if (options.Verbose) {
                Console.ForegroundColor = ConsoleColor.White;
                await Console.Error.WriteLineAsync($"* {displayFilename}");
                Console.ResetColor();
            }

            if (isStdInput) continue;
            
            string ioerror = string.Empty;
            try {
                FileAttributes attributes = File.GetAttributes(inpath);
                FileInfo infile = new(inpath);
                if ((attributes & FileAttributes.Directory) == FileAttributes.Directory || !infile.Exists) {
                    ioerror = "does not exist or is a directory";
                }
                else {
                    if (options.Verbose)
                        await Console.Error.WriteLineAsync($"    Reported size: {infile.Length}");

                    TenoriToolApi.TenoriCallbacks callbacks = TenoriToolApi.TenoriCallbacks.Default(highlightColor: HighlightColor);
                    TenoriToolApi.ProcessReturn processReturn;
                    if (!(processReturn = await TenoriToolApi.ProcessIndividualExtract(options.ListPath, options.OutputDirectory, options.Use32Mode, options.Verbose,"", inpath, callbacks, options.ExtractStub)).Success)
                        ioerror = processReturn.Error;
                }
            }
            catch (IOException ex) {
                ioerror = ex.Message;
            }
            catch (UnauthorizedAccessException ex) {
                ioerror = ex.Message;
            }
            catch (Exception ex) {
                ReportError($"{inpath}: unexpected exception occured ({ex.Message})");
                throw;
            }

            if (ioerror.Length <= 0) continue;
            ReportError($"cannot open {inpath}: {ioerror}");
            hasError = true;
        }

        return !hasError;
    }

    private static Task<string> DummyExtractEntry(string outputDirectory, bool verbose, string baseSubdirectory, Stream inputStream, ArchiveEntryInfo entryinfo, Action<string> processingFilepath, Action<string> processedFilepath) => Task.FromResult("");
}