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
        private readonly IAppUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;

        public ContactMessageClientService(
            IContactMessageRepository messageRepository,
            UserManager<AppUser> userManager,
            IAppUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _userManager = userManager;
            _userRepository = userRepository;
        }


        // sends a contact message to all admins/managers
        // ensuring no duplicates for the same subject/message from the same sender
        public async Task SendContactMessageAsync(ContactMessageCreateViewModel model, ClaimsPrincipal userPrincipal)
        {
            var sender = await _userManager.GetUserAsync(userPrincipal)
                ?? throw new ArgumentException("You must be logged in to send a contact message.");

            var adminIds = await GetAllAdminAndManagerIds();
            var existingRecipients = await GetExistingRecipients(sender.Id, model.Subject);

            var newMessages = adminIds
                .Where(id => id != sender.Id && !existingRecipients.Contains(id))
                .Select(id => new ContactMessage
                {
                    Id = Guid.NewGuid(),
                    SenderId = sender.Id,
                    ReceiverId = id,
                    Subject = model.Subject,
                    Message = model.Message,
                    Type = InboxMessageType.ContactMessage,
                    CreatedOn = DateTime.UtcNow,
                    IsRead = false
                })
                .ToList();

            if (newMessages.Any())
            {
                await _messageRepository.AddRangeAsync(newMessages);
                await _messageRepository.SaveChangesAsync();
            }
        }


        // retrieves all contact messages sent by the user that have a RESPONSE,
        // grouped by subject/message
        public async Task<List<ContactMessageDetailsViewModel>> GetUserMessagesAsync(string userId)
        {
            var messages = await _messageRepository
                .GetAllAttached()
                .Include(m => m.Sender)
                .Include(m => m.RespondedBy)
                .Where(m => m.SenderId == userId && !string.IsNullOrEmpty(m.Response))
                .OrderByDescending(m => m.CreatedOn)
                .ToListAsync();

            return messages
                .GroupBy(m => new { m.Subject, m.Message })
                .Select(g => new ContactMessageDetailsViewModel
                {
                    Id = g.First().Id,
                    Subject = g.Key.Subject,
                    Message = g.Key.Message,
                    SenderName = g.First().Sender?.FullName ?? "Unknown",
                    SenderEmail = g.First().Sender?.Email ?? string.Empty,
                    ReceiverName = "Admin Team",
                    ReceiverEmail = "support@furnituregardendesign.com",
                    IsRead = g.All(m => m.IsRead),
                    CreatedOn = g.Min(m => m.CreatedOn),
                    Response = g.FirstOrDefault(m => !string.IsNullOrEmpty(m.Response))?.Response,
                    RespondedAt = g.FirstOrDefault(m => m.RespondedAt.HasValue)?.RespondedAt,
                    RespondedByName = g.FirstOrDefault(m => m.RespondedBy != null)?.RespondedBy?.FullName
                })
                .ToList();
        }

        // retrieves the full conversation for a specific message
        // marks it as read, and returns details

        public async Task<ContactMessageDetailsViewModel?> GetMessageDetailsAsync(Guid messageId, string userId)
        {
            var message = await _messageRepository
                .GetAllAttached()
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.RespondedBy)
                .FirstOrDefaultAsync(m => m.Id == messageId && m.SenderId == userId);

            if (message == null) return null;

            var conversation = await GetConversation(message, userId);
            await MarkConversationAsRead(conversation);

            var respondedCopy = conversation.FirstOrDefault(m => !string.IsNullOrEmpty(m.Response));

            return new ContactMessageDetailsViewModel
            {
                Id = message.Id,
                Subject = message.Subject,
                Message = message.Message,
                SenderName = message.Sender?.FullName ?? "Unknown",
                SenderEmail = message.Sender?.Email ?? string.Empty,
                ReceiverName = "Admin Team",
                ReceiverEmail = "support@furnituregardendesign.com",
                IsRead = conversation.All(m => m.IsRead),
                CreatedOn = conversation.Min(m => m.CreatedOn),
                Response = respondedCopy?.Response,
                RespondedAt = respondedCopy?.RespondedAt,
                RespondedByName = respondedCopy?.RespondedBy?.FullName
            };
        }

        public async Task MarkMessageAsReadAsync(Guid messageId, string userId)
        {
            var message = await _messageRepository
                .FirstOrDefaultAsync(m => m.Id == messageId && m.ReceiverId == userId);

            if (message != null)
            {
                message.IsRead = true;
                await _messageRepository.UpdateAsync(message);
            }
        }

        // Helper methods
        private async Task<HashSet<string>> GetAllAdminAndManagerIds()
        {
            var allUsers = await _userRepository.GetAllAttached().ToListAsync();
            var adminIds = new HashSet<string>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin") || roles.Contains("Manager"))
                    adminIds.Add(user.Id);
            }

            return adminIds;
        }


        // retrieves all existing recipients for the same sender and subject to prevent duplicates
        private async Task<HashSet<string>> GetExistingRecipients(string senderId, string subject)
        {
            var recipients = await _messageRepository
                .GetAllAttached()
                .Where(m => m.SenderId == senderId && m.Subject == subject)
                .Select(m => m.ReceiverId)
                .ToListAsync();

            return recipients.ToHashSet();
        }


        // retrieves the full conversation for a specific message based on subject/message and sender
        private async Task<List<ContactMessage>> GetConversation(ContactMessage message, string userId)
        {
            return await _messageRepository
                .GetAllAttached()
                .Where(m => m.SenderId == userId
                    && m.Subject == message.Subject
                    && m.Message == message.Message)
                .ToListAsync();
        }


        // marks all messages in the conversation as read
        private async Task MarkConversationAsRead(List<ContactMessage> conversation)
        {
            var unreadMessages = conversation.Where(m => !m.IsRead).ToList();
            foreach (var msg in unreadMessages)
            {
                msg.IsRead = true;
                await _messageRepository.UpdateAsync(msg);
            }
        }
    }
}