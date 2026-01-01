using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using FurnitureGardenDesign.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<DesignVariant> DesignVariants { get; set; } = null!;
        public virtual DbSet<CatalogDesign> CatalogDesigns { get; set; } = null!;
        public virtual DbSet<Favorite> Favorites { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Material> Materials { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
