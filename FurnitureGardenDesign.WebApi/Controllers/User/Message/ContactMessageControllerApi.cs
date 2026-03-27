using FurnitureGardenDesign.Services.Core.Interfaces.Message;
using FurnitureGardenDesign.Web.ViewModels.Messages;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.WebApi.Controllers.User.Message
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactMessageControllerApi : ControllerBase
    {
        private readonly IContactMessageClientService _contactMessageService;

        public ContactMessageControllerApi
            (IContactMessageClientService contactMessageService)
        {
            _contactMessageService = contactMessageService;
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok(new ContactMessageCreateViewModel());
        }

        [HttpPost("index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactMessageCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _contactMessageService.SendContactMessageAsync(model, User);
            return Ok(new { message = "Your message has been sent successfully! We'll get back to you soon." });
        }
    }
}