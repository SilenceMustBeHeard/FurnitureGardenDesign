using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces.Catalog;
using FurnitureGardenDesign.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurnitureGardenDesign.WebApi.Controllers.User.Interactions
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersControllerApi : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICategoryServiceClient _categoryServiceClient;

        public OrdersControllerApi(
            IOrderService orderService,
            ICategoryServiceClient categoryServiceClient)
        {
            _orderService = orderService;
            _categoryServiceClient = categoryServiceClient;
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            return Ok(new OrderFormViewModel());
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(OrderFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (User.Identity?.IsAuthenticated != true)
            {
                return Unauthorized(new { error = "You must be logged in to submit an order." });
            }
            await _orderService.CreateOrderAsync(userId, model);

            return Ok(new { success = "Your order has been submitted!" });
        }
    }
}