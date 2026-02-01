using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.Controllers;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;






namespace FurnitureGardenDesign.Web.Controllers
{
    [Authorize]
    public class ReviewController : BaseController
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

        public async Task<IActionResult> Write(Guid id)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();


            if (await _reviewService.HasUserReviewedAsync(userId, id))
            {
                TempData["Error"] = "You have already reviewed this design.";
                return RedirectToAction("Index", "Catalog");
            }

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
                AverageRating = design.Reviews.Any() ? design.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = design.Reviews.Count
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post(Guid catalogDesignId, int rating, string? comment)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (await _reviewService.HasUserReviewedAsync(userId, catalogDesignId))
            {
                TempData["Error"] = "You have already reviewed this design.";
                return RedirectToAction("Index", "Catalog");
            }

            var reviewModel = new AddReviewViewModel
            {
                CatalogDesignId = catalogDesignId,
                Rating = rating,
                Comment = comment
            };

            await _reviewService.AddReviewAsync(userId, reviewModel);

            TempData["Success"] = "Review added successfully!";
            return RedirectToAction("Index", "Catalog");
        }


        [HttpGet]

        [HttpGet]
        public async Task<IActionResult> Reviews(Guid id)
        {
            var reviews = await _reviewService.GetReviewsByDesignIdAsync(id);

            var model = new ReviewListViewModel
            {
                Reviews = reviews.ToList()
            };

            return View(model);
        }










    }
}