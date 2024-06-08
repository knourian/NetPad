using NetPad.Events;
using NetPad.Presentation;

namespace NetPad.Scripts.Events;

public class ScriptOutputEmittedEvent : IEvent
{
    public ScriptOutputEmittedEvent(Guid scriptId, ScriptOutput output)
    {
        ScriptId = scriptId;
        Output = output;
        OutputType = output.GetType().Name;
    }

    public Guid ScriptId { get; }
    public ScriptOutput Output { get; }
    public string OutputType { get; }
}
