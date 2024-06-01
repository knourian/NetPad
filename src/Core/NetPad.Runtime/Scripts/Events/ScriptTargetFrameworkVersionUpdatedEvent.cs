using NetPad.DotNet;
using NetPad.Events;

namespace NetPad.Scripts.Events;

public class ScriptTargetFrameworkVersionUpdatedEvent : IEvent
{
    public ScriptTargetFrameworkVersionUpdatedEvent(Script script, DotNetFrameworkVersion oldVersion, DotNetFrameworkVersion newVersion)
    {
        Script = script;
        OldVersion = oldVersion;
        NewVersion = newVersion;
    }

    public Script Script { get; }
    public DotNetFrameworkVersion OldVersion { get; }
    public DotNetFrameworkVersion NewVersion { get; }
}
