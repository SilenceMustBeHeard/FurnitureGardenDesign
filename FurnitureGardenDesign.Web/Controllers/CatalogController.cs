using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurnitureGardenDesign.Web.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly IFavoriteService _favoriteService;

        public CatalogController(ICatalogService catalogService, IFavoriteService favoriteService)
        {
            _catalogService = catalogService;
            _favoriteService = favoriteService;
        }

        // Index
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var designs = await _catalogService.GetAllActiveAsync();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
                IsFavorited = userId != null &&
                              d.Favorites.Any(f => f.UserId == userId && !f.IsDeleted),
                AverageRating = d.Reviews.Any()
                    ? d.Reviews.Average(r => r.Rating)
                    : 0,
                ReviewCount = d.Reviews.Count
            });

            return View(model);
        }

        // Details
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {
            var design = await _catalogService.GetByIdAsync(id);
            if (design == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = new CatalogDesignViewModel
            {
                Id = design.Id,
                Title = design.Title,
                Description = design.Description,
                ImageUrl = design.ImageUrl,
                Price = design.Price,
                CategoryName = design.Category.Name,
                IsFavorited = userId != null &&
                              design.Favorites.Any(f => f.UserId == userId && !f.IsDeleted),
                AverageRating = design.Reviews.Any()
                    ? design.Reviews.Average(r => r.Rating)
                    : 0,
                ReviewCount = design.Reviews.Count
            };

            return View(model);
        }

        // Toggle Favorite(like/unlike)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(Guid id)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid Action.";
                return RedirectToAction(nameof(Index));
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            bool isNowFavorited = await _favoriteService.ToggleFavoriteAsync(userId, id);

            if (isNowFavorited)
            {
                TempData["Success"] = "You added this design to favorites!";
            }
            else
            {
                TempData["Success"] = "You removed this design from favorites.";
            }

            return RedirectToAction(nameof(Index));
        }



        // Add Review
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(Guid id, int rating, string? comment)
        {
            if (rating < 1 || rating > 5)
                return RedirectToAction(nameof(Details), new { id });

            await _catalogService.AddReviewAsync(
                User.Identity!.Name!,
                id,
                rating,
                comment
            );

            return RedirectToAction(nameof(Index));
        }
    }
}
