using FurnitureGardenDesign.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Controllers
{
    public class DesignsController : BaseController
    {
        public DesignsController(UserManager<AppUser> userManager) : base(userManager)
        {
        }



        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create(string id)
        {
            return View();
        }
    }
}




