using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Route("Admin")]
    [Authorize]
    public class AdminHomeController : Controller
    {
        [Route("")]
        
        
        public IActionResult Index()
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_AdminHomePartial"); // AJAX request
            }
            return View();
        }

        
    }
}
