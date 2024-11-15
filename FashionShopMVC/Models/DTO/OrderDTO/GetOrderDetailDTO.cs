using FashionShopMVC.Models.Domain;

namespace FashionShopMVC.Models.DTO.OrderDTO
{
    public class GetOrderDetailDTO
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
        public string Address { get; set; }
        public double DeliveryFee { get; set; }
        public string? Note { get; set; }
        public DateTime OrderDate { get; set; }
        public int TypePayment { get; set; }
        public int Status { get; set; }
        public string UserID { get; set; }
        public IEnumerable<OrderDetailDTO> OrderDetails { get; set; }
        public Voucher? Voucher { get; set; }
    }
}
