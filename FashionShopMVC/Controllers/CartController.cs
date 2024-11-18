using FashionShopMVC.Models.DTO.CartDTO;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.OrderDTO;
using System.Web.Helpers;
using System.Linq;
using FashionShopMVC.Models.DTO.ProductDTO;
namespace FashionShopMVC.Controllers
{


    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;

        public CartController (IProductRepository productRepository)
        {
                _productRepository = productRepository;
        }
            public async Task<IActionResult> IndexAsync()
            {
             
           
            string jsonCart = HttpContext.Session.GetString("Cartlist");
           


            if (string.IsNullOrEmpty(jsonCart))
            {
                return View();
            }
            
            // Deserialize chuỗi JSON thành danh sách CartItem
            List<Product> cartItems = JsonConvert.DeserializeObject<List<Product>>(jsonCart);
            var CartINtel = await _productRepository.GetListById(cartItems);

            return View(CartINtel);  // Truyền danh sách giỏ hàng vào view
           
            
            }
        
        
        [HttpPost]
        public JsonResult DeleteSelectedProducts(string selectedIds)
        {
            // Kiểm tra nếu selectedIds không rỗng
            if (string.IsNullOrEmpty(selectedIds))
            {
                return Json(new { success = false, message = "Không có sản phẩm được chọn" });
            }

            // Chuyển chuỗi các ID thành danh sách số nguyên
            var ids = selectedIds.Split(',').Select(id => int.Parse(id)).ToList();

            try
            {
                string jsonCart = HttpContext.Session.GetString("Cartlist");
                List<Product> cartItems = JsonConvert.DeserializeObject<List<Product>>(jsonCart);
                cartItems =cartItems.Where(p => !ids.Contains(p.ID)).ToList();

                
                String json = JsonConvert.SerializeObject(cartItems);
                if (json == "[]")
                {
                    json = "";
                    HttpContext.Session.SetString("Cartlist", json);
                }
                else HttpContext.Session.SetString("Cartlist", json);
                // Trả về kết quả thành công
                IndexAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
               
                // Nếu có lỗi xảy ra
                return Json(new { success = false, message = ex.Message });
            }
        }
    
    [HttpPost]
        public IActionResult AddToCart(int id,int quantaty)
        {
            Session_build(id,quantaty);
            
            return RedirectToAction("Index");
        }

        public bool creat_order(List<OrderDetail> detailorder)
        {
            Order neworder = new Order();
            neworder.ID = 0;
            neworder.FullName = "nguyen phu thinh";
            neworder.PhoneNumber = "123456789";
            neworder.Email = "thinhnguyen25292425@gmail.com";
            neworder.ProvinceID = 5;
            neworder.Province = null;
            neworder.DistrictID = 10;
            neworder.District = null;
            neworder.WardID = 7;
            neworder.Ward = null;
            neworder.Address = "test";
            neworder.Note = "test order";
            neworder.OrderDate = DateTime.Now;
            neworder.DeliveryFee = 0;
            neworder.Status = 1;
            neworder.UserID = "1";
            User purchase_user = new User();
            neworder.User = purchase_user;
            neworder.VoucherID = null;
            neworder.Voucher = null;
            neworder.TypePayment = 1;
            neworder.OrderDetails = detailorder;
            return true;

        }

        public void get_detailorder ()
        {

        }
        [HttpPost]
        
        public IActionResult CancelButton() 
            {
            TempData["Message"] = "Purchase cancel!";

            return RedirectToAction("Index");
            }
            
       public void Session_build (int id,int quantati)
        {
            Product newproduct = new Product();
            newproduct.ID = id;
            newproduct.Quantity = quantati;
            GetProductByIdDTO getproduct = new GetProductByIdDTO();
            string jsonCart = HttpContext.Session.GetString("Cartlist");
            if (string.IsNullOrEmpty(jsonCart))
            {



                List<Product> items = new List<Product>
                {

                };
                items.Add(newproduct);
                String json = JsonConvert.SerializeObject(items);

                HttpContext.Session.SetString("Cartlist", json);
            }

            else
            {
                List<Product> cartItems = JsonConvert.DeserializeObject<List<Product>>(jsonCart);

                if (cartItems.Any(ID => ID.ID != id))

                {
                    cartItems.Add(newproduct);
                    String json = JsonConvert.SerializeObject(cartItems);
                    HttpContext.Session.SetString("Cartlist", json);
                }
                    
            }
            

        }
        /*public CartController (ProductRepository productRepository)
        {
            _producrespo = productRepository;
        }
        public Product getproductByid(int id )
        {   Product product = new Product();
            _producrespo.GetId(id);
            
            return product;
        }
        */
        public void findproducbyID (Cart_item list)
        {
            
        }
        public String Session_GetINTEL()
        {
            string json = HttpContext.Session.GetString("Cartlist");
            if (!string.IsNullOrEmpty(json))
            {
                return json;
                /*List <Cart_item> items = JsonConvert.DeserializeObject<List<Cart_item>>(json);

                foreach (var item in items)
                {
                    Response.WriteAsync($"Product_ID :{item.Id}, Count: {item.Number} <br>");
                }*/

            }
            else
            {
               return null;
            }
            //var username = HttpContext.Session.GetString("Cartlist").ToString();
            

        }
         

      

        
    }
}
