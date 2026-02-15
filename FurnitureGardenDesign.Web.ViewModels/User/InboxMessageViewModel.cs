using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Web.ViewModels.User
{
    public class InboxMessageViewModel
    {
        public Guid Id { get; set; }

        public string DesignImage2DUrl { get; set; } = null!;

        public string? Model3DUrl { get; set; } = null!;

        public string? Notes { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedOn { get; set; }
    }

}
