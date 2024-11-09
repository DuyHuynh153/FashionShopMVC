using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
//using Blogger_Common;
using FashionShopMVC.Helper;
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.OrderDTO;
using FashionShopMVC.Models.DTO.UserDTO;
using FashionShopMVC.Models.ViewModel;
using FashionShopMVC.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NETCore.MailKit.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Policy;
using System.Xml.Linq;

namespace FashionShopMVC.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly INotyfService _notyfService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShoppingCartController(IProductRepository productRepository, IVoucherRepository voucherRepository, IOrderRepository orderRepository, INotyfService notyfService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _productRepository = productRepository;
            _voucherRepository = voucherRepository;
            _orderRepository = orderRepository;
            _notyfService = notyfService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            var cartSession = HttpContext.Session.GetString(CommonConstants.SessionCart);

            if (cartSession == null)
            {
                var emptyCart = new List<ShoppingCartViewModel>();

                HttpContext.Session.SetString(CommonConstants.SessionCart, JsonConvert.SerializeObject(emptyCart));

                cartSession = HttpContext.Session.GetString(CommonConstants.SessionCart);
            };

            return View();
        }

        public JsonResult GetAll()
        {
            var cartSession = HttpContext.Session.GetString(CommonConstants.SessionCart);

            if (cartSession == null)
            {
                var emptyCart = new List<ShoppingCartViewModel>();

                HttpContext.Session.SetString(CommonConstants.SessionCart, JsonConvert.SerializeObject(emptyCart));

                cartSession = HttpContext.Session.GetString(CommonConstants.SessionCart);
            };

            var cart = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(cartSession);

            return Json(new
            {
                data = cart,
                status = true,
            });
        }

        [HttpPost]
        public async Task<JsonResult> Add(int productID, int quantity)
        {
            var cartSession = HttpContext.Session.GetString(CommonConstants.SessionCart);

            if (cartSession == null)
            {
                var emptyCart = new List<ShoppingCartViewModel>();

                HttpContext.Session.SetString(CommonConstants.SessionCart, JsonConvert.SerializeObject(emptyCart));
            };

            var cart = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(cartSession);

            if (cart.Any(c => c.ProductID == productID))
            {
                foreach (var item in cart)
                {
                    if (item.ProductID == productID)
                    {
                        item.Quantity += quantity;
                    }
                }
            }
            else
            {
                var newItem = new ShoppingCartViewModel();
                newItem.ProductID = productID;
                newItem.Quantity = quantity;
                newItem.Product = _productRepository.GetId(productID);

                cart.Add(newItem);
            }

            HttpContext.Session.SetString(CommonConstants.SessionCart, JsonConvert.SerializeObject(cart));

            var product = await _productRepository.GetById(productID);
            _notyfService.Custom("<img style='height: 40px; padding-right: 10px;' src='/" + product.Image + "'/> Đã thêm vào giỏ hàng", 2, "white");

            return Json(new
            {
                status = true
            });
        }

        [HttpPost]
        public JsonResult Update(string cartData)
        {
            var cartViewModel = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(cartData);

            var cartSession = HttpContext.Session.GetString(CommonConstants.SessionCart);

            var cart = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(cartSession);

            foreach (var item in cart)
            {
                foreach (var item2 in cartViewModel)
                {
                    if (item.ProductID == item2.ProductID)
                    {
                        item.Quantity = item2.Quantity;
                    };
                };
            };

            HttpContext.Session.SetString(CommonConstants.SessionCart, JsonConvert.SerializeObject(cart));


            return Json(new
            {
                status = true,
            });
        }


        [HttpPost]
        public JsonResult Delete(int productID)
        {
            var cartSession = HttpContext.Session.GetString(CommonConstants.SessionCart);

            if (cartSession != null)
            {
                var cart = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(cartSession);

                cart.RemoveAll(c => c.ProductID == productID);

                HttpContext.Session.SetString(CommonConstants.SessionCart, JsonConvert.SerializeObject(cart));

                _notyfService.Success("Đã xóa khỏi giỏ hàng", 2);
                return Json(new
                {
                    status = true,
                });
            };

            return Json(new
            {
                status = false,
            });
        }

        [HttpPost]
        public JsonResult DeleteAll()
        {
            var emptyCart = new List<ShoppingCartViewModel>();

            HttpContext.Session.SetString(CommonConstants.SessionCart, JsonConvert.SerializeObject(emptyCart));

            return Json(new
            {
                status = true,
            });
        }

        public IActionResult CheckOut()
        {
            string userSession = HttpContext.Session.GetString(CommonConstants.SessionUser);

            if (userSession == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var user = JsonConvert.DeserializeObject<User>(userSession);

                return View(user);
            }
        }
    }
}
