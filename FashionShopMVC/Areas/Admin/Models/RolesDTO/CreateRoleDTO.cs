using System.ComponentModel.DataAnnotations;

namespace FashionShopMVC.Areas.Admin.Models.RolesDTO
{
    public class CreateRoleDTO
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        public string Name { get; set; }
    }
}
