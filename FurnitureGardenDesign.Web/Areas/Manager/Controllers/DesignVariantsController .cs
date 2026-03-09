using FurnitureGardenDesign.Services.Core.Manager.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.DesignVariants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Areas.Manager.Controllers
{

    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class DesignVariantsController : Controller
    {
        private readonly IDesignVariantService _designVariantService;

        public DesignVariantsController(IDesignVariantService designVariantService)
        {
            _designVariantService = designVariantService;
        }


        // a GET action to display the form for creating a new design variant for a specific order
        [HttpGet]
        public IActionResult Create(Guid orderId)
        {
            var model = new DesignVariantViewModel
            {
                OrderId = orderId
            };

            return View(model);
        }

        // creates a new design variant for a specific order,
        // and redirects to the details page of the newly created variant
        // IMPORTANT - this action only creates the variant with the basic information (orderId)
        // the designer can then edit the variant to add more details like images, notes, etc.
        [HttpPost]
        public async Task<IActionResult> Create(DesignVariantViewModel model)
        {
            if (!ModelState.IsValid)
              {
                TempData["Error"] = "Please correct the errors in the form.";
                return View(model); 
            }

            
            var entity = await _designVariantService.CreateDesignVariantAsync(model);
            TempData["Success"] = "Design variant created successfully. You can now edit it to add more details.";

            return RedirectToAction("Details", new { id = entity.Id });
        }



        // sends a design variant proposal to the client for approval
        [HttpPost]
        public async Task<IActionResult> Send(Guid designVariantId)
        {
            try
            {
                await _designVariantService.SendDesignVariantProposalAsync(designVariantId);

                TempData["Success"] = "Design proposal sent successfully.";
                return RedirectToAction("Index", "Home");
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Details", new { id = designVariantId });
            }
        }


        // creates a proxy endpoint to fetch images from external URLs,
        // to avoid CORS(cross-origin resource sharing) issues when displaying them in the views
        [HttpGet]
        public async Task<IActionResult> ProxyImage(string url)
        {
            using var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(url);
            var contentType = GetContentType(url);
            return File(bytes, contentType);
        }


        // Get the content type based on the file extension
        // currently supports common image formats, but can be extended as needed

        private string GetContentType(string url)
        {
            var ext = Path.GetExtension(url).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }



        // gets the details of a design variant
        // including the associated order information, to display in the details view
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var variant = await _designVariantService.GetDesignVariantByIdAsync(id);

            if(variant == null)
            {
                TempData["Error"] = "Design variant not found.";
                return RedirectToAction("Index", "Home");
            }

            var model = new DesignVariantViewModel
            {
                Id = variant.Id,
                OrderId = variant.OrderId,
                Image2DUrl = variant.Image2DUrl,
                Model3DUrl = variant.Model3DUrl,
                Notes = variant.Notes,
                IsApproved = variant.IsApproved,

                OrderDescription = variant.Order.Description,
                OrderDimensions = variant.Order.Dimensions,
                ReferenceImageUrl = variant.Order.ReferenceImageUrl
            };

            return View(model);
        }




    }
}
