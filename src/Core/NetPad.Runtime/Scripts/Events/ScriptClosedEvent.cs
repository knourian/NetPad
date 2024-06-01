using NetPad.Events;

namespace NetPad.Scripts.Events;

public class ScriptClosedEvent : IEvent
{
    public ScriptClosedEvent(Script script)
    {
        Script = script;
    }

    public Script Script { get; }
}
