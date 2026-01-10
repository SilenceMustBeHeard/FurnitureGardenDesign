using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Implementations;
using FurnitureGardenDesign.Data.Repository.Implementations.FurnitureGardenDesign.Data.Repository.Implementations;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Data.Seeding;
using FurnitureGardenDesign.Services.Core.Implementations;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.Infrastructure.Extensions;
using FurnitureGardenDesign.Web.Infrastructure.MiddleWare;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;


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





builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});




// repositories


builder.Services.AddScoped<IRepositoryAsync<CatalogDesign, Guid>, CatalogRepository>();

builder.Services.AddScoped<IRepositoryAsync<Order, Guid>, OrderRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IRepositoryAsync<CatalogDesign, Guid>, CatalogRepository>();
builder.Services.AddScoped<IRepositoryAsync<Favorite, Guid>, FavoriteRepository>();
builder.Services.AddScoped<IRepositoryAsync<Review, Guid>, ReviewRepository>();

//builder.Services.AddScoped<ICatalogService, CatalogService>();


//builder.Services.RegisterRepositories(typeof(ICategoryRepository).Assembly);
// services
//builder.Services.AddScoped<ICategoryService, CategoryService>();
//builder.Services.AddScoped<ICatalogService, CatalogService>();
//builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.RegisterServices(typeof(ICategoryService).Assembly);





// Build App 
var app = builder.Build();

//  Seed Roles and Users
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await IdentitySeeder.SeedRolesAsync(roleManager);
    await IdentitySeeder.SeedAdminAsync(userManager);
    await IdentitySeeder.SeedManagerAsync(userManager);

    
    await DbSeeder.SeedCatalogAsync(context);
}


//  Middleware 
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// Middleware for /manager routes
//app.UseMiddleware<ManagerAccessMiddleware>();





//  Routing 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();

app.Run();
