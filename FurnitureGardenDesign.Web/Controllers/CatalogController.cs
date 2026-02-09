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
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isGuest = !User.Identity!.IsAuthenticated;

            var designs = await _catalogService.GetPublicCatalogAsync(userId, page, pageSize, isGuest);

            // За pagination
            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalItems"] = await _catalogService.GetTotalActiveDesignsAsync(); 

            return View(designs);
        }



        // Details
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = await _catalogService.GetDetailsAsync(id, userId);

            if (model == null)
                return NotFound();

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
