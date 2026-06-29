using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Web.ViewModels.Category
{
    public class CategoryViewModelCreate
    {
        public Guid Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}