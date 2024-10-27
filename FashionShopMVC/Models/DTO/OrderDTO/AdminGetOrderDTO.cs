using FashionShopMVC.Models.Domain;

namespace FashionShopMVC.Models.DTO.OrderDTO
{
    public class AdminGetOrderDTO
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public double DeliveryFee { get; set; }
        public DateTime OrderDate { get; set; }
        public int TypePayment { get; set; }
        public Voucher? Voucher { get; set; }
        public int Status { get; set; }
        public double TotalPayment { get; set; }

        public string loadTypePayment()
        {
            if (this.TypePayment == 1)
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
                    return TotalPayment * getVoucher.DiscountValue / 100;
                }
                else if (getVoucher.DiscountAmount)
                {
                    return TotalPayment - getVoucher.DiscountValue;
                }
            }
            return 0;
        }

        public double getTotalPayment()
        {
            return TotalPayment - getVoucherDiscount() + DeliveryFee;
        }
    }
}
