using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Services.Core.Implementations.Catalog;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using FurnitureGardenDesign.WebApi.Controllers.User.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FurnitureGardenDesign.WebApi.Controllers.Admin.Catalog
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CatalogManagementControllerApi : ControllerBase
    {
        private readonly ICatalogManagementService _catalogManagementService;
        private readonly ICategoryServiceClient _categoryServiceClient;
        private readonly IFavoriteService _favoriteService;

        public CatalogManagementControllerApi(
            ICatalogManagementService catalogManagementService,
            ICategoryServiceClient categoryServiceClient,
            IFavoriteService favoriteService)
        {
            _catalogManagementService = catalogManagementService;
            _categoryServiceClient = categoryServiceClient;
            _favoriteService = favoriteService;
        }



        [HttpGet("list")]
      
        public async Task<IActionResult> EditList()
        {
            var catalogues = await _catalogManagementService.GetAllCataloguesForAdminAsync();
            return Ok(catalogues);
        }

        [HttpPost("toggle/{id}")]

        public async Task<IActionResult> ToggleActive(Guid id)
        {
            await _catalogManagementService.ToggleCatalogAsync(id);
          
            return Ok(new { message = "Catalogue status changed!" });
        }

        [HttpGet("create")]
      
        public async Task<IActionResult> Create()
        {
            var model = new CatalogViewModelCreate();
            await LoadCategoriesAsync(model);
            return Ok(model);
        }

      
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CatalogViewModelCreate model)
        {
            if (!ModelState.IsValid)
            {
               return BadRequest(ModelState);
            }

            await _catalogManagementService.AddCatalogAsync(model);
          
            return Ok(new {message = "Catalogue created successfully!"});
        }

      
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _catalogManagementService.GetCatalogForEditByIdAsync(id);
            if (model == null)
            { return NotFound(new {error = "Design Not Found!"}); }

            await LoadCategoriesAsync(model);
            return Ok(model);
        }
            
      
      
        [HttpPost("edit")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] CatalogViewModelEdit model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _catalogManagementService.EditCatalogAsync(model.Id, model);
          
            return Ok(new {messagte = "Catalog Design edited successfully!"});
        }

        [HttpGet("admin-index")]
       
        public async Task<IActionResult> AdminIndex(int page = 1, int pageSize = 9)
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
               
                return NotFound(new { message = "Design not found."});
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
    public class ReviewRequest
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
    
