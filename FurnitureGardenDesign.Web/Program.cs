using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Data.Seeding;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Services.Core.Implementations;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Connection string 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add DbContext 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Identity 
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

    options.AddPolicy("ManagerPolicy", policy =>
       policy.RequireRole("Manager"));
});

// Repositories & Services
builder.Services.RegisterRepositories(typeof(ICategoryRepository).Assembly);
builder.Services.RegisterServices(typeof(ICategoryService).Assembly);
builder.Services.AddScoped<ICategoryServiceClient, CategoryServiceClient>();

// Add MVC with custom error handling
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configure error handling
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();

// Seed Roles, Users and Data
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

// Static files with .glb support for 3D models
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".glb"] = "model/gltf-binary";

// Configure error handling middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/500");
    app.UseHsts();
}
else
{
    // In development, show detailed errors but still use custom error pages for status codes
    app.UseDeveloperExceptionPage();
}

// Use status code pages with re-execute
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();

// Serve static files with the custom content type provider
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Custom error handling for 404 when no endpoint found
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        // Re-execute the error handling middleware
        var originalPath = context.Request.Path;
        context.Items["originalPath"] = originalPath;
        context.Request.Path = "/Error/404";
        await next();
    }
});

// Routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

await app.RunAsync();