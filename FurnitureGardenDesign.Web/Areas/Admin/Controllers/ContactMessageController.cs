using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;

using FurnitureGardenDesign.Web.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] 
    public class ContactMessageController : Controller
    {
        private readonly IContactMessageService _contactMessageService;
        private readonly IAppUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;

        public ContactMessageController(
            IContactMessageService contactMessageService,
            IAppUserRepository userRepository,
            UserManager<AppUser> userManager)
        {
            _contactMessageService = contactMessageService;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var adminId = _userManager.GetUserId(User);
            var messages = await _contactMessageService.GetAdminMessagesAsync(adminId);
            return View(messages);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var adminId = _userManager.GetUserId(User);
            var message = await _contactMessageService.GetMessageDetailsAsync(id, adminId);

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

            try
            {
                await _contactMessageService.RespondToConversationAsync(model.Id, model.Response, adminId);
                TempData["Success"] = "Response sent successfully!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var adminId = _userManager.GetUserId(User);
            await _contactMessageService.MarkMessageAsReadAsync(id, adminId!);

            return RedirectToAction(nameof(Index));
        }
    }
}