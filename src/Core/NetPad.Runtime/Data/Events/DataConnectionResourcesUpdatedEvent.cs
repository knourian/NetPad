using System.Text.Json.Serialization;
using NetPad.Events;

namespace NetPad.Data.Events;

public class DataConnectionResourcesUpdatedEvent : IEvent
{
    public DataConnectionResourcesUpdatedEvent(DataConnection dataConnection, DataConnectionResources resources,
        DataConnectionResourceComponent updatedComponent)
    {
        DataConnection = dataConnection;
        Resources = resources;
        UpdatedComponent = updatedComponent;
    }

    public DataConnection DataConnection { get; }

    [JsonIgnore] public DataConnectionResources Resources { get; }

    public DataConnectionResourceComponent UpdatedComponent { get; }
}
