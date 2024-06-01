using NetPad.Data;
using NetPad.Events;

namespace NetPad.Scripts.Events;

public class ScriptDataConnectionChangedEvent : IEvent
{
    public ScriptDataConnectionChangedEvent(Script script, DataConnection? dataConnection)
    {
        Script = script;
        DataConnection = dataConnection;
    }

    public Script Script { get; }
    public DataConnection? DataConnection { get; }
}
