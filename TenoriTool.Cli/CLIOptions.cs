#region Copyright (C) 2008 Giacomo Stelluti Scala
//
// Copy Directory Tree: CLIOptions.cs
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CommandLine;
using CommandLine.Text;
using TenoriTool.Cli.Properties;
using static tenoriTool.TenoriToolApi;

namespace TenoriTool.Cli;

internal static partial class TenoriTool
{
    sealed class Options
    {
        [ValueList(typeof(List<string>))]
        public IList<string> Paths = null;

        #region command-line options
        [Option("v", "verbose",
            HelpText = "Explain what is being done.")]
        public bool Verbose = false;

        [Option("x", null,
            HelpText = "Extract files from archive. See -a option.")]
        public bool UseExtractMode = false;

//!#            [Option("a", null,
//!#                    HelpText = "Add files to archive. Requires -l or --list argument to be specified.")]
        //public bool UsePackMode = false;

        [Option("d", "output-directory",
            HelpText = "Specify output directory (otherwise same as input parameter).")]
        public string OutputDirectory = string.Empty;

        [Option("l", "list",
            HelpText = "Specify path for list of archive contents. If this option is omitted when extracting, list is echoed on standard output. If this option is a blank string when extracting, list is silently discarded. If this option is omitted when packing, list is read on standard input.")]
        public string ListPath = "-";

        [Option("4", "32bit-mode",
            HelpText = "Interpret file offsets as 32-bit integers instead of 64-bit. Ignored when packing.")]
        public bool Use32Mode = false;

//!#            [Option("r", "recurse",
//!#                   HelpText = "Perform task on extracted files, creating subdirectories. Ignored when packing.")]
        //public bool UseRecurseMode = false;
        #endregion

        private ExtractDelegate _ExtractStub = DummyExtractEntry;
        public ExtractDelegate ExtractStub { get { return _ExtractStub; } set { _ExtractStub = value; } }

        // This attributed method is used for handling the help option
        [HelpOption("?", "help",
            HelpText = "Display help and exit.")]
        public string GetProgramHeader(bool useShortSummary)
        {
            HelpText info = new(Heading);
            info.Copyright = ThisAssembly.Copyright;
            info.AddPreOptionsLine("This is free software. You may redistribute copies of it under the terms of");
            info.AddPreOptionsLine("the MIT License <http://www.opensource.org/licenses/mit-license.php>.");
            info.AddPreOptionsLine("  - Based on Giacomo Stelluti Scala's cptree program, Version 1.1 (http://cptree.codeplex.com/).");
            info.AddPreOptionsLine("  - Includes Giacomo Stelluti Scala's Command Line Library, Version 1.5 (http://commandline.codeplex.com/).");
            info.AddPreOptionsLine("  - Includes a code fragment from Banshee, Version 1.4.1 (http://banshee-project.org/)." +
                                   Environment.NewLine);
            info.AddPreOptionsLine($"Usage: {ThisAssembly.Name} [OPTION]... [FILE]...");
            info.AddPreOptionsLine($"       {ThisAssembly.Name} -x [OPTION]... [FILE]...");
//!#                info.AddPreOptionsLine(String.Format("       {0} -a -l <list_path> [OPTION]... [FILE]...", ThisAssembly.Name) +
//!#                    Environment.NewLine);
            if ( useShortSummary == false )
            {
                info.AddOptions(this);
            }
            else
            {
                MethodInfo methodInfo = typeof(Options).GetMethod("GetProgramHeader");
                foreach ( Attribute attr in methodInfo.GetCustomAttributes(true) )
                {
                    HelpOptionAttribute HelpAttr = attr as HelpOptionAttribute;
                    if (null != HelpAttr)
                    {
                        string Switch = string.Empty;
                        if ( HelpAttr.HasLongName )
                        {
                            Switch = "--" + HelpAttr.LongName;
                        }
                        if ( HelpAttr.HasShortName )
                        {
                            Switch = "-" + HelpAttr.ShortName;
                        }
                        if ( Switch.Length > 0 )
                        {
                            info.AddPreOptionsLine($"Type {ThisAssembly.Name} {Switch} to obtain online help." +
                                                   Environment.NewLine);
                        }
                    }
                }
            }
            return info.ToString();
        }



        #region Extra validation stuff
        public bool Validate()
        {
            #region General validation
            /*if ( UseExtractMode && UsePackMode )
            {
                ReportError("cannot specify both extract and pack modes!");
                return false;
            }*/

            if ( OutputDirectory.Length > 0 && IsInvalidPath(OutputDirectory) )
            {
                ReportError("specified output directory is invalid");
                return false;
            }
            #endregion

            #region Extract or "Listing" mode validation
            if ( UseExtractMode )
            {
                if ( ListPath.Length > 0 && ListPath != "-" && Directory.Exists(ListPath) )
                {
                    ReportError("file list output path cannot be a directory");
                    return false;
                }
                if ( Paths.Count < 1 )
                {
                    ReportError("no file specified");
                    return false;
                }
                if ( Paths.Contains("-") )
                {
                    ReportError("STDIN input is not supported in this version");
                    return false;
                }
            }
            #endregion

//                    if ( this.Paths.Count > 1 )
//                    {
//                    }

/*                if ( this.UsePackMode )
                {
                    if ( this.ListPath.Length == 0 )
                    {
                        ReportError("cannot use STDIN as input when packing");
                        return false;
                    }
                    if ( this.Paths.Count > 1 )
                    {
                        ReportError("can only pack a file at a time");
                        return false;
                    }
                }
*/
            #if MYFOO
                    if (!this.SourcePath.Equals(Environment.CurrentDirectory, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (File.Exists(this.SourcePath))
                        {
                            ReportError(string.Format("'{0}' is a file", this.SourcePath));
                            return false;
                        }
                        if (!Directory.Exists(this.SourcePath))
                        {
                            ReportError(string.Format("'{0}' does not exist", this.SourcePath));
                            return false;
                        }
                    }
                    if (this.TargetPath != null)
                    {
                        if (File.Exists(this.TargetPath))
                        {
                            ReportError(string.Format("'{0}' is a file", this.TargetPath));
                            return false;
                        }
                    }
            #endif
            return true;
        }

        private static bool IsInvalidPath(string path)
        {
            Regex re = new("^(:|[^a-z]:)|(:.*:)|..:|[" + Regex.Escape(string.Join("", Array.ConvertAll(Path.GetInvalidPathChars(), Convert.ToString)) + "*?") + "]", RegexOptions.IgnoreCase);
            return re.IsMatch(path);
        }
        #endregion
    }


    private static void ReportError(string message)
    {
        StringBuilder builder = new(message.Length * 2);
        builder.Append(ThisAssembly.Name);
        builder.Append(": ");
        builder.Append(message);
        Console.Error.WriteLine(builder.ToString());
    }
}