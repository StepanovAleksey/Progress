using Microsoft.AspNetCore.Mvc;
using SignalRChat.Models;

namespace SignalRChat.Controllers
{
    public class HomeController : Controller
    {
         public IActionResult Index()
        {
            return View();
        }

    }
}
