using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.DesignVariants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurnitureGardenDesign.Web.Areas.Admin.Controllers.Interactions
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DesignVariantsController : Controller
    {
        private readonly IAdminDesignVariantService _designVariantService;

        public DesignVariantsController(IAdminDesignVariantService designVariantService)
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

            if(!ModelState.IsValid) {
                TempData["Error"] = "There was an error preparing the form for creating a design variant.";
                return RedirectToAction("Details", "Orders", new { id = orderId });
            }

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

            if(!ModelState.IsValid) {
                TempData["Error"] = "There was an error sending the design variant proposal.";
                return RedirectToAction("Details", new { id = designVariantId });
            }

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

        private static string GetContentType(string url)
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
            if(id == Guid.Empty)
            {
                TempData["Error"] = "Invalid design variant ID.";
                return RedirectToAction("Index", "Home");
            }
            var variant = await _designVariantService.GetDesignVariantByIdAsync(id);

            if(variant == null)
            {
                TempData["Error"] = "Design variant not found.";
                return RedirectToAction("Details", new { id = id });
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

            if(!ModelState.IsValid) {
                TempData["Error"] = "There was an error loading the design variant details.";
                return RedirectToAction("Details", new { id = id });
            }


            return View(model);
        }




    }
}
