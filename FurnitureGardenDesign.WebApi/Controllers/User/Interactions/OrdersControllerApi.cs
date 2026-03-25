using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces.Catalog;
using FurnitureGardenDesign.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        [ValidateAntiForgeryToken]
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

       

      

        [HttpGet("fetch-webpage-image")]
        public async Task<IActionResult> FetchWebpageImage(string url)
        {
            try
            {
               
                using var httpClient = new HttpClient();
              
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)" +
                    " AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");


               
                var html = await httpClient.GetStringAsync(url);

               
                var ogImageMatch = System.Text.RegularExpressions.Regex.Match(html,
                    @"<meta\s+property=[""]og:image[""]\s+content=[""]([^""]*)[""]",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);


              
                if (ogImageMatch.Success)
                {
                  
                    var imageUrl = ogImageMatch.Groups[1].Value;
                    if (!imageUrl.StartsWith("http"))
                    {
                       
                        var baseUri = new Uri(url);
                      
                        imageUrl = new Uri(baseUri, imageUrl).ToString();
                    }
                
                    return Ok(new { success = true, imageUrl = imageUrl });
                }

              
                var imgMatch = System.Text.RegularExpressions.Regex.Match(html,
                    @"<img[^>]*src=[""]([^""]*)[""][^>]*>",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
              
                if (imgMatch.Success)
                {

                    var imageUrl = imgMatch.Groups[1].Value;
                    return Ok(new { success = true, imageUrl = imageUrl });
                }

                return Ok(new { success = false, message = "No image found on page" });
            }
            catch
            {
                return Ok(new { success = false, message = "Could not fetch webpage" });
            }
        }








    }
}
