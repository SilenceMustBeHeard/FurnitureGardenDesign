using FurnitureGardenDesign.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Web.ViewModels.User
{
    public class InboxMessageViewModel
    {
        public Guid Id { get; set; }
        public Guid DesignVariantId { get; set; }
        public string DesignImage2DUrl { get; set; } = null!;
        public string? Model3DUrl { get; set; }
        public string? Notes { get; set; }
        public bool IsRead { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedOn { get; set; }

        public InboxMessageType Type { get; set; } 
        public string? TypeDisplayName => Type.ToString();

        public string? OrderDescription { get; set; }
        public string? OrderDimensions { get; set; }
        public string? ReferenceImageUrl { get; set; }
    }

}
