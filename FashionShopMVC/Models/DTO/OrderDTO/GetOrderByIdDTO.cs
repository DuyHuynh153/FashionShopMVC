using FashionShopMVC.Models.Domain;

namespace FashionShopMVC.Models.DTO.OrderDTO
{
    public class GetOrderByIdDTO
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
        public List<OrderDetail> OrderDetails { get; set; }
        public Voucher? Voucher { get; set; }

        public string loadTypePayment()
        {
            if(this.TypePayment == 1)
            {
                return "COD";
            }
            else
            {
                return "Chuyển khoản";
            }
        }

        public string loadStatusOrder()
        {
            if (this.Status == 0)
            {
                return "Đã hủy";
            }
            else if (this.Status == 1)
            {
                return "Chờ xác nhận";
            }
            else if (this.Status == 2)
            {
                return "Đang giao hàng";
            }
            return "Giao hàng thành công";
        }

        public double getTotalMoney()
        {
            return this.OrderDetails.Sum(item => item.Quantity * item.Price);
        }

        public string getVNDPrice(double price)
        {
            return string.Format("{0:C}", price);
        }

        public double getVoucherDiscount()
        {
            var getVoucher = this.Voucher;
            if (getVoucher != null)
            {
                if (getVoucher.DiscountPercentage)
                {
                    return getTotalMoney() * getVoucher.DiscountValue / 100;
                }
                else if (getVoucher.DiscountAmount)
                {
                    return getTotalMoney() - getVoucher.DiscountValue;
                }
            }
            return 0;
        }

        public double getTotalPayment()
        {
            return getTotalMoney() - getVoucherDiscount() + DeliveryFee;
        }
    }
}
