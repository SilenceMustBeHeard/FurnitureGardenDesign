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
        //  only creates one message per admin/manager (no duplicates)
        public async Task SendContactMessageAsync(ContactMessageCreateViewModel model, ClaimsPrincipal userPrincipal)
        {
            var sender = await _userManager.GetUserAsync(userPrincipal);
            if (sender == null)
                throw new ArgumentException("You must be logged in to send a contact message.");

            // Get all admins and managers
            var adminAndManagerIds = await GetAllAdminAndManagerIds();

            // Get existing messages from this sender with same subject
            var existingMessages = await _messageRepository
                .GetAllAttached()
                .Where(m => m.SenderId == sender.Id && m.Subject == model.Subject)
                .Select(m => m.ReceiverId)
                .ToListAsync();

            var newMessages = new List<ContactMessage>();

            // Create ONE message per admin/manager (excluding self and duplicates)
            foreach (var receiverId in adminAndManagerIds)
            {
                if (receiverId == sender.Id)
                    continue;

                if (existingMessages.Contains(receiverId))
                    continue;

                newMessages.Add(new ContactMessage
                {
                    Id = Guid.NewGuid(),
                    SenderId = sender.Id,
                    ReceiverId = receiverId,
                    Subject = model.Subject,
                    Message = model.Message,
                    Type = InboxMessageType.ContactMessage,
                    CreatedOn = DateTime.UtcNow,
                    IsRead = false
                });
            }

            if (newMessages.Count > 0)
            {
                await _messageRepository.AddRangeAsync(newMessages);
                await _messageRepository.SaveChangesAsync();
            }
        }



        // retrieves all messages sent by the user
        // grouped by subject and message content to combine duplicates
        public async Task<List<ContactMessageDetailsViewModel>> GetUserMessagesAsync(string userId)
        {
            // Get all messages sent by this user
            var messages = await _messageRepository
                .GetAllAttached()
                .Include(m => m.Receiver)
                .Include(m => m.RespondedBy)
                .Include(m => m.Sender)
                .Where(m => m.SenderId == userId)
                .OrderByDescending(m => m.CreatedOn)
                .ToListAsync();

            // Group by subject and message content to combine duplicates
            var groupedMessages = messages
                .GroupBy(m => new { m.Subject, m.Message })
                .Select(g => new ContactMessageDetailsViewModel
                {
                    Id = g.First().Id,
                    Subject = g.Key.Subject,
                    Message = g.Key.Message,
                    SenderName = g.First().Sender != null
                        ? $"{g.First().Sender.FirstName} {g.First().Sender.LastName}"
                        : "Unknown",
                    SenderEmail = g.First().Sender?.Email ?? string.Empty,
                    ReceiverName = "Admin Team",
                    ReceiverEmail = "support@furnituregardendesign.com",
                    IsRead = g.Any(m => m.IsRead),
                    CreatedOn = g.Min(m => m.CreatedOn),
                    Response = g.FirstOrDefault(m => !string.IsNullOrEmpty(m.Response))?.Response,
                    RespondedAt = g.FirstOrDefault(m => m.RespondedAt.HasValue)?.RespondedAt,
                    RespondedByName = g.FirstOrDefault(m => m.RespondedBy != null)?.RespondedBy != null
                        ? $"{g.First().RespondedBy?.FirstName} {g.First().RespondedBy?.LastName}"
                        : null
                })
                .ToList();

            return groupedMessages;
        }

        // retrieves details for a specific message
        // including response if available
        public async Task<ContactMessageDetailsViewModel?> GetMessageDetailsAsync(Guid messageId, string userId)
        {
            var message = await _messageRepository
                .GetAllAttached()
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.RespondedBy)
                .FirstOrDefaultAsync(m => m.Id == messageId && m.SenderId == userId);

            if (message == null)
                return null;

           
            if (!message.IsRead)
            {
                message.IsRead = true;
                await _messageRepository.UpdateAsync(message);
            }

            // Get all copies of this conversation to show combined view
            var allCopies = await _messageRepository
                .GetAllAttached()
                .Include(m => m.Receiver)
                .Include(m => m.RespondedBy)
                .Where(m => m.SenderId == userId
                    && m.Subject == message.Subject
                    && m.Message == message.Message)
                .ToListAsync();

          
            var respondedCopy = allCopies.FirstOrDefault(m => !string.IsNullOrEmpty(m.Response));
            var anyCopyRead = allCopies.Any(m => m.IsRead);
            var earliestDate = allCopies.Min(m => m.CreatedOn);

            return new ContactMessageDetailsViewModel
            {
                Id = message.Id,
                Subject = message.Subject,
                Message = message.Message,
                SenderName = message.Sender != null
                    ? $"{message.Sender.FirstName} {message.Sender.LastName}"
                    : "Unknown",
                SenderEmail = message.Sender?.Email ?? string.Empty,
                ReceiverName = "Admin Team",
                ReceiverEmail = "support@furnituregardendesign.com",
                IsRead = anyCopyRead,  
                CreatedOn = earliestDate,
                Response = respondedCopy?.Response,
                RespondedAt = respondedCopy?.RespondedAt,
                RespondedByName = respondedCopy?.RespondedBy != null
                    ? $"{respondedCopy.RespondedBy.FirstName} {respondedCopy.RespondedBy.LastName}"
                    : null
            };
        }



        public async Task MarkMessageAsReadAsync(Guid messageId, string userId)
        {
            var message = await _messageRepository
                .FirstOrDefaultAsync(x => x.Id == messageId && x.ReceiverId == userId);

            if (message == null)
                return;

            message.IsRead = true;
            await _messageRepository.UpdateAsync(message);
        }

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
    }
}