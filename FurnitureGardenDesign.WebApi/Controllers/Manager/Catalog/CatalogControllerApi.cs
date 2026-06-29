using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurnitureGardenDesign.WebApi.Controllers.Areas.Manager.Catalog
{
    [Route("api/manager/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager")]
    public class CatalogControllerApi : ControllerBase
    {
        private readonly ICatalogManagementService _catalogManagementService;
        private readonly ICategoryServiceClient _categoryServiceClient;
        private readonly IFavoriteService _favoriteService;

        public CatalogControllerApi(
            ICatalogManagementService catalogManagementService,
            ICategoryServiceClient categoryServiceClient,
            IFavoriteService favoriteService)
        {
            _catalogManagementService = catalogManagementService;
            _categoryServiceClient = categoryServiceClient;
            _favoriteService = favoriteService;
        }

        [HttpGet("catalog-index")]
        public async Task<IActionResult> CatalogIndex(int page = 1, int pageSize = 9)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isGuest = !User.Identity!.IsAuthenticated;

            var designs = await _catalogManagementService.GetPublicCatalogAsync(userId, page, pageSize, isGuest);

            return Ok(designs);
        }

        [HttpGet("details")]
        public async Task<IActionResult> Details(Guid id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = await _catalogManagementService.GetDetailsAsync(id, userId);

            if (model == null)
            {
                return NotFound(new { message = "Design not found." });
            }

            return Ok(model);
        }

        [HttpPost("{id}/review")]
        [Authorize]
        public async Task<ActionResult> AddReview(Guid id, [FromBody] ReviewRequest request)
        {
            if (request.Rating < 1 || request.Rating > 5)
            {
                return BadRequest("Rating must be between 1 and 5.");
            }

            await _catalogManagementService.AddReviewAsync(
                User.Identity!.Name!,
                id,
                request.Rating,
                request.Comment
            );

            return Ok();
        }

        [HttpPost("{id}/favorite")]
        [Authorize]
        public async Task<ActionResult<bool>> ToggleFavorite(Guid id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            bool isNowFavorited = await _favoriteService.ToggleFavoriteAsync(userId, id);
            return Ok(isNowFavorited);
        }
    }

    public class ReviewRequest
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}