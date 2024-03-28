using Microsoft.AspNetCore.Mvc;

namespace bnAPI.Controllers;

[ApiController]
public sealed class LobbyController : Controller
{
    [HttpGet("/hello")]
    public IActionResult Hello()
    {
        return Ok("Hello This Is BnScreenshareAPI");
    }
    
}