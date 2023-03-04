using ClaimExample.Models;
using Microsoft.AspNetCore.Identity;
using System;

namespace ClaimExample.Data
{
    public static class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            foreach (var role in (Roles[])Enum.GetValues(typeof(Roles)))
            {
                await roleManager.CreateAsync(new IdentityRole(role.ToString()));
            }
        }

        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "superadmin",
                Email = "superadmin@nhatkyhoctap.com",
                FirstName = "Seta",
                LastName = "Soujiro",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "123Pa$$word.");
                    foreach (var role in (Roles[])Enum.GetValues(typeof(Roles)))
                    {
                        await userManager.AddToRoleAsync(defaultUser, role.ToString());
                    }
                }

            }
        }
    }
}
