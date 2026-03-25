using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.WebApi.Controllers.Admin.Interactions
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReviewManagementControllerApi : ControllerBase
    {
        private readonly IReviewService _reviewService;
       
        private readonly IReviewManagementService _reviewManagementService;

        public ReviewManagementControllerApi(
            
            IReviewService reviewService,
            IReviewManagementService reviewManagementService)
          
        {
            _reviewService = reviewService;
            _reviewManagementService = reviewManagementService;
        }


        [HttpGet("write/{id}")]
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





        [HttpGet("reviews/{id}")]
        public async Task<IActionResult> Reviews(Guid id)
        {
            var reviews = await _reviewService.GetReviewsByDesignIdAsync(id);

            return Ok(reviews);

        }

       
        
        [HttpGet("list")]
        public async Task<IActionResult> EditList(bool includeDeleted = true)
        {
            IEnumerable<ReviewViewModelList> reviews;

            if (includeDeleted)
            {
                reviews = await _reviewManagementService.GetAllIncludingDeletedAsync();
              
            }
            else
            {
                reviews = await _reviewManagementService.GetAllActiveAsync();
               
            }

            return Ok(new
            {
                reviews = reviews.OrderByDescending(r => r.CreatedAt),
                showDeleted = includeDeleted
            });
        }

     

     
      
        [HttpPost("toggle/{id}")]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            try
            {
                await _reviewManagementService.ToggleReviewAsync(id);
                var review = await _reviewManagementService.GetByIdAsync(id);

                return Ok(new
                {

                    message = review.IsDeleted
                        ? "Review has been deactivated."
                        : "Review has been activated.",
                    isDeleted = review.IsDeleted
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error toggling review status: " + ex.Message });
            }
        }


       
      
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var review = await _reviewManagementService.GetByIdAsync(id);

            if (review == null)
            {

                return NotFound(new { error = "Review not found." });
            }

            return Ok(review);
        }

    }
}

