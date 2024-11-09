using FashionShopMVC.Models.DTO.UserDTO;

namespace FashionShopMVC.Areas.Admin.Models.UserDTO
{
    public class PageUserListDTO
    {
        public IEnumerable<GetUserDTO> Users { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
