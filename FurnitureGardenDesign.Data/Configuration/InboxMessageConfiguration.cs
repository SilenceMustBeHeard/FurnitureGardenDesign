using FurnitureGardenDesign.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Configuration
{
    public class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
    {
        public void Configure(EntityTypeBuilder<InboxMessage> builder)
        {
            builder.HasKey(im => im.Id);

            // Message -> Receiver
            builder
                .HasOne(im => im.Receiver)
                .WithMany(u => u.InboxMessages)
                .HasForeignKey(im => im.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Message -> Sender
            builder
                .HasOne(im => im.Sender)
                .WithMany(u => u.SentMessages)  
                .HasForeignKey(im => im.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Message -> DesignVariant
            builder
                .HasOne(im => im.DesignVariant)
                .WithMany()
                .HasForeignKey(im => im.DesignVariantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}