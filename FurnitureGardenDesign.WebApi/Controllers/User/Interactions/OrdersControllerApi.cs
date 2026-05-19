using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces.Catalog;
using FurnitureGardenDesign.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurnitureGardenDesign.WebApi.Controllers.User.Interactions;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersControllerApi : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ICategoryServiceClient _categoryServiceClient;

    public OrdersControllerApi(IOrderService orderService, ICategoryServiceClient categoryServiceClient)
    {
        _orderService = orderService;
        _categoryServiceClient = categoryServiceClient;
    }

 
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { error = "Invalid order data.", details = ModelState });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "You must be logged in to submit an order." });
        }

        await _orderService.CreateOrderAsync(userId, model);

        return Ok(new { message = "Your order has been submitted successfully!" });
    }
}