namespace NetPad.Scripts.Events;

public class ScriptCreatedEvent : IScriptEvent
{
    public ScriptCreatedEvent(Script script)
    {
        Script = script;
    }

    public Script Script { get; }
    public Guid ScriptId => Script.Id;
}
