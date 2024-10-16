using FashionShopMVC.Areas.Admin.Models.RolesDTO;
using Microsoft.AspNetCore.Identity;

namespace FashionShopMVC.Repositories.@interface
{
    public interface IRoleRepository
    {
        public Task<IEnumerable<GetRoleDTO>> GetAllAsync();
        public Task<CreateRoleDTO> CreateAsync(CreateRoleDTO createRoleDTO);
        public Task<GetRoleDTO> GetByIdAsync(string id);
        public Task<GetRoleDTO> GetByNameAsync(string name);
        public Task<EditRoleDTO> UpdateAsync(EditRoleDTO createRoleDTO, string id);
        public Task<IdentityRole> DeleteAsync(string id);
    }
}
