using FurnitureGardenDesign.Data.Models.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureGardenDesign.Data.Configuration.Interactions;

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
        builder.HasOne(o => o.User)
               .WithMany(u => u.Orders)
               .HasForeignKey(o => o.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}