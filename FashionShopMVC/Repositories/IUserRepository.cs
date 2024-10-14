using FashionShopMVC.Data;
using FashionShopMVC.Models.Domain;

namespace FashionShopMVC.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        //Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);
        //Task<IEnumerable<User>> GetRoleNameByUser(string userName);
    }

    public class UserRepository : Repository<User>
    {
        public UserRepository(FashionShopDBContext context) : base(context) 
        {

        }
    }
}
