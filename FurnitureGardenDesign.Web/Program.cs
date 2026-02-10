using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Data.Seeding;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Services.Core.Implementations;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//  Connection string 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//  Add DbContext 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//  Add Identity 
builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});

// Repositories & Services
builder.Services.RegisterRepositories(typeof(ICategoryRepository).Assembly);
builder.Services.RegisterServices(typeof(ICategoryService).Assembly);
builder.Services.AddScoped<ICategoryServiceClient, CategoryServiceClient>();

// Build App 
var app = builder.Build();

//  Seed Roles, Users and Data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await IdentitySeeder.SeedRolesAsync(roleManager);
    await IdentitySeeder.SeedAdminAsync(userManager);
    await IdentitySeeder.SeedManagerAsync(userManager);

    await DbSeeder.SeedAsync(context);
}

//  Static files with .glb support for 3D models
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".glb"] = "model/gltf-binary";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error/NotImplemented");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

await app.RunAsync();
