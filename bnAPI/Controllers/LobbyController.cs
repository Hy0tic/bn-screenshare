using Microsoft.AspNetCore.Mvc;

namespace bnAPI.Controllers;

[ApiController]
public class LobbyController : Controller
{
    [HttpGet("/hello")]
    public IActionResult Hello()
    {
        return Ok("Hello This Is BnScreenshareAPI");
    }

}