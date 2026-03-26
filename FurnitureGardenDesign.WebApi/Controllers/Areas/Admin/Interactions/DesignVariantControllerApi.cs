using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.DesignVariants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.WebApi.Controllers.Areas.Admin.Interactions
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DesignVariantControllerApi : ControllerBase
    {
       
            private readonly IAdminDesignVariantService _designVariantService;

            public DesignVariantControllerApi(IAdminDesignVariantService designVariantService)
            {
                _designVariantService = designVariantService;
            }



             [HttpGet("create/{orderId}")]
            public IActionResult Create(Guid orderId)
            {
                var model = new DesignVariantViewModel
                {
                    OrderId = orderId
                };

                return Ok(model);
            }

           
            [HttpPost("create")]
            public async Task<IActionResult> Create(DesignVariantViewModel model)
            {
                if (!ModelState.IsValid)
                {
                   return BadRequest(ModelState);
            }


                var entity = await _designVariantService.CreateDesignVariantAsync(model);
               

                return Ok(new { Id = entity.Id });
            }




           [HttpPost("send/{designVariantId}")]
            public async Task<IActionResult> Send(Guid designVariantId)
            {
                try
                {
                    await _designVariantService.SendDesignVariantProposalAsync(designVariantId);

                   
                    return Ok(new { message = "Design variant proposal sent to client successfully." });
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(new { message = ex.Message });
                }
                
            }


          


            
            [HttpGet("{id}")]
            public async Task<IActionResult> Details(Guid id)
            {
                var variant = await _designVariantService.GetDesignVariantByIdAsync(id);

                if (variant == null)
                {
                   return NotFound(new { message = "Design variant not found." });
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

                return Ok(model);
            }




        
    }
}