using FurnitureGardenDesign.Data.Models.Messages;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureGardenDesign.Data.Configuration.Messages
{
    public class SystemInboxMessageConfiguration : BaseMessageConfiguration<SystemInboxMessage>
    {
        public override void Configure(EntityTypeBuilder<SystemInboxMessage> builder)
        {
            base.Configure(builder);

            builder
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedSystemMessages)
                .HasForeignKey(m => m.ReceiverId);

            builder
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentSystemMessages)
                .HasForeignKey(m => m.SenderId);
        }
    }
}