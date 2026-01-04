using FurnitureGardenDesign.Services.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Controllers
{
    public class CatalogDesignsController : Controller
    {
        private readonly ICatalogService _catalogService;

        public CatalogDesignsController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<IActionResult> Index()
        {
            var designs = await _catalogService.GetAllDesignsAsync();
            return View(designs);
        }
    }


}
