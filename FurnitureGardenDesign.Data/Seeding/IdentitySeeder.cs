using FurnitureGardenDesign.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace CinemaApp.Data.Seeding
{
    public static class IdentitySeeder
    {
        private const string DefaultPassword = "1234567890";

        // 🔹 Seed Roles + Admin + Manager
        public static async Task SeedRolesAndUsersAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // ====== 1️⃣ Roles ======
            string[] roles = { "Admin", "Manager" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // ====== 2️⃣ Admin ======
            const string adminEmail = "admin@cinema.com";
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
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // ====== 3️⃣ Manager ======
            const string managerEmail = "manager@cinema.com";
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
            {
                await userManager.AddToRoleAsync(manager, "Manager");
            }
        }
    }
}
