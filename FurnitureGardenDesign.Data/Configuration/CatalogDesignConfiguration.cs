using FurnitureGardenDesign.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureGardenDesign.Data.Configurations
{
    public class CatalogDesignConfiguration : IEntityTypeConfiguration<CatalogDesign>
    {

        public void Configure(EntityTypeBuilder<CatalogDesign> builder)
        {
            builder.HasKey(d => d.Id);

            //precision for decimal fields
            builder.Property(d => d.Price)
             .HasPrecision(18, 2);


            builder.HasOne(d => d.Category)
                   .WithMany(c => c.CatalogDesigns)
                   .HasForeignKey(d => d.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            // CatalogDesign <> Materials
            builder.HasMany(d => d.Materials)
                   .WithMany(m => m.CatalogDesigns)
                   .UsingEntity(j => j.ToTable("CatalogDesignMaterials"));

            // CatalogDesign <> Reviews
            builder.HasMany(d => d.Reviews)
                   .WithOne(r => r.CatalogDesign)
                   .HasForeignKey(r => r.CatalogDesignId)
                   .OnDelete(DeleteBehavior.Restrict);

            // CatalogDesign => Favorites
            builder.HasMany(d => d.Favorites)
                   .WithOne(f => f.CatalogDesign)
                   .HasForeignKey(f => f.CatalogDesignId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(d => d.CategoryId);
            builder.HasIndex(d => d.IsActive);

        }
    }
}
