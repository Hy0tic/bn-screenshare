using bnAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace bn_screenshare.tests.ApiTests;

public class LobbyHubTests
{
    public LobbyHub lobbyHub { get; set; }

    public LobbyHubTests()
    {
        lobbyHub = new LobbyHub();
    }

    [Fact]
    public void GetLobbyName_Return_String()
    {
        var x = lobbyHub.GetLobbyName("");
        Assert.IsType<string>(x);
    }

    [Fact]
    public void GetLobbyName_NotNull()
    {
        var x = lobbyHub.GetLobbyName("");
        Assert.NotNull(x);
    }

}