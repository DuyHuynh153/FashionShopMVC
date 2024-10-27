using FashionShopMVC.Models.DTO.UserDTO;
using FashionShopMVC.Data;
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.UserDTO;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FashionShopMVC.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FashionShopDBContext _context;
        private readonly UserManager<User> _userManager;

        public UserRepository(FashionShopDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> AccountLock(string idAccount)
        {
            var existingUser = await _userManager.FindByIdAsync(idAccount);
            if (existingUser != null)
            {
                existingUser.LockoutEnabled = !existingUser.LockoutEnabled;
                var result = await _userManager.UpdateAsync(existingUser);
                if (result.Succeeded)
                {
                    await _context.SaveChangesAsync();
                    return true;
                }
                foreach(var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                }
            }
            return false;

        }

        public async Task<bool> AccountUnlock(string idAccount)
        {
            var existingUser = _userManager.FindByIdAsync(idAccount).Result;
            if (existingUser != null && existingUser.LockoutEnabled)
            {
                existingUser.LockoutEnabled = false;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public Task<int> Count()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Delete(string id)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                return false;
            }
            var result = await _userManager.DeleteAsync(existingUser);
            if (result.Succeeded)
            {
                return true;
            }
            foreach (var error in result.Errors)
            {
                Console.WriteLine(error.Description);
            }

            return false;

        }

        public async Task<IEnumerable<GetUserDTO>> GetAllUserAsync(string? searchByName, string? filterRole)
        {
            var listUserDomain = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchByName))
            {
                listUserDomain = listUserDomain.Where(user => user.FullName.Contains(searchByName));
            }

            if (!string.IsNullOrEmpty(filterRole))
            {
                var userIdsWithRole = await _context.UserRoles
                    .Where(ur => ur.RoleId == filterRole)
                    .Select(ur => ur.UserId)
                    .ToListAsync();

                listUserDomain = listUserDomain.Where(u => userIdsWithRole.Contains(u.Id));
            }

            var listUserDTO = await listUserDomain.Select(user => new GetUserDTO()
            {
                ID = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = _context.UserRoles
                    .Where(ur => ur.UserId == user.Id)
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .FirstOrDefault(),
                LockoutEnabled = user.LockoutEnabled
            }).OrderByDescending(user => user.FullName).ToListAsync();

            return listUserDTO;
        }

        public async Task<GetUserDTO> GetUserByIdAsync(string id)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                return null;
            }
            var role = await _context.UserRoles
                .Where(ur => ur.UserId == existingUser.Id)
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                .FirstOrDefaultAsync();
            return new GetUserDTO
            {
                ID = existingUser.Id,
                FullName = existingUser.FullName,
                Email = existingUser.Email,
                PhoneNumber = existingUser.PhoneNumber,
                Role = role,
                LockoutEnabled = existingUser.LockoutEnabled
            };

        }

        public Task<string> Login(LoginRequestDTO loginRequestDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RegisterAccountAdminAsync(RegisterRequestDTO registerRequestDTO)
        {
            var admin = new User
            {
                FullName = registerRequestDTO.FullName,
                UserName = registerRequestDTO.Email,
                Email = registerRequestDTO.Email,
                PhoneNumber = registerRequestDTO.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(admin, registerRequestDTO.Password);


            if (result.Succeeded)
            {
                result = await _userManager.AddToRoleAsync(admin, "Quản trị viên");

                if (result.Succeeded)
                {
                    admin.LockoutEnabled = false;
                    await _context.SaveChangesAsync();

                    return true;
                }
            }
            return false;
        }

        public Task<bool> RegisterAccountCustomer(RegisterRequestDTO registerRequestDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RegisterAccountEmployeeAsync(RegisterRequestDTO registerRequestDTO)
        {
            var existingEmployee = await _userManager.FindByEmailAsync(registerRequestDTO.Email);
            if (existingEmployee != null)
            {
                return false;
            }
            
            var result = await _userManager.CreateAsync(new User
            {
                FullName = registerRequestDTO.FullName,
                UserName = registerRequestDTO.Email,
                Email = registerRequestDTO.Email,
                PhoneNumber = registerRequestDTO.PhoneNumber,
                LockoutEnabled = false
            }, registerRequestDTO.Password);

            if (result.Succeeded)
            {
                result = await _userManager.AddToRoleAsync(await _userManager.FindByEmailAsync(registerRequestDTO.Email), "Nhân viên");
                if (result.Succeeded)
                {
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<UpdateUserDTO> UpdateAsync(UpdateUserDTO updateUserDTO, string id)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                return null;
            }

            // check for email conflict 

            var emailUser = await _userManager.FindByEmailAsync(updateUserDTO.Email);
            if (emailUser != null && emailUser.Id != id)
            {
                return null;
            }
            // success at geting an use, now just need to update that user
            existingUser.FullName = updateUserDTO.FullName;
            existingUser.Email = updateUserDTO.Email;
            existingUser.PhoneNumber = updateUserDTO.PhoneNumber;


            var result = await _userManager.UpdateAsync(existingUser);
            if (result.Succeeded)
            {
                return updateUserDTO;
            }

            foreach (var error in result.Errors)
            {
                Console.WriteLine(error.Description);
            }

            return null;

        }
    }
}
