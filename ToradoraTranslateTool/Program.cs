using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ToradoraTranslateTool;

static class Program
{
    /// <summary>
    /// Entry point for program
    /// </summary>
    [STAThread]
    static void Main()
    {
        AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += MyResolve;
        AppDomain.CurrentDomain.AssemblyResolve += MyResolve;

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FormMain());
    }

    private static Assembly MyResolve(Object sender, ResolveEventArgs e)
    {
        string assemblyPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Bin" ,new AssemblyName(e.Name).Name + ".dll");
        return !File.Exists(assemblyPath) ? null : Assembly.LoadFrom(assemblyPath);
    }
}