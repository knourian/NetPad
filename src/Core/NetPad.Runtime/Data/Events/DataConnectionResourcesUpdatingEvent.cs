using NetPad.Events;

namespace NetPad.Data.Events;

public class DataConnectionResourcesUpdatingEvent : IEvent
{
    public DataConnectionResourcesUpdatingEvent(DataConnection dataConnection, DataConnectionResourceComponent updatingComponent)
    {
        DataConnection = dataConnection;
        UpdatingComponent = updatingComponent;
    }

    public DataConnection DataConnection { get; }
    public DataConnectionResourceComponent UpdatingComponent { get; }
}
