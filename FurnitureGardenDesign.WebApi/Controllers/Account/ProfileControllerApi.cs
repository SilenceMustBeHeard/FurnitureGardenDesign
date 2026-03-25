using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces.Account;
using FurnitureGardenDesign.Services.Core.Interfaces.Message;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.WebApi.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileControllerApi : ControllerBase
    {

        private readonly IProfileService _profileService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IInboxMessageService _inboxMessageService;
        private readonly ISystemInboxMessageService _systemInboxMessageService;
        private readonly IContactMessageClientService _contactMessageClientService;

        public ProfileControllerApi(
            IContactMessageClientService contactMessageClientService,
            ISystemInboxMessageService systemInboxMessageService,
            IProfileService profileService,
            UserManager<AppUser> userManager,
            IInboxMessageService inboxMessageService)
        {
            _contactMessageClientService = contactMessageClientService;
            _systemInboxMessageService = systemInboxMessageService;
            _profileService = profileService;
            _userManager = userManager;
            _inboxMessageService = inboxMessageService;
        }

        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
              
                return BadRequest(new { error = "You must be logged in to access this resource." });

            }

            var model = await _profileService.GetProfileAsync(user.Id);
            return Ok(model);
        }

        [HttpPost("mark-as-read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                
                return BadRequest(new { error = "You must be logged in to perform this action." });

            }

            await _inboxMessageService.MarkMessageAsReadAsync(id, user.Id);
            return Ok(new { success = "Message marked as read successfully." });
        }

        [HttpGet("proxy-image")]
        public async Task<IActionResult> ProxyImage(string url)
        {
            using var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(url);
            var contentType = GetContentType(url);
            return File(bytes, contentType);
        }

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

        [HttpGet("message-details")]
        public async Task<IActionResult> MessageDetails(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
               
                return BadRequest(new { error = "You must be logged in to perform this action." });

            }

            var viewModel = await _inboxMessageService.GetMessageDetailsAsync(id, user.Id);

            if (viewModel == null)
            {
             
                return NotFound(new { error = "Message not found or you do not have permission to view it." });
            }

            return Ok(viewModel);
        }

        [HttpPost("approve-design")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveDesign(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest(new { error = "You must be logged in to perform this action." });
            }

            var updatedMessage = await _inboxMessageService.ApproveDesignAsync(id, user.Id);

            if (updatedMessage == null)
            {
                return NotFound(new { error = "Unable to approve design. Message not found or you don't have permission." });
            }

            return Ok(new { success = "Design approved successfully!" });
        }

        [HttpGet("system-message-details")]
        public async Task<IActionResult> SystemMessageDetails(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest(new { error = "You must be logged in to perform this action." });
            }

            var viewModel = await _systemInboxMessageService.GetMessageDetailsAsync(id, user.Id);

            if (viewModel == null)
            {
                return NotFound(new { error = "Message not found or you do not have permission to view it." });
            }

            return Ok(viewModel);
        }

        [HttpGet("contact-message-details")]
        public async Task<IActionResult> ContactMessageDetails(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest(new { error = "You must be logged in to perform this action." });
            }


            var viewModel = await _contactMessageClientService.GetMessageDetailsAsync(id, user.Id);

            if (viewModel == null)
            {
                return NotFound(new { error = "Message not found or you do not have permission to view it." });
            }

            return Ok(viewModel);
        }

        [HttpGet("admin-inbox")]
        public async Task<IActionResult> AdminInbox()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest(new { error = "You must be logged in to perform this action." });
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isManager = await _userManager.IsInRoleAsync(user, "Manager");

            if (!isAdmin && !isManager)
            {
                return Forbid();
            }

            var messages = await _inboxMessageService.GetAdminMessagesAsync(user.Id);

            return Ok(messages);
        }
    }
}