using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FurnitureGardenDesign.WebApi.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class OrderManagementControllerApi : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;

        public OrderManagementControllerApi(
            IOrderService orderService,
            ICategoryService categoryService)
        {
            _orderService = orderService;
            _categoryService = categoryService;
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllActiveCategoriesAsync();
            return Ok(new OrderFormViewModel
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] OrderFormViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _orderService.CreateOrderAsync(userId, model);
            return Ok(new { message = "Order created successfully." });
        }

        [HttpGet("manage")]
        public async Task<IActionResult> Manage()
        {
            var orders = await _orderService.GetPendingOrdersAsync();
            return Ok(orders);
        }

        [HttpPost("reject/{id}")]
        public async Task<IActionResult> Reject(Guid id)
        {
            var result = await _orderService.RejectOrderAsync(id);

            if (result)
                return Ok(new { message = "Order has been rejected." });

            return BadRequest(new { message = "Failed to reject order." });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);

            if (order == null)
                return NotFound(new { message = "Order not found." });

            return Ok(order);
        }
    }
}