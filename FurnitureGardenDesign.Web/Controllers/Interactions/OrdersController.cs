using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using FurnitureGardenDesign.Services.Core.Interfaces.Catalog;

namespace FurnitureGardenDesign.Web.Controllers.Interactions
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICategoryServiceClient _categoryServiceClient;
        private readonly IPreviewService _previewService;
        public OrdersController(
            IOrderService orderService,
            ICategoryServiceClient categoryServiceClient,
            IPreviewService previewService)
        {
            _orderService = orderService;
            _categoryServiceClient = categoryServiceClient;
            _previewService = previewService;
            _categoryServiceClient = categoryServiceClient;
        }





        // show order form for submission
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            return View(new OrderFormViewModel());
        }


        // create order (submit)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors in the form.";
                await LoadCategoriesAsync();
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if(User.Identity?.IsAuthenticated != true)
            {
                TempData["Error"] = "You must be logged in to submit an order.";
                return RedirectToAction("Login", "Account");
            }
            await _orderService.CreateOrderAsync(userId, model);

            TempData["Success"] = "Your order has been submitted!";
            return RedirectToAction("Index", "Home");
        }

        // load categories for the dropdown

        [HttpPost]
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

        // fetch image for preview 
        [HttpGet]
        public async Task<IActionResult> FetchWebpageImage(string url)
        {
            var (success, imageUrl, message) = await _previewService.FetchWebpageImageAsync(url);

            if (success && imageUrl != null)
            {
                return Json(new { success = true, imageUrl });
            }

            return Json(new { success = false, message = message ?? "Could not fetch image" });
        }
    }








    
}
