using Microsoft.AspNetCore.SignalR;

namespace bnAPI.Hubs;

public class LobbyHub : Hub
{
    public List<string> FoodList { get; set; }
    private readonly Random _random = new Random();
    public LobbyHub()
    {
        FoodList = new List<string>
        {
            "Apple", "Mango", "Olive", "Lemon", "Peach", 
            "Berry", "Chard", "Dates", "Grape", "Melon", 
            "Guava", "Onion", "Chili", "Sushi", "Bread", 
            "Pasta", "Lychee", "Bagel", "Bacon", "Trout", 
            "Steak", "Fries", "Herbs", "Honey", "Kiwi", 
            "Prune", "Squid", "Tofu", "Wheat", "Basil", 
            "Curry", "Thyme", "Beans", "Cream", "Flax", 
            "Jelly", "Pizza", "Salad", "Rice", "Maize", 
            "Pears", "Plums", "Cocoa", "Limes", "Nuts", 
            "Seeds", "Chips", "Salsa", "Cakes", "Mints", 
            "Wafel", "Broth", "Stews", "Soups", "Syrup", 
            "Tarts", "Rolls", "Romesco", "Tapas", "Kabob", 
            "Naans", "Tacos", "Nacho", "Queso", "Vegan", 
            "Meats", "Fruit", "Spice", "Scone", "Latte", 
            "Juice", "Drink", "Water", "Wines", "Beers", 
            "Ale", "Cider", "Pesto", "Sauce", "Cheese"
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
        var index = _random.Next(foods.Count);
        return foods[index];
    }
    
}