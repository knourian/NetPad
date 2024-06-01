namespace NetPad.Apps.App.Common.CQs;

public class AlertUserCommand : Command
{
    public AlertUserCommand(string message)
    {
        Message = message;
    }

    public string Message { get; }
}
