using NetPad.Events;

namespace NetPad.Scripts.Events;

public class ScriptRunCancelledEvent : IEvent
{
    public ScriptRunCancelledEvent(ScriptEnvironment scriptEnvironment)
    {
        ScriptEnvironment = scriptEnvironment;
    }

    public ScriptEnvironment ScriptEnvironment { get; }
}
