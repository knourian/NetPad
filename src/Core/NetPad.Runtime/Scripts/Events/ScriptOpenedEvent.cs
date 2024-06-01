using NetPad.Events;

namespace NetPad.Scripts.Events;

public class ScriptOpenedEvent : IEvent
{
    public ScriptOpenedEvent(Script script)
    {
        Script = script;
    }

    public Script Script { get; }
}
