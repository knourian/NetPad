using NetPad.Events;

namespace NetPad.Data.Events;

public class DataConnectionSavedEvent : IEvent
{
    public DataConnectionSavedEvent(DataConnection dataConnection)
    {
        DataConnection = dataConnection;
    }

    public DataConnection DataConnection { get; }
}
