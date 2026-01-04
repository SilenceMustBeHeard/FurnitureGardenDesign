using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Web.ViewModels
{
    public class NavbarButtonsViewModel
    {
        public bool IsLoggedIn { get; set; }
        public bool IsUser { get; set; }
        public bool IsManager { get; set; }
        public bool IsAdmin { get; set; }

        public int PendingOrdersCount { get; set; }
    }


}
