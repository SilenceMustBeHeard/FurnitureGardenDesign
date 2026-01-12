using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Implementations;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Controllers
{


    [Authorize]
    public class ReviewController :
        BaseController
    {
        private readonly IReviewService _reviewService;
        private readonly ICatalogService _catalogService;

        public ReviewController(
      UserManager<AppUser> userManager,
      IReviewService reviewService,
      ICatalogService catalogService)
      : base(userManager)
        {
            _reviewService = reviewService;
            _catalogService = catalogService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var designs = await _catalogService.GetAllActiveAsync();

            var model = designs.Select(d => new CatalogDesignViewModel
            {
                Id = d.Id,
                Title = d.Title,
                Description = d.Description,
                ImageUrl = d.ImageUrl,
                Price = d.Price,
                AverageRating = d.Reviews.Any()
                ? d.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = d.Reviews.Count
            }).ToList();

            return View(model);
        }


        [HttpPost]

        public async Task<IActionResult> Post(Guid catalogDesignId, int rating, string? comment)
        {
            if (rating < 0 || rating > 5)
                return RedirectToAction(nameof(Index));
            var catalogDesign = await _catalogService.GetByIdAsync(catalogDesignId);
            if (catalogDesign == null)
            {
                TempData["Error"] = "Catalog design not found!";
                return RedirectToAction(nameof(Index));
            }

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var reviewModel = new AddReviewViewModel
            {
                CatalogDesignId = catalogDesignId,
                Rating = rating,
                Comment = comment
            };

            await _reviewService.AddReviewAsync(userId, reviewModel);

            TempData["Success"] = "Review added successfully!";
            return RedirectToAction(nameof(Index));

        }







    }
}
