using Furniture_GardenDesign.Data.Enums;
using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Implementations;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace FurnitureGardenDesign.Web.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;


        public OrdersController(
            IOrderService orderService,
            ICategoryService categoryService)
        {
            _orderService = orderService;
            _categoryService = categoryService;
        }

        // GET: Orders/Create
       
        public async Task<IActionResult> Create()
        {
           
            await LoadCategoriesAsync();
            return View(new OrderFormViewModel());
        }



        // POST : Orders
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

            TempData["SuccessMessage"] = "Your order has been submitted!";

            return RedirectToAction("Index", "Home");

        }



        // GET: Orders/Manage

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Manage()
        {
            var orders = await _orderService.GetPendingOrdersAsync();
            return View(orders);
        }


        // Helper

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




    }
}