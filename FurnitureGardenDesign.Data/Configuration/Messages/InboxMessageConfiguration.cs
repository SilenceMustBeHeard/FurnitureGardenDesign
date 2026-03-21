using FurnitureGardenDesign.Data.Models.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Configuration.Messages
{
    public class InboxMessageConfiguration : BaseMessageConfiguration<InboxMessage>
    {
        public override void Configure(EntityTypeBuilder<InboxMessage> builder)
        {
            base.Configure(builder);

            // Override navigation relationships
            builder
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedDesignMessages)
                .HasForeignKey(m => m.ReceiverId);

            builder
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentDesignMessages)
                .HasForeignKey(m => m.SenderId);

            // DesignVariant relationship
            builder
                .HasOne(m => m.DesignVariant)
                .WithMany()
                .HasForeignKey(m => m.DesignVariantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}