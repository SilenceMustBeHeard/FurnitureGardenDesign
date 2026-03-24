using System.Collections.Generic;
using FurnitureGardenDesign.Web.ViewModels.Messages;

namespace FurnitureGardenDesign.Web.ViewModels.Admin
{
    public class AdminInboxViewModel
    {
        public List<InboxMessageViewModel> DesignMessages { get; set; } = new();
        public List<ContactMessageDetailsViewModel> ContactMessages { get; set; } = new();
    }
}