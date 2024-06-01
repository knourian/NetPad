using NetPad.Events;

namespace NetPad.Data.Events;

public class DataConnectionDeletedEvent : IEvent
{
    public DataConnectionDeletedEvent(DataConnection dataConnection)
    {
        DataConnection = dataConnection;
    }

    public DataConnection DataConnection { get; }
}
