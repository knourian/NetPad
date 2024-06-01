using NetPad.Events;

namespace NetPad.Application.Events;

public class AppStatusMessagePublishedEvent : IEvent
{
    public AppStatusMessagePublishedEvent(AppStatusMessage message)
    {
        Message = message;
    }

    public AppStatusMessage Message { get; }
}
