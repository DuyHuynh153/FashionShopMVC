using FashionShopMVC.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FashionShopMVC.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index ()
        {
            string jsonCart = HttpContext.Session.GetString("Cartlist");
            if (string.IsNullOrEmpty(jsonCart))
            {
                return View();
            }

            // Deserialize chuỗi JSON thành danh sách CartItem
            List<Product> cartItems = JsonConvert.DeserializeObject<List<Product>>(jsonCart);
            var total = 0;
            foreach (var item in cartItems)
            {
                total = (int)(total + item.Price * (100 - item.Discount));
            }
            TempData["Message"] = total;

            return View();
        }
    }
}
