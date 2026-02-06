
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FurnitureGardenDesign.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersManagementController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;


        public OrdersManagementController(
            IOrderService orderService,
            ICategoryService categoryService)
        {
            _orderService = orderService;
            _categoryService = categoryService;
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return View(model); 
            }


            await _orderService.CreateOrderAsync(userId, model);

            TempData["Success"] = "Your order has been submitted!";

            return RedirectToAction("Index", "Home");

        }




       
        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var orders = await _orderService.GetPendingOrdersAsync();
            return View(orders);
        }


        [HttpPost]
      
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id)
        {
            var result = await _orderService.RejectOrderAsync(id);

            TempData[result ? "Success" : "Error"] =
                result ? "Order has been rejected." : "Failed to reject order.";

            return RedirectToAction(nameof(Manage));
        }



        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryService.GetAllActiveCategoriesAsync();

            ViewBag.Categories = categories
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
                .ToList();
        }   


        public async Task<IActionResult> Details(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


    }
}