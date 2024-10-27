using FashionShopMVC.Areas.Admin.Models.RolesDTO;
using FashionShopMVC.Data;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FashionShopMVC.Repositories
{

    public class RoleRepository : IRoleRepository
    {
        private readonly FashionShopDBContext _fashionShopDBContext;

        public RoleRepository(FashionShopDBContext fashionShopDBContext)
        {
            _fashionShopDBContext = fashionShopDBContext;
        }
        public async Task<IEnumerable<GetRoleDTO>> GetAllAsync()
        {
            var listRoleDomain = await _fashionShopDBContext.Roles.Select(role => new GetRoleDTO()
            {
                ID = role.Id,
                Name = role.Name,
            }).ToListAsync();

            return listRoleDomain;
        }

        public async Task<GetRoleDTO> GetByIdAsync(string id)
        {
            var role = await _fashionShopDBContext.Roles
                .Where(role=>role.Id == id)
                .Select(role => new GetRoleDTO()
                {
                    ID = role.Id,
                    Name = role.Name
                }).FirstOrDefaultAsync();

            return role;
        }

        public async Task<CreateRoleDTO> CreateAsync(CreateRoleDTO createRoleDTO)
        {
            var existingRole = await _fashionShopDBContext.Roles.FirstOrDefaultAsync(role => role.Name == createRoleDTO.Name);
            if (existingRole != null)
            {
                return null; // Role already exists
            }
            var newRole = new IdentityRole()
            {
                Id = Guid.NewGuid().ToString(),
                ConcurrencyStamp = new Guid().ToString(),
                Name = createRoleDTO.Name,
                NormalizedName = createRoleDTO.Name.ToUpper(),
            };

            await _fashionShopDBContext.Roles.AddAsync(newRole);
            await _fashionShopDBContext.SaveChangesAsync();

            return createRoleDTO;
        }

        public async Task<EditRoleDTO> UpdateAsync(EditRoleDTO createRoleDTO, string id)
        {
            var existingRole = await _fashionShopDBContext.Roles.FirstOrDefaultAsync(role => role.Id == id);
            if (existingRole == null)
            {
                return null; // Role not found
            }

            existingRole.Name = createRoleDTO.Name;
            existingRole.NormalizedName = createRoleDTO.Name.ToUpper();

            await _fashionShopDBContext.SaveChangesAsync();
            return createRoleDTO;
        }

        public async Task<IdentityRole> DeleteAsync(string id)
        {
            var existingRole = await _fashionShopDBContext.Roles.FirstOrDefaultAsync(role => role.Id == id);
            if (existingRole == null)
            {
                return null; // Role not found
            }

            _fashionShopDBContext.Roles.Remove(existingRole);
            await _fashionShopDBContext.SaveChangesAsync();
            return existingRole;
        }

        public Task<GetRoleDTO> GetByNameAsync(string name)
        {
            var existingRole = _fashionShopDBContext.Roles
                .Where(role => role.Name == name)
                .Select(role => new GetRoleDTO()
                {
                    ID = role.Id,
                    Name = role.Name
                }).FirstOrDefaultAsync();   

            if (existingRole == null)
            {
                return null; // Role not found
            }
            return existingRole;
        }
    }
}
