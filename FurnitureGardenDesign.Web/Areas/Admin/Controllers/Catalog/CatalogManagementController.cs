using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FurnitureGardenDesign.Web.Areas.Admin.Controllers.Catalog
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CatalogManagementController : Controller
    {
        private readonly ICatalogManagementService _catalogManagementService;
        private readonly ICategoryServiceClient _categoryServiceClient;
        private readonly IFavoriteService _favoriteService; 

        public CatalogManagementController(
            ICatalogManagementService catalogManagementService,
            ICategoryServiceClient categoryServiceClient,
            IFavoriteService favoriteService)
        {
            _catalogManagementService = catalogManagementService;
            _categoryServiceClient = categoryServiceClient;
            _favoriteService = favoriteService; 
        }

        

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditList()
        {
            var catalogues = await _catalogManagementService.GetAllCataloguesForAdminAsync();
            return View("EditList", catalogues);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            await _catalogManagementService.ToggleCatalogAsync(id);
            TempData["Success"] = "Catalogue status changed!";
            return RedirectToAction(nameof(EditList));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var model = new CatalogViewModelCreate();
            await LoadCategoriesAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CatalogViewModelCreate model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(model);
                return View(model);
            }

            await _catalogManagementService.AddCatalogAsync(model);
            TempData["Success"] = "Catalogue created successfully!";
            return RedirectToAction(nameof(EditList));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _catalogManagementService.GetCatalogForEditByIdAsync(id);
            if (model == null) return NotFound();

            await LoadCategoriesAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(CatalogViewModelEdit model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(model);
                return View(model);
            }

            await _catalogManagementService.EditCatalogAsync(model.Id, model);
            TempData["Success"] = "Catalogue edited successfully!";
            return RedirectToAction(nameof(EditList));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex(int page = 1, int pageSize = 9)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isGuest = !User.Identity!.IsAuthenticated;

            var designs = await _catalogManagementService.GetPublicCatalogAsync(userId, page, pageSize, isGuest);

            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalItems"] = await _catalogManagementService.GetTotalActiveDesignsAsync();

            return View(designs);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(Guid id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = await _catalogManagementService.GetDetailsAsync(id, userId);

            if (model == null)
            {
                TempData["Error"] = "Design not found.";
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddReview(Guid id, int rating, string? comment)
        {
            if (rating < 1 || rating > 5)
            {
                TempData["Error"] = "Rating must be between 1 and 5.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await _catalogManagementService.AddReviewAsync(
                User.Identity!.Name!,
                id,
                rating,
                comment
            );

           if(!ModelState.IsValid)
            {
                TempData["Error"] = "Failed to add review. Please try again.";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["Success"] = "You added a review!";
            return RedirectToAction(nameof(AdminIndex));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(Guid id, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid Action.";
                return RedirectToAction(nameof(AdminIndex));
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to manage favorites.";
                return RedirectToAction(nameof(AdminIndex));

            }

            bool isNowFavorited = await _favoriteService.ToggleFavoriteAsync(userId, id);

           
            TempData["Success"] = isNowFavorited
                ? "You added this design to favorites!"
                : "You removed this design from favorites.";

           
            return RedirectToAction(nameof(AdminIndex));
        }



       
        private async Task LoadCategoriesAsync(CatalogViewModelCreate model)
        {
            var categories = await _categoryServiceClient.GetAllActiveCategoriesForClientAsync();
            model.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
        }

        private async Task LoadCategoriesAsync(CatalogViewModelEdit model)
        {
            var categories = await _categoryServiceClient.GetAllActiveCategoriesForClientAsync();
            model.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
        }
    }
}