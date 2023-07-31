using bnAPI.Models;
using Microsoft.AspNetCore.SignalR;

namespace bnAPI.Hubs;

public class LobbyHub : Hub
{
    public async Task CreateLobby()
    {
        var lobbyId = Guid.NewGuid();
        var groupName = $"Lobby-{lobbyId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync("JoinedGroup", lobbyId);
    }
    
    public async Task JoinLobby(string lobbyId)
    {
        var lobbyName = $"Lobby-{lobbyId}";
        await Clients.Caller.SendAsync("JoinedGroup", lobbyId);
        await Clients.Group(lobbyName).SendAsync("MemberJoined", Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyName);
    }

    public async Task LeaveLobby(string lobbyId)
    {
        var groupName = $"Lobby-{lobbyId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("CallerLeft", $"{Context.ConnectionId} has left the group {groupName}.");
    }
    
    public async Task StartScreenSharing(ScreenSharingData data)
    {
        var frame = data.Frame;  // this is the frame received from the client
        var lobbyId = data.LobbyId;  // this is the lobby id received from the client
        Console.WriteLine(data.Frame);

        var groupName = $"Lobby-{lobbyId}";

        // now send the frame to all clients in the specified lobby
        await Clients.Group(groupName).SendAsync("ReceiveScreenShare", frame);
    }
    

    public async Task SendOffer(string text, string uid)
    {
        await Clients.Client(uid).SendAsync("ReceivingOffer", text, Context.ConnectionId);
    }
    
}