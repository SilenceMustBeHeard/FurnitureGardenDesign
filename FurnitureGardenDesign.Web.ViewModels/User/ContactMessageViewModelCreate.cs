using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FurnitureGardenDesign.Web.ViewModels.User
{
    public class ContactMessageCreateViewModel
    {
       

        [Required(ErrorMessage = "Please enter a subject.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Subject must be between 3 and 100 characters.")]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "Please enter your message.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 2000 characters.")]
        public string Message { get; set; } = null!;
    }
}
