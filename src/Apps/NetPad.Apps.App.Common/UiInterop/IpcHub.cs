using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

namespace NetPad.Apps.App.Common.UiInterop;

/// <summary>
/// SignalR Hub. This includes methods that clients can invoke.
/// </summary>
public class IpcHub : Hub
{
    public void Respond(Guid messageId, JsonElement response)
    {
        IpcResponseQueue.ResponseReceived(messageId, response);
    }
}
