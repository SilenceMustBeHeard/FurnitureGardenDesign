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

        public OrdersController(
            IOrderService orderService,
            ICategoryServiceClient categoryServiceClient)
        {
            _orderService = orderService;
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
            // Fetch categories from the service
            var categories = await _categoryServiceClient.GetAllActiveCategoriesForClientAsync();

            // Map categories to SelectListItem for the dropdown
            ViewBag.Categories = categories
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
                .ToList();
        }

        [HttpGet]
        public async Task<IActionResult> FetchWebpageImage(string url)
        {
            try
            {
                // Use HttpClient to fetch the webpage content
                using var httpClient = new HttpClient();
                // Set a user agent to avoid being blocked by some websites
                // mimic a common browser user agent to increase chances of getting the content
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)" +
                    " AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");


                // Fetch the HTML content of the page
                var html = await httpClient.GetStringAsync(url);

                // Try to find Open Graph image (best for social sharing)
                var ogImageMatch = System.Text.RegularExpressions.Regex.Match(html,
                    @"<meta\s+property=[""]og:image[""]\s+content=[""]([^""]*)[""]",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);


                // If found, return the OG image URL
                if (ogImageMatch.Success)
                {
                    // Handle relative URLs by converting them to absolute
                    var imageUrl = ogImageMatch.Groups[1].Value;
                    if (!imageUrl.StartsWith("http"))
                    {
                        // Convert relative URL to absolute using the base URL
                        var baseUri = new Uri(url);
                        // Combine base URL with relative image URL
                        imageUrl = new Uri(baseUri, imageUrl).ToString();
                    }
                    // Return the image URL as JSON
                    return Json(new { success = true, imageUrl = imageUrl });
                }

                // Fallback to first large image
                // regex to find all img tags and extract their src attributes
                var imgMatch = System.Text.RegularExpressions.Regex.Match(html,
                    @"<img[^>]*src=[""]([^""]*)[""][^>]*>",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // If found, return the first image URL
                if (imgMatch.Success)
                {
                    
                    var imageUrl = imgMatch.Groups[1].Value;
                    return Json(new { success = true, imageUrl = imageUrl });
                }

                return Json(new { success = false, message = "No image found on page" });
            }
            catch
            {
                return Json(new { success = false, message = "Could not fetch webpage" });
            }
        }








    }
}
