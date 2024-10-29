using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.CartDTO;
using FashionShopMVC.Repositories.@interface;
using VNPAY_CS_ASPX;
// VnPayservice.cs
namespace FashionShopMVC.Repositories
{
    public class VnpayRepository : IVnpayRespository
    {
        private readonly IConfiguration _config;

        public VnpayRepository(IConfiguration config)
        {
            _config = config;
        }
        public string CreatPaymentUrl(HttpContext context, VnpayMentRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["Vnpay.Version"]);
            vnpay.AddRequestData("vnp_Command", _config["Vnpay.Conmmand"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["Vnpay.TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString()); 
            /*Số tiền thanh toán. Số tiền không 
            mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND
            (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần(khử phần thập phân), sau đó gửi sang VNPAY
            là: 10000000*/
               
            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["Vnpay.CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            //viet them ca quoc gia khac
            vnpay.AddRequestData("vnp_Locale", _config["Vnpay.Locate"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán cho đơn hàng :" + model.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", _config["Vnpay.PaymentBackReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef",tick);
            var paymentUrl = vnpay.CreateRequestUrl(_config["Vnpay:BaseUrl"], _config["Vnpay:HashSercret"]);
            return paymentUrl ;

        }

        public VnpaymentResponseModel PaymentExecute(IQueryCollection Collection)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in Collection)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key,value.ToString());
                }
            }

            var vnp_orderId          = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId    = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash       = Collection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode     = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OderInfo       = vnpay.GetResponseData("vnp_ResponseCode");
            bool checkSignature      = vnpay.ValidateSignature(vnp_SecureHash, _config["Vnpay:HashSercret"]);
            if (!checkSignature)
            {
                return new VnpaymentResponseModel
                {
                    Success = false,
                };
            }
            return new VnpaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OderInfo,
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode,
            };
        }
    }
}
