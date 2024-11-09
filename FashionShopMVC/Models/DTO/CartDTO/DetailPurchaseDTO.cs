namespace FashionShopMVC.Models.DTO.CartDTO
{
    public class DetailPurchaseDTO
    {
        public int orderId { get; set; }

        public int productId { get; set; }
        public float price { get; set; }

        public int quantity { get; set; }
    }
}
