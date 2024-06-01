using NetPad.Events;

namespace NetPad.Data.Events;

public class DataConnectionSchemaValidationStartedEvent : IEvent
{
    public DataConnectionSchemaValidationStartedEvent(Guid dataConnectionId)
    {
        DataConnectionId = dataConnectionId;
    }

    public Guid DataConnectionId { get; }
}
