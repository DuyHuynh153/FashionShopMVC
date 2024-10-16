using System.ComponentModel.DataAnnotations;

namespace FashionShopMVC.Areas.Admin.Models.RolesDTO
{
    public class EditRoleDTO
    {
        [Required]
        public string ID { get; set; }

        [Required(ErrorMessage = "Trường Tên là bắt buộc.")]
        public string Name { get; set; }
    }
}
