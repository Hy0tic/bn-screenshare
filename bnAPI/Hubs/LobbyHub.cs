using Microsoft.AspNetCore.SignalR;

namespace bnAPI.Hubs;

public class LobbyHub : Hub
{
    public async Task CreateLobby()
    {
        var lobbyId = Guid.NewGuid().ToString().Substring(0, 5);
        var groupName = $"Lobby-{lobbyId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync("JoinedGroup", lobbyId);
    }
    
    public async Task JoinLobby(string lobbyId)
    {
        var lobbyName = $"Lobby-{lobbyId}";
        await Clients.Group(lobbyName).SendAsync("MemberJoined", Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyName);
        await Clients.Caller.SendAsync("JoinedGroup", lobbyId);
    }

    public async Task LeaveLobby(string lobbyId)
    {
        var groupName = $"Lobby-{lobbyId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("CallerLeft", $"{Context.ConnectionId} has left the group {groupName}.");
    }

    public async Task SendOffer(string text, string uid)
    {
        await Clients.Client(uid).SendAsync("ReceivingOffer", text, Context.ConnectionId);
    }

    public async Task SendOfferToLobby(string lobbyId, string text)
    {
        var lobbyName = $"Lobby-{lobbyId}";
        await Clients.OthersInGroup(lobbyName).SendAsync("ReceivingOffer", text,Context.ConnectionId);
    }

    public async Task SendMessage(string username, string message, string lobbyId)
    {
        var groupName = $"Lobby-{lobbyId}";
        await Clients.Group(groupName).SendAsync("ReceiveMessage", username,message);
    }
    
}