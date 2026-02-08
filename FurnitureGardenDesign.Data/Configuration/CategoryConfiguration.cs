using Microsoft.EntityFrameworkCore.Metadata.Builders;

using FurnitureGardenDesign.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category> 
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.IsDeleted)
                   .HasDefaultValue(false);


            builder.HasQueryFilter(f => !f.IsDeleted);

            builder.HasMany(c => c.Orders)
                   .WithOne(o => o.Category)
                   .HasForeignKey(o => o.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(c => c.CatalogDesigns)
                   .WithOne(d => d.Category)
                   .HasForeignKey(d => d.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
