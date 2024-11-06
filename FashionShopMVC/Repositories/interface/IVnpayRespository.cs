using FashionShopMVC.Models.DTO.CartDTO;

namespace FashionShopMVC.Repositories.@interface
{
    
        //IVnpayservice.cs
        public interface IVnpayRespository 
        {
            string CreatPaymentUrl(HttpContext context, VnpayMentRequestModel model);
            VnpaymentResponseModel PaymentExecute(IQueryCollection Collection);

        }

  
}
