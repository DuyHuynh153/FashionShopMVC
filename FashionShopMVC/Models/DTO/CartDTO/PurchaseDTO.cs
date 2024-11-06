namespace FashionShopMVC.Models.DTO.CartDTO
{
    public class PurchaseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int provinceID { get; set; } 
        public int districID { get; set; }
        public int wardID { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public DateTime datetime { get; set; }
        public float Delevery_fee { get; set; }

        public int status { get; set; }

        public string UserID { get; set; }

        public string Voucher { get; set; }
        public string TypeOfPayment { get; set; }
    }
}
