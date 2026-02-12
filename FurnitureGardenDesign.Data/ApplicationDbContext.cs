using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FurnitureGardenDesign.Data.Models;

namespace FurnitureGardenDesign.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
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
        public virtual DbSet<InboxMessage> InboxMessages { get; set; } = null!;


        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Material> Materials { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply all configurations from assembly
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            //  GLOBAL QUERY FILTERS 

            // Soft delete
            builder
                .Entity<Order>()
                .HasQueryFilter(o => !o.IsDeleted); // filter for order



            builder
                .Entity<CatalogDesign>()
                .HasQueryFilter(c => c.IsActive); // filter for catalogDesign


            builder
                .Entity<Category>()
                .HasQueryFilter(c => c.IsDeleted); // filter for category


            builder
                .Entity<Material>()
                .HasQueryFilter(m => m.IsOutdoorSuitable); // filter for outdoor/indoor


            // Reviews and Favorites not needed to filter out

            // Automatic set for Created on
            builder
                .Entity<Order>()
                .Property(o => o.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");  // if not set any value, giving it currnet UTC time



            builder
                .Entity<CatalogDesign>()
                .Property(c => c.Id)             // Can be added new CatalogDesign id without thinking if  ef or ssql will give it value
                .HasDefaultValueSql("NEWID()"); // automatically set new guid for id if not set in insert. 



            builder.Entity<Review>()
                .Property(r => r.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");  // if not set any value, giving it currnet UTC time
        }
    }
}
