using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Web.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;
    }

  
}