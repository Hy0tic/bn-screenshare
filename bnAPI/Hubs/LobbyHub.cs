using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography;

namespace bnAPI.Hubs;

public class LobbyHub : Hub
{
    private List<string> FoodList { get; set; }
    public LobbyHub()
    {
        FoodList = new List<string>
        {
            "apple", "mango", "olive", "lemon", "peach", 
            "berry", "chard", "dates", "grape", "melon", 
            "guava", "onion", "chili", "sushi", "bread", 
            "pasta", "lychee", "bagel", "bacon", "trout", 
            "steak", "fries", "herbs", "honey", "kiwi", 
            "prune", "squid", "tofu", "wheat", "basil", 
            "curry", "thyme", "beans", "cream", "patty", 
            "jelly", "pizza", "salad", "rices", "maize", 
            "pears", "plums", "cocoa", "limes", "yeast", 
            "seeds", "chips", "salsa", "cakes", "mints", 
            "wafer", "broth", "stews", "soups", "syrup", 
            "tarts", "rolls", "romesco", "tapas", "kabob", 
            "naans", "tacos", "nacho", "queso", "vegan", 
            "meat", "fruit", "spice", "scone", "latte", 
            "juice", "drink", "water", "wines", "beers", 
            "toast", "cider", "pesto", "sauce", "cheese"
        };
    }

    public async Task CreateLobby()
    {
        var lobbyId = PickRandomFood(FoodList);
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
    
    private string PickRandomFood(List<string> foods)
    {
        var index = RandomNumberGenerator.GetInt32(foods.Count);
        while (foods[index].Length != 5)
        {
            index = RandomNumberGenerator.GetInt32(foods.Count);
        }
        return foods[index];
    }
    
}