using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces.Catalog;
using FurnitureGardenDesign.Data.Seeding;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Services.Core.Implementations.Catalog;
using FurnitureGardenDesign.Services.Core.Interfaces.Catalog;
using FurnitureGardenDesign.Web.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add DbContext - PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add HttpClient factory for services that need to make HTTP requests
builder.Services.AddHttpClient();

// Add Identity
builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;

    // password settings
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 10;
    options.Password.RequiredUniqueChars = 4;

    // lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // user settings
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
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

// MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configure error handling
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


// Health Checks
builder.Services.AddHealthChecks();
var app = builder.Build();

// Seed Roles, Users and Data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Apply migrations
    await context.Database.MigrateAsync();

    // Seed Identity
    await IdentitySeeder.SeedRolesAsync(roleManager);
    await IdentitySeeder.SeedAdminAsync(userManager);
    await IdentitySeeder.SeedManagerAsync(userManager);

    // Seed Catalog Data
    var anyCategories = await context.Categories.AnyAsync();

    if (!anyCategories)
    {
        try
        {
            await DbSeeder.SeedCategoriesAsync(context);
            await DbSeeder.SeedCatalogAsync(context);

            Console.WriteLine("Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Seeding failed: {ex.Message}");
            throw;
        }
    }
    else
    {
        Console.WriteLine("Categories already exist. Skipping catalog data seeding.");
    }
}

// Static files with .glb support
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".glb"] = "model/gltf-binary";

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/500");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Custom 404 handling
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        context.Items["originalPath"] = context.Request.Path;
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

app.MapHealthChecks("/health");
await app.RunAsync();