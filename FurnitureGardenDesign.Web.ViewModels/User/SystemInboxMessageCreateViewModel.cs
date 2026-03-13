using FurnitureGardenDesign.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FurnitureGardenDesign.Web.ViewModels.User
{
    public class SystemInboxMessageCreateViewModel
    {
        public string? ReceiverId { get; set; }
        public string? ReceiverName { get; set; }

        [Required]
        [MinLength(20, ErrorMessage = "Description must be at least 20 characters long.")]
        [Display(Name = "Message")]
        public string Description { get; set; } = null!;

        [Required]
        [Display(Name = "Message Type")]
        public InboxMessageType Type { get; set; }
   
        public List<UserSelectViewModel> AvailableUsers { get; set; } = new();
    }

   
}
