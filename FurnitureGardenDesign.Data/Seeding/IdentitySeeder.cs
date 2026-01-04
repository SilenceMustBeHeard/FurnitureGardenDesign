using FurnitureGardenDesign.Data.Models;
using Microsoft.AspNetCore.Identity;

public static class IdentitySeeder
{
    private const string DefaultPassword = "123456";

    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    public static async Task SeedAdminAsync(UserManager<AppUser> userManager)
    {
        const string adminEmail = "admin@fgd.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new AppUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            await userManager.CreateAsync(admin, DefaultPassword);
        }
        if (!await userManager.IsInRoleAsync(admin, "Admin"))
            await userManager.AddToRoleAsync(admin, "Admin");
    }

   
}
