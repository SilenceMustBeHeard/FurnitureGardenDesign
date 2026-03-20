using FurnitureGardenDesign.Data.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FurnitureGardenDesign.Data.Models.Messages
{
    public class ContactMessage : BaseDeletableEntity
    {
        public Guid Id { get; set; }

        public string SenderId { get; set; } = null!;
        public string ReceiverId { get; set; } = null!;
        public string? RespondedById { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [MinLength(3, ErrorMessage = "Subject must be at least 3 characters long.")]
        [MaxLength(200, ErrorMessage = "Subject cannot exceed 200 characters.")] 
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "Message is required.")]
        [MinLength(10, ErrorMessage = "Message must be at least 10 characters long.")]
        [MaxLength(5000, ErrorMessage = "Message cannot exceed 5000 characters.")] 
        public string Message { get; set; } = null!;

        


        public InboxMessageType Type { get; set; }


        public bool IsRead { get; set; }
        public DateTime? RespondedAt { get; set; }
      

        [MaxLength(5000, ErrorMessage = "Response cannot exceed 5000 characters.")]
        public string? Response { get; set; }


        // Navigation
        public virtual AppUser? RespondedBy { get; set; }
        public virtual AppUser Sender { get; set; } = null!;
        public virtual AppUser Receiver { get; set; } = null!;
    }
}