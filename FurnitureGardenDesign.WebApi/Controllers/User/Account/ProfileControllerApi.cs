using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces.Account;
using FurnitureGardenDesign.Services.Core.Interfaces.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.WebApi.Controllers.User;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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

    
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { error = "You must be logged in to access this resource." });
        }

        var model = await _profileService.GetProfileAsync(user.Id);
        return Ok(model);
    }


    [HttpPut("inbox/{id}/read")]
    public async Task<IActionResult> MarkMessageAsRead(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { error = "You must be logged in to perform this action." });
        }

        await _inboxMessageService.MarkMessageAsReadAsync(id, user.Id);
        return Ok(new { message = "Message marked as read successfully." });
    }

    [HttpGet("inbox/{id}")]
    public async Task<IActionResult> GetMessageDetails(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { error = "You must be logged in to perform this action." });
        }

        var viewModel = await _inboxMessageService.GetMessageDetailsAsync(id, user.Id);
        if (viewModel == null)
        {
            return NotFound(new { error = "Message not found or you do not have permission to view it." });
        }

        return Ok(viewModel);
    }

   
    [HttpPut("inbox/{id}/approve-design")]
    public async Task<IActionResult> ApproveDesign(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { error = "You must be logged in to perform this action." });
        }

        var updatedMessage = await _inboxMessageService.ApproveDesignAsync(id, user.Id);
        if (updatedMessage == null)
        {
            return NotFound(new { error = "Unable to approve design. Message not found or you don't have permission." });
        }

        return Ok(new { message = "Design approved successfully!" });
    }

  
    [HttpGet("system-messages/{id}")]
    public async Task<IActionResult> GetSystemMessageDetails(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { error = "You must be logged in to perform this action." });
        }

        var viewModel = await _systemInboxMessageService.GetMessageDetailsAsync(id, user.Id);
        if (viewModel == null)
        {
            return NotFound(new { error = "Message not found or you do not have permission to view it." });
        }

        return Ok(viewModel);
    }

    
    [HttpGet("contact-messages/{id}")]
    public async Task<IActionResult> GetContactMessageDetails(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { error = "You must be logged in to perform this action." });
        }

        var viewModel = await _contactMessageClientService.GetMessageDetailsAsync(id, user.Id);
        if (viewModel == null)
        {
            return NotFound(new { error = "Message not found or you do not have permission to view it." });
        }

        return Ok(viewModel);
    }

    
    [HttpGet("admin/inbox")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAdminInbox()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { error = "You must be logged in to perform this action." });
        }

        var messages = await _inboxMessageService.GetAdminMessagesAsync(user.Id);
        return Ok(messages);
    }
}