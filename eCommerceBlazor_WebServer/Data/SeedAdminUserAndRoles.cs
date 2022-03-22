using Microsoft.AspNetCore.Identity;
using eCommerceBlazor_Common;

namespace eCommerceBlazor_WebServer.Data
{
    public class SeedAdminUserAndRoles
    {
        internal async static Task Seed(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            await SeedRoles(roleManager);
            await SeedAdminUser(userManager);
        }
        private async static Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            bool administratorRoleExists = await roleManager.RoleExistsAsync(SD.Role_Admin);
            bool customerRoleExists = await roleManager.RoleExistsAsync(SD.Role_Customer);

            if (administratorRoleExists == false)
            {
                var adminRole = new IdentityRole
                {
                    Name = SD.Role_Admin,
                };

                await roleManager.CreateAsync(adminRole);
            }

            if (customerRoleExists == false)
            {
                var customerRole = new IdentityRole
                {
                    Name = SD.Role_Customer,
                };

                await roleManager.CreateAsync(customerRole);
            }
        }

        private async static Task SeedAdminUser(UserManager<IdentityUser> userManager)
        {
            string adminEmail = "jimmy@purnellsoftwaredevelopment.com";
            bool adminUserExists = await userManager.FindByEmailAsync(adminEmail) != null;

            if (!adminUserExists)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                };

                IdentityResult result = await userManager.CreateAsync(adminUser, "Admin1!*");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, SD.Role_Admin);
                }
            }
        }
    }
}
