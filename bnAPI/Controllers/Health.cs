using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using BnScreenshareAPI;

namespace bnAPI.Controllers;

public sealed class Health : Controller
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

        if(jsonData== null)
        {
            return BadRequest("build info not found");
        }
        var buildInfo = jsonData["buildInfo"];

        return Ok(buildInfo);
    }
}