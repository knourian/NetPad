using NetPad.Events;

namespace NetPad.Sessions.Events;

public class ActiveEnvironmentChangedEvent : IEvent
{
    public ActiveEnvironmentChangedEvent(Guid? scriptId)
    {
        ScriptId = scriptId;
    }

    public Guid? ScriptId { get; }
}
