using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Review;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.WebApi.Controllers.User.Interactions
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewControllerApi : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewControllerApi(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }





      
        [HttpGet("write")]
        public async Task<IActionResult> Write(Guid id)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { error = "You must be logged in to write a review." });

            var model = await _reviewService.GetWriteReviewModelAsync(userId, id);

            if (model == null)
            {
              
                return BadRequest(new { error = "You have already reviewed this design." });
            }

            return Ok(model);
        }

        private string? GetUserId() => User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

        [HttpPost("post")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post(AddReviewViewModel model)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _reviewService.CreateReviewAsync(userId, model);

            if (!result.Success)
            {
               
                return Unauthorized(new { error = "You must be logged in to add a review." });
            }

          
            return Ok(new { message = "Review added successfully." });
        }


     

        [HttpGet("reviews")]
        public async Task<IActionResult> Reviews(Guid id)
        {
            var reviews = await _reviewService.GetReviewsByDesignIdAsync(id);

            return Ok(reviews);

        }
    }
}