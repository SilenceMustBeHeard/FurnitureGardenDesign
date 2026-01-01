using FurnitureGardenDesign.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureGardenDesign.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);


            // Order ↔ DesignVariants
            builder.HasMany(o => o.DesignVariants)
                   .WithOne(d => d.Order)
                   .HasForeignKey(d => d.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Order ↔ Category
            builder.HasOne(o => o.Category)
                   .WithMany(c => c.Orders)
                   .HasForeignKey(o => o.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Order ↔ User (ApplicationUser)
            builder.HasOne<ApplicationUser>()
                   .WithMany(u => u.Orders)
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

