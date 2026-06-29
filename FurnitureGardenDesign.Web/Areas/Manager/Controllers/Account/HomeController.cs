using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FurnitureGardenDesign.Web.Areas.Manager.Controllers.Account
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class HomeController : BaseManagerController
    {
        public HomeController(UserManager<AppUser> userManager) : base(userManager)
        {
        }

        public IActionResult Index() => View();

        public IActionResult About() => View();

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}