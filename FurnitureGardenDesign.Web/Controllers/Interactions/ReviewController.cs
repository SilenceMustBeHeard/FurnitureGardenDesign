using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.Controllers.Account;
using FurnitureGardenDesign.Web.ViewModels.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Controllers.Interactions;

[Authorize]
public class ReviewController : BaseController
{
    private readonly IReviewService _reviewService;

    public ReviewController(
        UserManager<AppUser> userManager,
        IReviewService reviewService)
        : base(userManager)
    {
        _reviewService = reviewService;
    }

    // gets the form for writing a review for a specific catalog design
    // checks if the user is authorized and if they have already reviewed the design
    // and returns the appropriate view or redirects with an error message
    [HttpGet]
    public async Task<IActionResult> Write(Guid id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var model = await _reviewService.GetWriteReviewModelAsync(userId, id);

        if (model == null)
        {
            TempData["Error"] = "You have already reviewed this design.";
            return RedirectToAction("CatalogIndex", "Catalog");
        }

        return View(model);
    }

    // posts the review form data to create a new review for a specific catalog design
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Post(AddReviewViewModel model)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _reviewService.CreateReviewAsync(userId, model);

        if (!result.Success)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction("CatalogIndex", "Catalog");
        }

        TempData["Success"] = "Review added successfully!";
        return RedirectToAction("CatalogIndex", "Catalog");
    }

    // gets the reviews for a specific catalog design and returns the view with the reviews list

    [HttpGet]
    public async Task<IActionResult> Reviews(Guid id)
    {
        var reviews = await _reviewService.GetReviewsByDesignIdAsync(id);

        return View(new ReviewListViewModel
        {
            Reviews = reviews.ToList()
        });
    }
}