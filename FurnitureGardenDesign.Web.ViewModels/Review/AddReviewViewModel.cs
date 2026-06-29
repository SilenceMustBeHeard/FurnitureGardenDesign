using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Web.ViewModels.Review
{
    public class AddReviewViewModel
    {
        public Guid CatalogDesignId { get; set; }

        public string CatalogDesignTitle { get; set; } = null!;

        [Range(0, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}