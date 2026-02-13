using Microsoft.AspNetCore.Mvc;


namespace FurnitureGardenDesign.Web.Controllers
{
    

    public class ErrorController : Controller
    {
        // Handles errors (not found or not developed features)
        public IActionResult NotImplemented(string? feature)
        {
            ViewBag.FeatureName = feature ?? "This feature";
            return View();
        }
        // Handles errors (forbidden access)
        public IActionResult NotAllowed()
        {
            return View();
        }
    }

}
