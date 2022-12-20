using Microsoft.AspNetCore.Mvc;

namespace OnlineShopApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
