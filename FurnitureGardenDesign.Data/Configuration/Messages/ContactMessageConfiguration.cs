using FurnitureGardenDesign.Data.Models.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureGardenDesign.Data.Configuration.Messages
{
    public class ContactMessageConfiguration : BaseMessageConfiguration<ContactMessage>
    {
        public override void Configure(EntityTypeBuilder<ContactMessage> builder)
        {
            base.Configure(builder);

            builder
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedContactMessages)
                .HasForeignKey(m => m.ReceiverId);

            builder
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentContactMessages)
                .HasForeignKey(m => m.SenderId);

            // RespondedBy relationship
            builder
                .HasOne(m => m.RespondedBy)
                .WithMany()
                .HasForeignKey(m => m.RespondedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}