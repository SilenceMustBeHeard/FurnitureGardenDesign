using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Models.Messages;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
        public virtual DbSet<SystemInboxMessage> SystemInboxMessages { get; set; } = null!;
        public virtual DbSet<ContactMessage> ContactMessages { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            builder.Entity<Order>()
                .HasQueryFilter(o => !o.IsDeleted);

            builder.Entity<DesignVariant>()
                .HasQueryFilter(d => !d.IsDeleted);

            builder.Entity<CatalogDesign>()
                .HasQueryFilter(c => !c.IsDeleted);

            builder.Entity<Category>()
                .HasQueryFilter(c => !c.IsDeleted);

            builder.Entity<Review>()
                .HasQueryFilter(r => r.CatalogDesign != null && !r.CatalogDesign.IsDeleted);

            builder.Entity<InboxMessage>()
                .HasQueryFilter(m => m.DesignVariant != null && !m.DesignVariant.IsDeleted);

           
            builder.Entity<Order>()
                .Property(o => o.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<DesignVariant>()
                .Property(d => d.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");


            builder.Entity<CatalogDesign>()
                .Property(c => c.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Entity<Review>()
                .Property(r => r.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}