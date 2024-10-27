using FashionShopMVC.Models.DTO.ProductDTO;

namespace FashionShopMVC.Models.ViewModel
{
    public class ShoppingCartViewModel
    {
        public int ProductID { get; set; }
        public GetProductByIdDTO Product { get; set; }
        public int Quantity { get; set; }
    }
}
