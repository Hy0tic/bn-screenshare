using bnAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace bn_screenshare.tests.ApiTests;

public class LobbyHubTests
{
    public LobbyHub LobbyHub { get; set; }

    public LobbyHubTests()
    {
        LobbyHub = new LobbyHub();
    }

    [Fact]
    public void GetLobbyName_Return_String()
    {
        var x = LobbyHub.GetLobbyName("");
        Assert.IsType<string>(x);
    }

    [Fact]
    public void GetLobbyName_NotNull()
    {
        var x = LobbyHub.GetLobbyName("");
        Assert.NotNull(x);
    }

}