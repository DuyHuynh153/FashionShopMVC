namespace FashionShopMVC.Models.DTO.OrderDTO
{
    public class OrderDetailDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl {  get; set; }
        public int OrderID { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
