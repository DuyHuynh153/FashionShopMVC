using System.ComponentModel.DataAnnotations;

namespace FashionShopMVC.Models.DTO.UserDTO
{
    public class ResetPasswordRequestDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(3, ErrorMessage = "Mật khẩu phải có ít nhất 3 ký tự")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp")]
        public string RePassword { get; set; }
    }
}
