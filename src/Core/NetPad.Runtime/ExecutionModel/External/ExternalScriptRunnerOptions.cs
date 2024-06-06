namespace NetPad.ExecutionModel.External;

public class ExternalScriptRunnerOptions
{
    public ExternalScriptRunnerOptions(string[] processCliArgs, bool redirectIo)
    {
        ProcessCliArgs = processCliArgs;
        RedirectIo = redirectIo;
    }

    public string[] ProcessCliArgs { get; set; }
    public bool RedirectIo { get; set; }
}
