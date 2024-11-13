using FashionShopMVC.Models.DTO.CartDTO;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace FashionShopMVC.Controllers
{
    public class VNPayController : Controller
    {

        // public FashionShopContext db { get; }


        // private readonly ProductRepository _producrespo;
        private readonly IVnpayRespository _vnpayrespository;
        public VNPayController(IVnpayRespository vnpayrespository)
        {
            //_paypalclient = paypalclient;

            //  db = context;
            _vnpayrespository = vnpayrespository;
        }


        [HttpGet]
        public IActionResult CheckoutVNpay(string payment = "Thanh toán VNPay")
        {
            if (true)
            {
                if (true)
                {
                    var vnPaymodel = new VnpayMentRequestModel
                    { // lay detail tu thanh toan

                        Amount = 10300,
                        CreatedDate = DateTime.Now,
                        Description = "Thanh toan don hang test",
                        fullname = "phu thinh test",
                        OrderId = new Random().Next(1000, 10000),

                    };

                    return Redirect(_vnpayrespository.CreatPaymentUrl(HttpContext, vnPaymodel));
                }

            }

            return View();
        }
        
        public IActionResult PaymentFail()
        { return View(); }
        
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
