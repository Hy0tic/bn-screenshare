using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using BnScreenshareAPI;

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