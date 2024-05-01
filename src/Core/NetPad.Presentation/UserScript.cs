using System.Diagnostics;
using System.Reflection;

namespace NetPad;

/// <summary>
/// Info about current script.
/// </summary>
/// <param name="Id">The unique script Id.</param>
/// <param name="Name">The script name.</param>
/// <param name="FilePath">The full path of the script file. Empty if not a saved script.</param>
/// <param name="Arguments">The argument list that was passed to the script.</param>
public record UserScript(Guid Id, string Name, string? FilePath)
{
    public string? Location => FilePath == null ? null : Path.GetDirectoryName(FilePath);
    public string RunFilePath => Assembly.GetEntryAssembly()!.Location;
    public string RunLocation => Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!;
}
