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

                string[] roles = new string[] { "Administrator", "Quản Trị Viên", "Khách Hàng" };

                foreach (string role in roles)
                {
                    if (!context.Roles.Any(r => r.Name == role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Check if any users exist in the database
                var existingUser = await userManager.FindByNameAsync("Owner");
                if (existingUser == null)
                {
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
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                        LockoutEnabled = false
                    };

                    // Create the user with a password
                    var result = await userManager.CreateAsync(user, "Duy@123");
                    if (result.Succeeded)
                    {
                        // Assign the "Quản Trị Viên" role to the new user
                        await userManager.AddToRoleAsync(user, "Quản Trị Viên");
                    }
                }
                else
                {
                    // Update existing user attributes
                    existingUser.FullName = "Admin User";
                    existingUser.Email = "Duy@gmail.com";
                    existingUser.NormalizedEmail = "DUY@GMAIL.COM";
                    existingUser.PhoneNumber = "+111111111111";
                    existingUser.EmailConfirmed = true;
                    existingUser.PhoneNumberConfirmed = true;
                    existingUser.SecurityStamp = Guid.NewGuid().ToString("D");
                    existingUser.LockoutEnabled = false;

                    await userManager.UpdateAsync(existingUser);

                    // Ensure the user has the "Quản Trị Viên" role
                    if (!await userManager.IsInRoleAsync(existingUser, "Quản Trị Viên"))
                    {
                        await userManager.AddToRoleAsync(existingUser, "Quản Trị Viên");
                    }
                }
            }
        }
    }
}
