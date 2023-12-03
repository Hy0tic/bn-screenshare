using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using BnScreenshareAPI;

namespace bnAPI.Controllers;

public class Health : Controller
{
    
    [HttpGet("/getVersion")]
    public IActionResult Version()
    {
        const string filePath = "buildinfo.json";
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }
        
        var jsonString = System.IO.File.ReadAllText(filePath);
        var jsonData = JsonSerializer.Deserialize<Dictionary<string, BuildInfo>>(jsonString);

        var buildInfo = jsonData["buildInfo"];

        return Ok(buildInfo);
    }
}