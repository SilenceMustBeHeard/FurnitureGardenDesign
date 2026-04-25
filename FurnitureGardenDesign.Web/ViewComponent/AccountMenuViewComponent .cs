

namespace FurnitureGardenDesign.Web.ViewComponents
{
    using FurnitureGardenDesign.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class AccountMenuViewComponent : ViewComponent
    {
        private readonly SignInManager<AppUser> _signInManager;

        public AccountMenuViewComponent(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public IViewComponentResult Invoke()
        {
            var isLoggedIn = _signInManager.IsSignedIn(HttpContext.User);
            return View(isLoggedIn);
        }
    }

}
