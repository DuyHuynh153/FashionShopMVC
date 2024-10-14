using FashionShopMVC.Data;
using FashionShopMVC.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FashionShopMVC.Repositories
{
    public interface ICustomerRepository : IRepository<User>
    {
        Task<IEnumerable<User>> GetAllCustomerAsync(); 
    }

    public class CustomerRepository : Repository<User>, ICustomerRepository 
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CustomerRepository(FashionShopDBContext context ,UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        :base(context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IEnumerable<User>> GetAllCustomerAsync()
        {
            // Get all users
            var allUsers = _dbSet.AsQueryable();

            // Filter users by role "customer"
            var customers = new List<User>();
            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "customer"))
                {
                    customers.Add(user);
                }
            }
            await SaveAsync();
            return customers; // Return the list of customers
        }
    }
}
