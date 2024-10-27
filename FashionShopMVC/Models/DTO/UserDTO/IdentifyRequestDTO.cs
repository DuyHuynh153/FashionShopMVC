using System.ComponentModel.DataAnnotations;

namespace FashionShopMVC.Models.DTO.UserDTO
{
    public class IdentifyRequestDTO
    {

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng")]
        public string Email { get; set; }

    }
}
