using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Interfaces.Account;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.WebApi.Controllers.Areas.Admin.Message
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SystemMessageControllerApi : ControllerBase
    {
        private readonly ISystemInboxMessageService _systemMessageService;
        private readonly IAppUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;

        public SystemMessageControllerApi(
            ISystemInboxMessageService systemMessageService,
            IAppUserRepository userRepository,
            UserManager<AppUser> userManager)
        {
            _systemMessageService = systemMessageService;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            var adminId = _userManager.GetUserId(User);
            var messages = await _systemMessageService.GetAdminMessagesAsync(adminId);
            return Ok(messages);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create(string? userId = null)
        {
            var availableUsers = await _userRepository
                .GetAllAttached()
                .Select(u => new
                {
                    u.Id,
                    FullName = u.FullName,
                    u.Email
                })
                .ToListAsync();

            if (!string.IsNullOrEmpty(userId) && !availableUsers.Any(u => u.Id == userId))
            {
                return BadRequest(new { error = "User not found." });
            }

            return Ok(new
            {
                ReceiverId = userId,
                AvailableUsers = availableUsers,
                ReceiverName = availableUsers.FirstOrDefault(u => u.Id == userId)!.FullName
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] SystemInboxMessageCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var receiver = await _userManager.FindByIdAsync(model.ReceiverId ?? "");

            if (receiver == null)
            {
                return BadRequest(new { error = "Receiver not found." });
            }

            var adminId = _userManager.GetUserId(User);

            var message = new SystemInboxMessage
            {
                Id = Guid.NewGuid(),
                Description = model.Description,
                ReceiverId = model.ReceiverId!,
                SenderId = adminId,
                Type = model.Type,
                IsRead = false,
                CreatedOn = DateTime.UtcNow
            };

            await _systemMessageService.CreateMessageAsync(message);

            return Ok(new { message = "Message sent successfully!", id = message.Id });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var adminId = _userManager.GetUserId(User);
            var message = await _systemMessageService.GetMessageDetailsAsync(id, adminId);

            if (message == null)
            {
                return NotFound(new { message = "Message not found or you don't have permission to view it." });
            }

            var sender = await _userManager.FindByIdAsync(message.SenderId ?? "");
            var receiver = await _userManager.FindByIdAsync(message.ReceiverId!);

            message.SenderName = sender != null ? $"{sender.FullName}" : "System";
            message.ReceiverName = receiver != null ? $"{receiver.FullName}" : "Unknown";

            return Ok(message);
        }
    }
}