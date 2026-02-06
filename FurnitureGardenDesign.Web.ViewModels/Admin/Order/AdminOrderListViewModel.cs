using Furniture_GardenDesign.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Web.ViewModels.Admin.Order
{
    public class AdminOrderListViewModel
    {
        public Guid Id { get; set; }
        public string UserEmail { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public OrderStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }

}
