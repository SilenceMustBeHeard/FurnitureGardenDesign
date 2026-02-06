using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Web.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICategoryServiceClient _categoryServiceClient;

        public OrdersController(
            IOrderService orderService,
            ICategoryServiceClient categoryServiceClient)
        {
            _orderService = orderService;
            _categoryServiceClient = categoryServiceClient;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            return View(new OrderFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _orderService.CreateOrderAsync(userId, model);

            TempData["Success"] = "Your order has been submitted!";
            return RedirectToAction("Index", "Home");
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryServiceClient.GetAllActiveCategoriesForClientAsync();

            ViewBag.Categories = categories
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
                .ToList();
        }
    }
}
