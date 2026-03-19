using FurnitureGardenDesign.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Configuration
{
    public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
    {
        public void Configure(EntityTypeBuilder<ContactMessage> builder)
        {



            builder
           .HasOne(im => im.Receiver)
           .WithMany(u => u.ContactMessages)  
           .HasForeignKey(im => im.ReceiverId)
           .OnDelete(DeleteBehavior.Restrict);

            
            builder
                .HasOne(im => im.Sender)
                .WithMany(u => u.SentContactMessages)  
                .HasForeignKey(im => im.SenderId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
