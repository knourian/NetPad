namespace NetPad.Apps.App.Common.UiInterop;

public class IpcMessageBatch
{
    public IpcMessageBatch(IList<IpcMessage> messages)
    {
        Messages = messages;
    }

    public IList<IpcMessage> Messages { get; }
}
