using bnAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace bn_screenshare.tests.ApiTests;

public class LobbyControllerTests
{

    [Fact]
    public void HelloEndpointReturnOk()
    {
        var lobbyController = new LobbyController();

        var result = lobbyController.Hello();

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(result);
    }
}