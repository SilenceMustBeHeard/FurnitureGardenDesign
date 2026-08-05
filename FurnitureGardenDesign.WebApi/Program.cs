using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace FurnitureGardenDesign.WebApi;
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Connection string

            var connectionString = builder.Configuration
                .GetConnectionString("DefaultConnection")
          ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
              options.UseNpgsql(connectionString));

            // Identity (Cookie based – same as MVC)

            builder.Services
                .AddIdentityCore<AppUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager();

            // Authentication + Authorization

            builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddIdentityCookies();

            builder.Services.AddAuthorization();

            // Controllers + OpenAPI

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // HTTP pipeline
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}