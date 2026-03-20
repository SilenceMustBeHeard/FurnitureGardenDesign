using FurnitureGardenDesign.Data.Models.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Configuration
{
    public class SystemInboxMessageConfiguration : IEntityTypeConfiguration<SystemInboxMessage>
    {
        public void Configure(EntityTypeBuilder<SystemInboxMessage> builder)
        {
            builder.HasKey(im => im.Id);

            // Message -> Receiver
            builder
                .HasOne(im => im.Receiver)
                .WithMany(u => u.SystemInboxMessages)
                .HasForeignKey(im => im.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Message -> Sender
            builder
                .HasOne(im => im.Sender)
                .WithMany(u => u.SentSystemInboxMessages)  
                .HasForeignKey(im => im.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

          
        }
    }
}