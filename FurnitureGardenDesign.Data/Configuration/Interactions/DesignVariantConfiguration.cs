using FurnitureGardenDesign.Data.Models.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureGardenDesign.Data.Configuration.Interactions
{
    public class DesignVariantConfiguration : IEntityTypeConfiguration<DesignVariant>
    {
        public void Configure(EntityTypeBuilder<DesignVariant> builder)
        {
            builder.HasKey(d => d.Id);

            builder.HasQueryFilter(d => !d.IsDeleted);

            builder.HasOne(d => d.Order)
                   .WithMany(o => o.DesignVariants)
                   .HasForeignKey(d => d.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}