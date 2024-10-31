using Microsoft.AspNetCore.Identity;

namespace FashionShopMVC.Repositories.@interface
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
