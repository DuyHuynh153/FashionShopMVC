

using FashionShopMVC.Models.Domain;

namespace FashionShopMVC.Models.DTO.FavoriteProductDTO
{
    public class GetFavoriteProductByUserIdDTO
    {
        public string UserID { get; set; }
        public int ProductID { get; set; }
        public Product ProductDTO { get; set; }
        public DateTime AddedDate { get; set; }
    }
}

