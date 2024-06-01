using NetPad.Apps.App.Common.UiInterop;

namespace NetPad.Apps.App.Common.CQs;

public class ConfirmWithUserCommand : Command<YesNoCancel>
{
    public ConfirmWithUserCommand(string message)
    {
        Message = message;
    }

    public string Message { get; }
}
