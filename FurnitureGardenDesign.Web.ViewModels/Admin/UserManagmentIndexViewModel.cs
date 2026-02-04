using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Web.ViewModels.Admin
{
    public class UserManagmentIndexViewModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;

        public IEnumerable<string> Roles { get; set; } = null!;

    }
}
