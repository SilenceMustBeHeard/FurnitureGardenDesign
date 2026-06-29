using FurnitureGardenDesign.Data.Models.Catalog;

namespace FurnitureGardenDesign.Data.Models.Interactions
{
    public class Favorite
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Foreign key to the user who favorited the catalog design
        public string UserId { get; set; } = null!;

        public virtual AppUser User { get; set; } = null!;

        // Foreign key to the catalog design being favorited
        public Guid CatalogDesignId { get; set; }

        public virtual CatalogDesign CatalogDesign { get; set; } = null!;

        public bool IsDeleted { get; set; } = false;
    }
}