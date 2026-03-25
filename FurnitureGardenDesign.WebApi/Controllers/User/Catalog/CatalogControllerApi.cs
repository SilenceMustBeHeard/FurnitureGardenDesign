using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurnitureGardenDesign.WebApi.Controllers.User.Catalog
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogControllerApi : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        private readonly IFavoriteService _favoriteService;

        public CatalogControllerApi(ICatalogService catalogService,
            IFavoriteService favoriteService)
        {
            _catalogService = catalogService;
            _favoriteService = favoriteService;
        }



        
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CatalogDesignViewModel>>> GetCatalog(int page = 1, int pageSize = 9)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isGuest = !User.Identity?.IsAuthenticated ?? true;

            var designs = await _catalogService.GetPublicCatalogAsync(userId, page, pageSize, isGuest);

            if (isGuest)
            {
                designs = designs.Take(3);
            }

            return Ok(designs);
        }



        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CatalogDesignViewModel>> GetDetails(Guid id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _catalogService.GetDetailsAsync(id, userId);

            if (model == null)
            { 
                return NotFound();
            }

            return Ok(model);
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

        [HttpPost("{id}/review")]
        [Authorize]
        public async Task<ActionResult> AddReview(Guid id, [FromBody] ReviewRequest request)
        {
            if (request.Rating < 1 || request.Rating > 5)
            { 
                return BadRequest("Rating must be between 1 and 5."); 
            }

            await _catalogService.AddReviewAsync(
                User.Identity!.Name!,
                id,
                request.Rating,
                request.Comment
            );

            return Ok();
        }
    }

    public class ReviewRequest
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}