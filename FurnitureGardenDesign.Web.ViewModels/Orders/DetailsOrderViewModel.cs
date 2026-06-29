using Furniture_GardenDesign.Data.Enums;

namespace FurnitureGardenDesign.Web.ViewModels.Orders
{
    public class DetailsOrderViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; } = null!;

        public string FurnitureType { get; set; } = null!;

        public string Dimensions { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string? ReferenceImageUrl { get; set; }

        public OrderStatus Status { get; set; }

        public Guid CategoryId { get; set; }
    }
}