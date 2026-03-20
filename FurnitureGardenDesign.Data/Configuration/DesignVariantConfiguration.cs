using FurnitureGardenDesign.Data.Models.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Furniture_GardenDesign.Data.Configuration
{
    public class DesignVariantConfiguration : IEntityTypeConfiguration<DesignVariant>
    {
        public void Configure(EntityTypeBuilder<DesignVariant> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.IsDeleted)
                  .HasDefaultValue(false);


            builder.HasQueryFilter(d => !d.IsDeleted);

            builder.HasOne(d => d.Order)
                   .WithMany(o => o.DesignVariants)
                   .HasForeignKey(d => d.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
