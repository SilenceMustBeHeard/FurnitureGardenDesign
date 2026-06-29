using FurnitureGardenDesign.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureGardenDesign.Data.Configuration
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasKey(u => u.Id);

            // User <> Orders
            //builder
            //    .HasMany(u => u.Orders)
            //    .WithOne(o => o.User)
            //    .HasForeignKey(o => o.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);

            // User <= Favorites
            builder
      .HasMany(u => u.Favorites)
      .WithOne(f => f.User)   //  IMPORTANT : ONE User CAN HAVE MANY Favorites, BUT ONE Favorite CAN HAVE ONLY ONE User
      .HasForeignKey(f => f.UserId)
      .OnDelete(DeleteBehavior.Restrict);
        }
    }
}