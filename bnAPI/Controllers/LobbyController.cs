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

    [HttpGet("/getVersion")]
    public IActionResult Version()
    {
        const string filePath = "buildinfo.json";
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }
        
        string jsonString = System.IO.File.ReadAllText(filePath);
        var jsonData = JsonSerializer.Deserialize<Dictionary<string, BuildInfo>>(jsonString);

        var buildInfo = jsonData["buildInfo"];

        return Ok(buildInfo);
    }

}