using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FashionShopMVC.Data;
using FashionShopMVC.Models.Domain;

namespace sportMVC.Models.Seed
{
    public class SampleData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {

            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<FashionShopDBContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = new string[] {  "Administrator", "Quản Trị Viên", "Khách Hàng" };

                foreach (string role in roles)
                {
                    if (!context.Roles.Any(r => r.Name == role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                var user = new User
                {
                    FullName = "Admin User",
                    Email = "Duy@gmail.com",
                    NormalizedEmail = "DUY@GMAIL.COM",
                    UserName = "Owner",
                    NormalizedUserName = "OWNER",
                    PhoneNumber = "+111111111111",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D")

                };
                if (!context.Users.Any(u => u.UserName == user.UserName))
                {
                    var result = await userManager.CreateAsync(user, "secret");
                    if (result.Succeeded)
                    {
                        await AssignRoles(scope.ServiceProvider, user.Email, new[] { "Quản Trị Viên" });
                    }
                }await userManager.CreateAsync(user);


            }
        }

        public static async Task<IdentityResult> AssignRoles(IServiceProvider services, string email, string[] roles)
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"User with email {email} not found." });
            }

            var result = await userManager.AddToRolesAsync(user, roles);
            return result;
        }

    }
}
