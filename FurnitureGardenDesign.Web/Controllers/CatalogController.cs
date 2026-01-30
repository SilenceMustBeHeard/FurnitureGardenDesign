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
        public async Task<IActionResult> Index(int page = 1, int pageSize = 9)
        {
            var allDesigns = await _catalogService.GetAllActiveAsync();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Total items before pagination
            var totalItems = allDesigns.Count();

            // Pagination
            var pagedDesigns = allDesigns
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Restriction for guests - the first 3
            if (!User.Identity!.IsAuthenticated && page == 1)
            {
                pagedDesigns = pagedDesigns.Take(3).ToList();
            }

            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalItems"] = totalItems;

            var model = pagedDesigns.Select(d => new CatalogDesignViewModel
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


        // Toggle Favorite
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(Guid id, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid Action.";
                return Redirect(returnUrl ?? Url.Action("Index"));
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            bool isNowFavorited = await _favoriteService.ToggleFavoriteAsync(userId, id);

            TempData["Success"] = isNowFavorited
                ? "You added this design to favorites!"
                : "You removed this design from favorites.";

            // if returnUrl is null, redirect to Index
            return Redirect(returnUrl ?? Url.Action("Index"));
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
            TempData["Success"] = "You added a review!";
            return RedirectToAction(nameof(Index));
        }
    }
}
