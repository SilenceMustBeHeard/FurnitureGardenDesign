using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.WebApi.Controllers.Admin.Message
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactMessageControllerApi : ControllerBase
    {
        private readonly IContactMessageService _contactMessageService;
        private readonly UserManager<AppUser> _userManager;

        public ContactMessageControllerApi(
            IContactMessageService contactMessageService,
            UserManager<AppUser> userManager)
        {
            _contactMessageService = contactMessageService;
            _userManager = userManager;
        }

        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            var adminId = _userManager.GetUserId(User);
            var messages = await _contactMessageService.GetAdminMessagesAsync(adminId);
            return Ok(messages);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var adminId = _userManager.GetUserId(User);
            var message = await _contactMessageService.GetMessageDetailsAsync(id, adminId);

            if (message == null)
            {
               
                return NotFound(new {error = "Message not found."});
            }

            return Ok(message);
        }

        [HttpGet("respond/{id}")]
        public async Task<IActionResult> Respond(Guid id)
        {
            var adminId = _userManager.GetUserId(User);
            var message = await _contactMessageService.GetMessageDetailsAsync(id, adminId);

            if (message == null)
            {

                return NotFound(new { error = "Message not found." });
            }


            if (!string.IsNullOrEmpty(message.Response))
            {

                return BadRequest(new { error = "This message has already been responded to." });
            }

            var model = new ContactMessageResponseViewModel
            {
                Id = message.Id,
                Subject = message.Subject,
                SenderName = message.SenderName,
                SenderEmail = message.SenderEmail,
                OriginalMessage = message.Message,
                Response = string.Empty
            };

            return Ok(model);
        }

        [HttpPost("respond/{id}")]

        public async Task<IActionResult> Respond(Guid id, [FromBody] ContactMessageResponseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var adminId = _userManager.GetUserId(User);

            try
            {
                await _contactMessageService.RespondToMessageAsync(model.Id, model.Response, adminId);
                return Ok(new { message = "Response sent successfully!" });
            }
            catch (InvalidOperationException ex)
            {
               return BadRequest(new {error = ex.Message });
            }

           
        }
    }
}
    