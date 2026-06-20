using Microsoft.AspNetCore.SignalR;

namespace TorneoDeportivo.API.Hubs;

public class BracketHub : Hub
{
    public async Task JoinTorneo(string torneoId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"torneo-{torneoId}");
}
