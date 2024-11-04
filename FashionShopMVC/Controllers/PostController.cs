using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
