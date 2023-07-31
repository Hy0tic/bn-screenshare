using Microsoft.AspNetCore.Mvc;

namespace bnAPI.Controllers;

public class LobbyController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}