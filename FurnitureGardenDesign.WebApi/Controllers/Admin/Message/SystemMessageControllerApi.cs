using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Interfaces.Account;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Messages;
using FurnitureGardenDesign.Web.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.WebApi.Controllers.Admin.Message
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
            var model = new SystemInboxMessageCreateViewModel
            {
                ReceiverId = userId,
                AvailableUsers = await _userRepository
                    .GetAllAttached()
                    .Select(u => new UserSelectViewModel
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Email = u.Email ?? string.Empty
                    })
                    .ToListAsync()
            };

            if (!string.IsNullOrEmpty(userId))
            {
                var selectedUser = model.AvailableUsers.FirstOrDefault(u => u.Id == userId);
                if (selectedUser != null)
                {
                    model.ReceiverName = selectedUser.FullName;
                }
            }

            return Ok(model);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] SystemInboxMessageCreateViewModel model)
        {
            
            model.AvailableUsers = await _userRepository
                .GetAllAttached()
                .Select(u => new UserSelectViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? string.Empty
                })
                .ToListAsync();

            if (!string.IsNullOrEmpty(model.ReceiverId))
            {
                var selectedUser = model.AvailableUsers.FirstOrDefault(u => u.Id == model.ReceiverId);
                if (selectedUser != null)
                {
                    model.ReceiverName = selectedUser.FullName;
                }
            }

            if (!ModelState.IsValid)
            { 
                return BadRequest(ModelState); 
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

            return Ok(new { message = "Message sent successfully!" });
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