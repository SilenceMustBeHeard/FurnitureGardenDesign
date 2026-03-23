using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class ContactMessageClientService : IContactMessageClientService
    {
        private readonly IContactMessageRepository _messageRepository;
        private readonly UserManager<AppUser> _userManager;

        public ContactMessageClientService(
            IContactMessageRepository messageRepository,
            UserManager<AppUser> userManager)
        {
            _messageRepository = messageRepository;
            _userManager = userManager;
        }

        public async Task SendContactMessageAsync(ContactMessageCreateViewModel model, ClaimsPrincipal userPrincipal)
        {
            var sender = await _userManager.GetUserAsync(userPrincipal)
                ?? throw new ArgumentException("You must be logged in to send a contact message.");

         
            var adminUser = (await _userManager.GetUsersInRoleAsync("Admin")).FirstOrDefault()
                ?? throw new InvalidOperationException("No admin user found in the system.");

           
            var existingMessage = await _messageRepository
                .GetAllAttached()
                .FirstOrDefaultAsync(m => m.SenderId == sender.Id
                    && m.Subject == model.Subject
                    && m.Message == model.Message
                    && m.ReceiverId == adminUser.Id);

            if (existingMessage == null)
            {
                var contactMessage = new ContactMessage
                {
                    Id = Guid.NewGuid(),
                    SenderId = sender.Id,
                    ReceiverId = adminUser.Id,
                    Subject = model.Subject,
                    Message = model.Message,
                    Type = InboxMessageType.ContactMessage,
                    CreatedOn = DateTime.UtcNow,
                    IsRead = false,
                    IsReadByAdmin = false
                };

                await _messageRepository.AddAsync(contactMessage);
                await _messageRepository.SaveChangesAsync();
            }
        }

        public async Task<List<ContactMessageDetailsViewModel>> GetUserMessagesAsync(string userId)
        {
            return await _messageRepository
                .GetAllAttached()
                .Include(m => m.Sender)
                .Include(m => m.RespondedBy)
                .Where(m => m.SenderId == userId && !string.IsNullOrEmpty(m.Response))
                .OrderByDescending(m => m.CreatedOn)
                .Select(m => new ContactMessageDetailsViewModel
                {
                    Id = m.Id,
                    Subject = m.Subject,
                    Message = m.Message,
                    SenderName = m.Sender!.FullName ?? "Unknown",
                    SenderEmail = m.Sender!.Email ?? string.Empty,
                    ReceiverName = "Admin",
                    ReceiverEmail = "admin@furnituregarden.com",
                    IsRead = m.IsRead,
                    IsReadByAdmin = m.IsReadByAdmin,
                    CreatedOn = m.CreatedOn,
                    Response = m.Response,
                    RespondedAt = m.RespondedAt,
                    RespondedByName = m.RespondedBy!.FullName
                })
                .ToListAsync();
        }

        public async Task<ContactMessageDetailsViewModel?> GetMessageDetailsAsync(Guid messageId, string userId)
        {
            var message = await _messageRepository
                .GetAllAttached()
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.RespondedBy)
                .FirstOrDefaultAsync(m => m.Id == messageId && m.SenderId == userId);

            if (message == null) return null;

       
            if (!string.IsNullOrEmpty(message.Response) && !message.IsRead)
            {
                message.IsRead = true;
                await _messageRepository.UpdateAsync(message);
            }

            return new ContactMessageDetailsViewModel
            {
                Id = message.Id,
                Subject = message.Subject,
                Message = message.Message,
                SenderName = message.Sender!.FullName ?? "Unknown",
                SenderEmail = message.Sender!.Email ?? string.Empty,
                ReceiverName = "Admin",
                ReceiverEmail = "admin@furnituregarden.com",
                IsRead = message.IsRead,
                IsReadByAdmin = message.IsReadByAdmin,
                CreatedOn = message.CreatedOn,
                Response = message.Response,
                RespondedAt = message.RespondedAt,
                RespondedByName = message.RespondedBy?.FullName
            };
        }

        public async Task<int> GetUserUnreadResponsesCountAsync(string userId)
        {
            return await _messageRepository
                .GetAllAttached()
                .CountAsync(m => m.SenderId == userId
                    && !string.IsNullOrEmpty(m.Response)
                    && !m.IsRead);
        }
    }
}