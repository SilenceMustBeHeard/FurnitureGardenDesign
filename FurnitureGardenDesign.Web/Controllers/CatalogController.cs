using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Controllers
{


    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        [AllowAnonymous]
     
        public async Task<IActionResult> Index()
        {
            var designs = await _catalogService.GetAllActiveAsync();


            if (!User.Identity!.IsAuthenticated)
            {
                designs = designs.Take(3).ToList();
                ViewData["IsGuest"] = true;
            }
            else
            {
                ViewData["IsGuest"] = false;
            }

            var model = designs.Select(d => new CatalogDesignViewModel
            {
                Id = d.Id,
                Title = d.Title,
                Description = d.Description,
                ImageUrl = d.ImageUrl,
                Price = d.Price,
                CategoryName = d.Category.Name,
                IsFavorited = d.Favorites.Any(f => f.UserId == User.Identity!.Name),
                AverageRating = d.Reviews.Any() ? d.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = d.Reviews.Count
            });

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Explore()
        {
            IEnumerable<CatalogDesignViewModel> model =
                await _catalogService.GetPublicCatalogAsync();

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddFavorite(Guid id)
        {
            var model = _catalogService.GetByIdAsync(id);

            if (!ModelState.IsValid)
                return View(model);
            await _catalogService
                .AddToFavoritesAsync(User.Identity!.Name!, id);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveFavorite(Guid id)
        {
            var model = _catalogService.GetByIdAsync(id);

            if (!ModelState.IsValid)
                return View(model);

            await _catalogService
                .RemoveFromFavoritesAsync(User.Identity!.Name!, id);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(Guid id, int rating, string? comment)
        {
            var model = _catalogService.GetByIdAsync(id);

            if (!ModelState.IsValid)
                return View(model);

            await _catalogService
                .AddReviewAsync(User.Identity!.Name!, id, rating, comment);


            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {
            var model1 = _catalogService.GetByIdAsync(id);

            if (!ModelState.IsValid)
                return View(model1);

            var design = await _catalogService.GetByIdAsync(id);
            if (design == null)
                return NotFound();

          


            var model = new CatalogDesignViewModel
            {
                Id = design.Id,
                Title = design.Title,
                Description = design.Description,
                ImageUrl = design.ImageUrl,
                Price = design.Price,
                CategoryName = design.Category.Name,

                IsFavorited = design
                .Favorites.Any(f => f.UserId == User.Identity!.Name),

                AverageRating = design
                .Reviews.Any() 
                ? design.Reviews.Average(r => r.Rating) : 0,

                ReviewCount = design.Reviews.Count
            };

            return View(model);
        }
    }

}
