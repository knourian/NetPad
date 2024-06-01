using NetPad.Events;

namespace NetPad.Scripts.Events;

public class ScriptDirectoryChangedEvent : IEvent
{
    public ScriptDirectoryChangedEvent(IEnumerable<ScriptSummary> scripts)
    {
        Scripts = scripts;
    }

    public IEnumerable<ScriptSummary> Scripts { get; }
}
