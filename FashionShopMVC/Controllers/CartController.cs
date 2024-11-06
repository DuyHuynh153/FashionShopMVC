using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Controllers
{
    public class CartController : Controller
    {
        
            public IActionResult Index()
            {
                return View();
            }

            public IActionResult Buy_action()
            {
            return View();
            }
            public IActionResult Error() 
            { 
            return View();
            }
        
    }
}
