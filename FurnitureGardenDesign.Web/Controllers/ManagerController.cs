using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Controllers
{
    using FurnitureGardenDesign.Data.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;

    [Authorize(Policy = "ManagerOnly")]
    public class ManagerController : BaseController
    {
        public ManagerController(UserManager<AppUser> userManager) : base(userManager)
        {
        }

        public IActionResult Index()
        {
            return Ok("You are logged in as manager!");
        }
    }

}
