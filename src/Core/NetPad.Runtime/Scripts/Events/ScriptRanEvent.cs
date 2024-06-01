using NetPad.Events;

namespace NetPad.Scripts.Events;

public class ScriptRanEvent : IEvent
{
    public ScriptRanEvent(ScriptEnvironment scriptEnvironment)
    {
        ScriptEnvironment = scriptEnvironment;
    }

    public ScriptEnvironment ScriptEnvironment { get; }
}
