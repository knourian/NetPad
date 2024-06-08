using System.Reflection;
using NetPad.ExecutionModel.External.Interface;

/// <summary>
/// Meant to be injected into script code so it can initialize <see cref="ScriptProcessIO"/>.
/// The class name must be "Program" and must be partial. This is so we augment the base "Program" class
/// .NET will implicitly wrap top-level statements within. Code in the constructor will be called by the runtime
/// before a script's code is executed.
///
/// This is embedded into the assembly to be read later as an Embedded Resource.
/// </summary>
public partial class Program
{
    public static readonly UserScript UserScript = new(new Guid("SCRIPT_ID"), "SCRIPT_NAME", "SCRIPT_LOCATION");

    static Program()
    {
        var args = Environment.GetCommandLineArgs();

        if (args.Contains("-help"))
        {
            ScriptProcessIO.UseConsoleOutput(true);
            var currentAssemblyPath = Assembly.GetExecutingAssembly().Location;
            if (Environment.CurrentDirectory.Length > 1)
            {
                currentAssemblyPath = "." + currentAssemblyPath.Replace(Environment.CurrentDirectory, string.Empty);
            }

            Console.WriteLine($"{UserScript.Name}");
            Console.WriteLine($@"
Usage:
    dotnet {currentAssemblyPath} [-console|-text|-html] [OPTIONS]

Output Format:
    -console    Optimized for console output (default)
    -text       Text output
    -html       HTML output

Options:
    -no-color   Do not color output. Does not apply to ""HTML"" format
    -help       Display this help
");

            Environment.Exit(0);
        }

        if (args.Contains("-html"))
        {
            ScriptProcessIO.UseHtmlOutput();
        }
        else
        {
            bool useConsoleColors = !args.Contains("-no-color");

            if (args.Contains("-text"))
            {
                ScriptProcessIO.UseTextOutput(useConsoleColors);
            }
            else
            {
                ScriptProcessIO.UseConsoleOutput(useConsoleColors);
            }
        }
    }
}

