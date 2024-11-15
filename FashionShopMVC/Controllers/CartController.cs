using FashionShopMVC.Models.DTO.CartDTO;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Controllers
{
    public class CartController : Controller
    {
        [Authorize]
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
        //  private readonly PaypalClient _paypalclient;

        //public FashionShopContext db { get; }

        private readonly IVnpayRespository _vnpayrespository;

        public CartController(/*FashionShopContext context*/ IVnpayRespository vnpayrespository)
        {
            //_paypalclient = paypalclient;

            //  db = context;
            _vnpayrespository = vnpayrespository;
        }


        [HttpGet]
        public IActionResult Checkout(/*checkoutVM model*/ string payment = "Thanh toán VNPay")
        {
            if (true )
            {
                if (true)
                {
                    var vnPaymodel = new VnpayMentRequestModel
                    {
                        Amount = 10300,
                        CreatedDate = DateTime.Now,
                        Description = "Thanh toan don hang test",
                        fullname    = "phu thinh test",
                        OrderId     = new Random().Next(1000,10000),

                    };

                    return Redirect(_vnpayrespository.CreatPaymentUrl(HttpContext,vnPaymodel));
                }

            }

            return View();
        }
        [Authorize]
        public IActionResult PaymentFail()
        { return View(); }
        [Authorize]
        public IActionResult PaymentCallBack()
        {
            var respone = _vnpayrespository.PaymentExecute(Request.Query);
            if (respone == null || respone.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"VNPay error !: {respone.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }
            // lưu đơn hàng vào database.
            TempData["Messgae"] = $"VNpay successfully";
            return RedirectToAction("PaymentSuccess");
        }



    }
}
