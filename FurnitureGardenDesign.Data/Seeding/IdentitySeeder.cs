using FurnitureGardenDesign.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace FurnitureGardenDesign.Data.Seeding
{
    public static class IdentitySeeder
    {
        private const string DefaultPassword = "Garden$2026";

        // 1️⃣ Seed Roles
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Manager", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2️⃣ Seed Admin
        public static async Task SeedAdminAsync(UserManager<AppUser> userManager)
        {
            const string adminEmail = "admin@fgd.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, DefaultPassword);
            }

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // 3️⃣ Seed Manager
        public static async Task SeedManagerAsync(UserManager<AppUser> userManager)
        {
            const string managerEmail = "manager@fgd.com";
            var manager = await userManager.FindByEmailAsync(managerEmail);

            if (manager == null)
            {
                manager = new AppUser
                {
                    UserName = managerEmail,
                    Email = managerEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(manager, DefaultPassword);
            }

            if (!await userManager.IsInRoleAsync(manager, "Manager"))
                await userManager.AddToRoleAsync(manager, "Manager");
        }
    }
}