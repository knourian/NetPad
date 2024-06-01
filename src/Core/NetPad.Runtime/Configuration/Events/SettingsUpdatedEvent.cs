using NetPad.Events;

namespace NetPad.Configuration.Events;

public class SettingsUpdatedEvent : IEvent
{
    public SettingsUpdatedEvent(Settings settings)
    {
        Settings = settings;
    }

    public Settings Settings { get; }
}
