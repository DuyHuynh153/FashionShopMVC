using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Route("Admin")]
    public class AdminHomeController : Controller
    {
        [Route("")]
        
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("Member")]
        public IActionResult Member()
        {
            return View();
        }
    }
}
