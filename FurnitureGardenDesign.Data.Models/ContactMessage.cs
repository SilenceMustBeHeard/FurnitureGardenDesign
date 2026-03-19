using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FurnitureGardenDesign.Data.Models
{
    public class ContactMessage : SystemInboxMessage
    {
        // Additional properties specific to contact messages
        [MaxLength(100, ErrorMessage = "Subject must be most 100 characters long.")]

        public string? Subject { get; set; }

        [MaxLength(100, ErrorMessage = "Customer name must be most 100 characters long.")]
        public string? CustomerName { get; set; }


        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? CustomerEmail { get; set; }


        


        public DateTime? RespondedAt { get; set; }

        [MaxLength(500, ErrorMessage = "Response must be most 500 characters long.")]
        public string? Response { get; set; }
    }
}
