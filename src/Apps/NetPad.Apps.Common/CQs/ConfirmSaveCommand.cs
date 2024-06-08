using NetPad.Apps.UiInterop;
using NetPad.Scripts;

namespace NetPad.Apps.CQs;

public class ConfirmSaveCommand : Command<YesNoCancel>
{
    public ConfirmSaveCommand(Script script)
    {
        Message = $"You have unsaved changes. Do you want to save '{script.Name}'?";
    }

    public string Message { get; }
}
