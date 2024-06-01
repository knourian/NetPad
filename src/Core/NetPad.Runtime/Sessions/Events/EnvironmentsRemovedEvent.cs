using NetPad.Events;
using NetPad.Scripts;

namespace NetPad.Sessions.Events;

public class EnvironmentsRemovedEvent : IEvent
{
    public EnvironmentsRemovedEvent(params ScriptEnvironment[] environments)
    {
        Environments = environments;
    }

    public ScriptEnvironment[] Environments { get; }
}
