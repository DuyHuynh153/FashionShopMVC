using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.OrderDTO;
using FashionShopMVC.Models.DTO.ProductDTO;
using FashionShopMVC.Models.ViewModel;
using FashionShopMVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

namespace FashionShopMVC.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IOrderRepository _orderRespository;

        public CheckoutController (IOrderRepository orderRepository)
        {
            _orderRespository = orderRepository;
        }

        public IActionResult Index ()
        {
           
            return View();
        }
        public IActionResult Success()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreateOrder(CreateOrderDTO model)
        {
            Success();
            if (ModelState.IsValid)
            {
                
                creatOrder(model);
               
                   
                
                // Process the order, save it to the database, etc.
                // Example: Save the order to the database

                // Return a success response
                return Json(new { success = true, message = "Order created successfully" });
            }
            else
            {
                // Return an error response if validation fails
                return Json(new { success = false, message = "There was an error processing your order." });
            }
        }
        public List<ShoppingCartViewModel> getdetail ()
        {
            List<ShoppingCartViewModel> detailcart = new List<ShoppingCartViewModel>();
            return detailcart;
        }
    public void creatOrder(CreateOrderDTO order)
        {  
            order.UserID = "001";
            order.shoppingCarts = getdetail();
            _orderRespository.Create(order);
        }
    }
}
