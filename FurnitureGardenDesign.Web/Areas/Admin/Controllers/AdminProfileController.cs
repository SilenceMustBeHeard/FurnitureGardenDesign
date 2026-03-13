using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Implementations;
using FurnitureGardenDesign.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IInboxMessageService _inboxMessageService;
        private readonly ISystemInboxMessageService _systemInboxMessageService;



        public AdminProfileController(
            ISystemInboxMessageService systemInboxMessageService,
            IProfileService profileService,
            UserManager<AppUser> userManager,
            IInboxMessageService inboxMessageService)
        {
            _systemInboxMessageService = systemInboxMessageService;
            _profileService = profileService;
            _userManager = userManager;
            _inboxMessageService = inboxMessageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "You must be logged in to perform this action.";
                return RedirectToAction("Login", "Account");

            }

            var model = await _profileService.GetProfileAsync(user.Id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "You must be logged in to perform this action.";
                return RedirectToAction("Login", "Account");

            }

            await _inboxMessageService.MarkMessageAsReadAsync(id, user.Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
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

        [HttpGet]
        public async Task<IActionResult> MessageDetails(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "You must be logged in to perform this action.";
                return RedirectToAction("Login", "Account");

            }

            var viewModel = await _inboxMessageService.GetMessageDetailsAsync(id, user.Id);

            if (viewModel == null)
            {
                TempData["Error"] = "Message not found or you do not have permission to view it.";
                return NotFound();
            }

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> SystemInbox()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "You must be logged in to perform this action.";
                return RedirectToAction("Login", "Account");
            }

            var messages = await _systemInboxMessageService.GetAdminMessagesAsync(user.Id);
            return View(messages);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveDesign(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "You must be logged in to perform this action.";
                return RedirectToAction("Login", "Account");
            }

            var updatedMessage = await _inboxMessageService.ApproveDesignAsync(id, user.Id);

            if (updatedMessage == null)
            {
                TempData["Error"] = "Unable to approve design. Message not found or you don't have permission.";
                return NotFound();
            }

            TempData["Success"] = "Design approved successfully!";
            return RedirectToAction(nameof(MessageDetails), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> SystemMessageDetails(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "You must be logged in to perform this action.";
                return RedirectToAction("Login", "Account");
            }

            var viewModel = await _systemInboxMessageService.GetMessageDetailsAsync(id, user.Id);

            if (viewModel == null)
            {
                TempData["Error"] = "Message not found or you do not have permission to view it.";
                return NotFound();
            }

            return View("SystemMessageDetails", viewModel);
        }





        [HttpGet]
        public async Task<IActionResult> AdminInbox()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "You must be logged in to perform this action.";
                return RedirectToAction("Login", "Account");
            }


            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isManager = await _userManager.IsInRoleAsync(user, "Manager");

            if (!isAdmin && !isManager)
            {
                TempData["Error"] = "You don't have permission to access this page.";
                return RedirectToAction("Index", "Home");
            }

            var messages = await _inboxMessageService.GetAdminMessagesAsync(user.Id);

            return View(messages);
        }
    }
}