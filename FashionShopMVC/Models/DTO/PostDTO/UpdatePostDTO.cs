using System.ComponentModel.DataAnnotations;

namespace FashionShopMVC.Models.DTO.PostDTO
{
    public class UpdatePostDTO
    {
        [Required(ErrorMessage = "Tên bài viết không được để trống")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ảnh cho bài viết")]
        public IFormFile? ImageFile { get; set; }
        public string? Image { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống")]
        public string Content { get; set; }
        public bool Status { get; set; }
    }
}
