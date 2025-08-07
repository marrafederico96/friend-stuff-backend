using Microsoft.AspNetCore.Mvc;

namespace FriendStuffBackend.Features.UserEvent;

public class EventController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}