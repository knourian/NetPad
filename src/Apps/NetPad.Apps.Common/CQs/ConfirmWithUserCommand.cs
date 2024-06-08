using NetPad.Apps.UiInterop;

namespace NetPad.Apps.CQs;

public class ConfirmWithUserCommand : Command<YesNoCancel>
{
    public ConfirmWithUserCommand(string message)
    {
        Message = message;
    }

    public string Message { get; }
}
