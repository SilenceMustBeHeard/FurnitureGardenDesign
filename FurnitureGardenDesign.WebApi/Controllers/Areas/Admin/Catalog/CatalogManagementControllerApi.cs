using FurnitureGardenDesign.Api.Common;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurnitureGardenDesign.WebApi.Controllers.Areas.Admin.Catalog;

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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CatalogViewModelCreate model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
        }

        await _catalogManagementService.AddCatalogAsync(model);

        return Ok(new { message = "Catalogue created successfully!" });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetForEdit(Guid id)
    {
        var model = await _catalogManagementService.GetCatalogForEditByIdAsync(id);
        if (model == null)
        {
            return NotFound(new { message = "Design not found." });
        }
        return Ok(model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] CatalogViewModelEdit model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
        }

        if (id != model.Id)
        {
            return BadRequest(new { message = "ID mismatch." });
        }

        await _catalogManagementService.EditCatalogAsync(model.Id, model);
        return Ok(new { message = "Catalog Design edited successfully!" });
    }

    [HttpGet("admin-index")]
    public async Task<IActionResult> AdminIndex(int page = 1, int pageSize = 9)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        bool isGuest = !User.Identity!.IsAuthenticated;

        var designs = await _catalogManagementService.GetPublicCatalogAsync(userId, page, pageSize, isGuest);

        Response.Headers.Append("X-Total-Count", (await _catalogManagementService.GetTotalActiveDesignsAsync()).ToString());
        Response.Headers.Append("X-Current-Page", page.ToString());
        Response.Headers.Append("X-Page-Size", pageSize.ToString());
        Response.Headers.Append("X-Is-Guest", isGuest.ToString());

        return Ok(designs);
    }

    [HttpGet("{id}/details")]
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

        return Ok(new { message = "Review added successfully!" });
    }

    [HttpPost("{id}/favorite")]
    public async Task<ActionResult<bool>> ToggleFavorite(Guid id)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "User not authenticated." });
        }

        bool isNowFavorited = await _favoriteService.ToggleFavoriteAsync(userId, id);
        return Ok
            (
            new
            {
                message = isNowFavorited ? "Design added to favorites!" : "Design removed from favorites!",
                isFavorited = isNowFavorited
            }
            );
    }
}