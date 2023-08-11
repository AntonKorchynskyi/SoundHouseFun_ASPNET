using Microsoft.AspNetCore.Mvc;

namespace SoundHouseFun.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
