using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.WebApi.Controllers.User.Interactions;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReviewControllerApi : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewControllerApi(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("{id}/eligible")]
    public async Task<IActionResult> CanWriteReview(Guid id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { error = "You must be logged in to write a review." });

        var model = await _reviewService.GetWriteReviewModelAsync(userId, id);

        if (model == null)
        
           return Ok(new {canReview = false, message = "You cannot review this design." });


        

        return Ok(new { canReview = true, message = "Thank you for your review." });

    }

    

    [HttpPost("{id}/reviews")]
  
    public async Task<IActionResult> CreateReview(Guid id, [FromBody] AddReviewViewModel model)
    {

        if(model == null || model.Rating < 1 || model.Rating > 5)
        {
            return BadRequest(new { error = "Invalid review data. Please provide a rating between 1 and 5." });
        }   

        var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "You must be logged in to add a review." });
        }

        model.CatalogDesignId = id;

        var result = await _reviewService.CreateReviewAsync(userId, model);

        if (!result.Success)
        {
            return Unauthorized(new { error = "You must be logged in to add a review." });
        }





        return CreatedAtAction(nameof(GetReviews), new { id },
       new { message = "Review added successfully." });
    }

    [HttpGet("{id}/reviews")]
    public async Task<IActionResult> GetReviews(Guid id)
    {
        var reviews = await _reviewService.GetReviewsByDesignIdAsync(id);

        return Ok(reviews);
    }

    private string? GetUserId()
    {
       
        return User.Claims.FirstOrDefault(c => c.Type == "id")?.Value
            ?? User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
    }
}
