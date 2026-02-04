using FurnitureGardenDesign.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : BaseAdminController
    {
        public HomeController(UserManager<AppUser> userManager) : base(userManager)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
