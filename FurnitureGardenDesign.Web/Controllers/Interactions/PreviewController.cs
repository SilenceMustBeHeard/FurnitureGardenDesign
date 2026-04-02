using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Controllers.Interactions
{

   
  
    [Authorize]
    public class PreviewController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> FetchWebpageImage(string url)
        {
            try
            {
                // Basic validation
        

                using var httpClient = new HttpClient();
                // Set user agent to mimic a browser (some sites block requests without it)
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                var html = await httpClient.GetStringAsync(url);

                // Try to find Open Graph image (best for social sharing)
                var ogImageMatch = System.Text.RegularExpressions.Regex.Match(html,
                    @"<meta\s+property=[""]og:image[""]\s+content=[""]([^""]*)[""]",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);


                // If found, return absolute URL of the image
                if (ogImageMatch.Success)
                {
                    var imageUrl = ogImageMatch.Groups[1].Value;
                    if (!imageUrl.StartsWith("http"))
                    {
                        var baseUri = new Uri(url);
                        imageUrl = new Uri(baseUri, imageUrl).ToString();
                    }
                    return Json(new { success = true, imageUrl = imageUrl });
                }

                // Fallback to first large image
                var imgMatch = System.Text.RegularExpressions.Regex.Match(html,
                    @"<img[^>]*src=[""]([^""]*)[""][^>]*>",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                
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
