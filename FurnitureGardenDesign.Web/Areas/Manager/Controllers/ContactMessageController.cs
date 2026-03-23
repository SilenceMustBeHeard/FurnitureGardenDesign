using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Manager.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.User;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class ContactMessageController : Controller
    {
        private readonly IManagerContactMessageService _contactMessageService;
        private readonly UserManager<AppUser> _userManager;

        public ContactMessageController(
            IManagerContactMessageService contactMessageService,
            UserManager<AppUser> userManager)
        {
            _contactMessageService = contactMessageService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var managerId = _userManager.GetUserId(User);
            var messages = await _contactMessageService.GetAdminMessagesAsync(managerId);
            return View(messages);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var managerId = _userManager.GetUserId(User);
            var message = await _contactMessageService.GetMessageDetailsAsync(id, managerId);

            if (message == null)
            {
                TempData["Error"] = "Message not found or you don't have permission to view it.";
                return NotFound();
            }

            return View(message);
        }

        [HttpGet]
        public async Task<IActionResult> Respond(Guid id)
        {
            var adminId = _userManager.GetUserId(User);
            var message = await _contactMessageService.GetMessageDetailsAsync(id, adminId);

            if (message == null)
            {
                TempData["Error"] = "Message not found or you don't have permission to view it.";
                return NotFound();
            }

           
            if (!string.IsNullOrEmpty(message.Response))
            {
                TempData["Error"] = "This message has already been responded to.";
                return RedirectToAction(nameof(Details), new { id });
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

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Respond(ContactMessageResponseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var adminId = _userManager.GetUserId(User);

         
            var existingMessage = await _contactMessageService.GetMessageDetailsAsync(model.Id, adminId);
            if (existingMessage != null && !string.IsNullOrEmpty(existingMessage.Response))
            {
                TempData["Error"] = "This message has already been responded to.";
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }

            await _contactMessageService.RespondToMessageAsync(
                model.Id,
                model.Response,
                adminId);

            TempData["Success"] = "Response sent successfully!";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var managerId = _userManager.GetUserId(User);
            await _contactMessageService.MarkMessageAsReadAsync(id, managerId);

            return RedirectToAction(nameof(Index));
        }
    }
}