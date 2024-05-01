using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NetPad.Presentation.Html;
using NetPad.Utilities;
using O2Html.Dom;

namespace NetPad;

public static class Util
{
    static Util()
    {
        ScriptStopwatch = new Stopwatch();
    }

    public static string[] StartArgs { get; private set; } = null!;
    public static UserScript CurrentScript { get; private set; } = null!;
    public static Stopwatch ScriptStopwatch { get; }
    public static TimeSpan ElapsedTime => ScriptStopwatch.Elapsed;
    public static Process CurrentProcess => Process.GetCurrentProcess();

    //public static int HostProcessID { get; internal set; }
    //public static string MyQueriesFolder =>
    //public static Script ActualScript =>
    //public static string CurrentCxString =>
    //public static string CurrentDataContext =>
    //public static System.Transactions.IsolationLevel? TransactionIsolationLevel { get; set; }
    //public static void ClearResults()
    //public static ExpandoObject ToExpando(object value, IEnumerable<MemberInfo> include)
    //public static Try

    public static Version CurrentRuntime => Environment.Version;
    public static OperatingSystem OsVersion => Environment.OSVersion;
    public static string RuntimeIdentifier => RuntimeInformation.RuntimeIdentifier;
    public static string FrameworkDescription => RuntimeInformation.FrameworkDescription;
    public static string OSDescription => RuntimeInformation.OSDescription;
    public static Architecture ProcessArchitecture => RuntimeInformation.ProcessArchitecture;
    public static Architecture OSArchitecture => RuntimeInformation.OSArchitecture;

    public static void Init(
        Guid currentScriptId,
        string currentScriptName,
        string currentScriptFilePath,
        string[] args
    )
    {
        StartArgs = args;

        CurrentScript = new UserScript(
            currentScriptId,
            currentScriptName,
            string.IsNullOrWhiteSpace(currentScriptFilePath) ? null : currentScriptFilePath
        );
    }

    public static void OpenBrowser(string url) => ProcessUtil.OpenWithDefaultApp(url);
    public static void OpenFolder(string fullPath) => ProcessUtil.OpenWithDefaultApp(fullPath);
    public static void OpenFile(string fullPath) => ProcessUtil.OpenWithDefaultApp(fullPath);
    public static void OpenWithDefaultApp(string path) => ProcessUtil.OpenWithDefaultApp(path);

    public static string ToHtmlString(object objectToSerialize)
    {
        return HtmlPresenter.Serialize(objectToSerialize);
    }

    public static Element ToHtmlElement(object objectToSerialize)
    {
        return HtmlPresenter.SerializeToElement(objectToSerialize);
    }
}
