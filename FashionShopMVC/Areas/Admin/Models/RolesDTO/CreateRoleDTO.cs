using System.ComponentModel.DataAnnotations;

namespace FashionShopMVC.Areas.Admin.Models.RolesDTO
{
    public class CreateRoleDTO
    {
        public string ID { get; set; }
        [Required(ErrorMessage = "Tên không được để trống")]
        public string Name { get; set; }
    }
}
