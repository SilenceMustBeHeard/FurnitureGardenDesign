using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Web.ViewModels.Category
{
    public class CategoryViewModelEdit
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
