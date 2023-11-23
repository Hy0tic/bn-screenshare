using bnAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace bn_screenshare.tests;

public class ApiTests
{

    [Fact]
    public void Test1()
    {
        var lobbyController = new LobbyController();

        var result = lobbyController.Hello();

        Assert.IsType<OkObjectResult>(result);

    }
}