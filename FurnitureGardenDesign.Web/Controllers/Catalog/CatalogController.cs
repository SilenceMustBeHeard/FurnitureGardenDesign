using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurnitureGardenDesign.Web.Controllers.Catalog
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
        public virtual async Task<IActionResult> CatalogIndex(int page = 1, int pageSize = 9)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isGuest = !User.Identity!.IsAuthenticated;

            IEnumerable<CatalogDesignViewModel> designs;
            int totalItems;

            if (isGuest)
            {
                // For guests: get only first 3 designs
                designs = await _catalogService.GetPublicCatalogAsync(userId, 1, 3, isGuest);
                totalItems = 3; // Guests only see 3 items total
            }
            else
            {
                // For authenticated users: full catalog with pagination
                designs = await _catalogService.GetPublicCatalogAsync(userId, page, pageSize, isGuest);
                totalItems = await _catalogService.GetTotalActiveDesignsAsync();
            }

            // For pagination
            ViewData["CurrentPage"] = isGuest ? 1 : page;
            ViewData["PageSize"] = isGuest ? 3 : pageSize;
            ViewData["TotalItems"] = totalItems;
            ViewData["IsGuest"] = isGuest; // Add this to view data

            return View(designs);
        }


        // Details
        [HttpGet]
        [AllowAnonymous]
        public virtual async Task<IActionResult> Details(Guid id)
        {   // if user is not authenticated
            // userId will be null, and service will handle it accordingly
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            
            var model = await _catalogService.GetDetailsAsync(id, userId);

            if (model == null)
              {
                TempData["Error"] = "Design not found.";
                return NotFound(); 
              }

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
                return RedirectToAction(nameof(CatalogIndex));
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if(userId == null)
            {
               TempData["Error"] = "You must be logged in to manage favorites.";
                return RedirectToAction(nameof(CatalogIndex));

            }

            bool isNowFavorited = await _favoriteService.ToggleFavoriteAsync(userId, id);

            // Show success message based on the new favorite status
            TempData["Success"] = isNowFavorited
                ? "You added this design to favorites!"
                : "You removed this design from favorites.";

            // if returnUrl is null, redirect to Index
            return RedirectToAction(nameof(CatalogIndex));
        }



        // Add Review
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(Guid id, int rating, string? comment)
        {
            if (rating < 1 || rating > 5)
               {
                TempData["Error"] = "Rating must be between 1 and 5.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await _catalogService.AddReviewAsync(
                User.Identity!.Name!,
                id,
                rating,
                comment
            );


            TempData["Success"] = "You added a review!";
            return RedirectToAction(nameof(CatalogIndex));
        }
    }
}
