using FurnitureGardenDesign.Data.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Data.Models.Messages
{
    public class SystemInboxMessage : BaseMessage
    {
        [Required]
        [MinLength(20, ErrorMessage = "Description must be at least 20 characters long.")]
        public string Description { get; set; } = null!;
    }
}