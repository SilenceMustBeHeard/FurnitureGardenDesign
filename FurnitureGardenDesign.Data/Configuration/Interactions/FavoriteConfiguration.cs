using FurnitureGardenDesign.Data.Models.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureGardenDesign.Data.Configuration.Interactions
{
    public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
    {
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            builder.HasKey(f => f.Id);

            builder.HasIndex(f => new { f.UserId, f.CatalogDesignId })
                   .IsUnique();

            builder.Property(f => f.IsDeleted)
             .HasDefaultValue(false);

            builder.HasQueryFilter(f => !f.IsDeleted);
        }
    }
}