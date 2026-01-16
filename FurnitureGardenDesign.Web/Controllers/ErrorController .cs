using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Controllers
{
    

    public class ErrorController : Controller
    {
        [Route("Error/NotImplemented")]
        public IActionResult NotImplemented()
        {
            return View();
        }
    }

}
