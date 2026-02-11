using Microsoft.AspNetCore.Mvc;


namespace FurnitureGardenDesign.Web.Controllers
{
    

    public class ErrorController : Controller
    {
        public IActionResult NotImplemented(string? feature)
        {
            ViewBag.FeatureName = feature ?? "This feature";
            return View();
        }
        public IActionResult NotAllowed()
        {
            return View();
        }
    }

}
