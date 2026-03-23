using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;

using FurnitureGardenDesign.Web.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Services.Core.Admin.Implementations
{
    public class ContactMessageService : IContactMessageService
    {
        private readonly IContactMessageRepository _messageRepository;
        private readonly IAppUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;

        public ContactMessageService(
            IContactMessageRepository messageRepository,
            IAppUserRepository userRepository,
            UserManager<AppUser> userManager)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<List<ContactMessageDetailsViewModel>> GetAdminMessagesAsync(string adminId)
        {
            return await _messageRepository
                .GetAllAttached()
                .Include(m => m.Sender)
                .Include(m => m.RespondedBy)
                .Where(m => m.ReceiverId == adminId)
                .OrderByDescending(m => m.CreatedOn)
                .Select(m => new ContactMessageDetailsViewModel
                {
                    Id = m.Id,
                    Subject = m.Subject,
                    Message = m.Message,
                    SenderName = m.Sender!.FullName ?? "Unknown",
                    SenderEmail = m.Sender!.Email ?? string.Empty,
                    IsRead = m.IsRead,
                    IsReadByAdmin = m.IsReadByAdmin,
                    CreatedOn = m.CreatedOn,
                    Response = m.Response,
                    RespondedAt = m.RespondedAt,
                    RespondedByName = m.RespondedBy!.FullName
                })
                .ToListAsync();
        }

        public async Task RespondToConversationAsync(Guid messageId, string response, string adminId)
        {
          
            var originalMessage = await _messageRepository
                .GetAllAttached()
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == messageId)
                ?? throw new ArgumentException("Message not found");

           
            var allCopies = await _messageRepository
                .GetAllAttached()
                .Where(m => m.SenderId == originalMessage.SenderId
                    && m.Subject == originalMessage.Subject
                    && m.Message == originalMessage.Message)
                .ToListAsync();

         
            if (allCopies.Any(c => !string.IsNullOrEmpty(c.Response)))
            {
                throw new InvalidOperationException("This message has already been responded to.");
            }

          
            foreach (var copy in allCopies)
            {
                copy.Response = response;
                copy.RespondedAt = DateTime.UtcNow;
                copy.RespondedById = adminId;
                copy.IsReadByAdmin = true;  

                await _messageRepository.UpdateAsync(copy);
            }
        }


        public async Task<ContactMessageDetailsViewModel?> GetMessageDetailsAsync(Guid messageId, string managerId)
        {
            var message = await _messageRepository
                .GetAllAttached()
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.RespondedBy)
                .FirstOrDefaultAsync(m => m.Id == messageId
                    && (m.SenderId == managerId || m.ReceiverId == managerId));

            if (message == null) return null;

         
            if (message.ReceiverId == managerId && !message.IsReadByAdmin)
            {
                message.IsReadByAdmin = true;
                await _messageRepository.UpdateAsync(message);
            }

           
            return new ContactMessageDetailsViewModel
            {
                Id = message.Id,
                Subject = message.Subject,
                Message = message.Message,
                SenderName = message.Sender?.FullName ?? "Unknown",
                SenderEmail = message.Sender?.Email ?? string.Empty,
                ReceiverName = message.Receiver?.FullName ?? "Unknown",
                ReceiverEmail = message.Receiver?.Email ?? string.Empty,
                IsRead = message.IsRead,
                IsReadByAdmin = message.IsReadByAdmin,
                CreatedOn = message.CreatedOn,
                Response = message.Response,  
                RespondedAt = message.RespondedAt,
                RespondedByName = message.RespondedBy?.FullName
            };
        }


        public async Task<int> GetUnreadCountAsync(string userId)
        {
           
            return await _messageRepository
                .GetAllAttached()
                .CountAsync(m => m.ReceiverId == userId && !m.IsReadByAdmin);
        }

        public async Task<List<ContactMessageDetailsViewModel>> GetUserMessagesAsync(string userId)
        {
            return await _messageRepository
                .GetAllAttached()
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == userId)
                .OrderByDescending(m => m.CreatedOn)
                .Select(m => new ContactMessageDetailsViewModel
                {
                    Id = m.Id,
                    Subject = m.Subject,
                    Message = m.Message,
                    SenderName = m.Sender!.FullName ?? "Unknown",
                    SenderEmail = m.Sender!.Email ?? string.Empty,
                    ReceiverName = m.Receiver!.FullName ?? "Unknown",
                    ReceiverEmail = m.Receiver!.Email ?? string.Empty,
                    IsRead = m.IsRead,
                    IsReadByAdmin = m.IsReadByAdmin,
                    CreatedOn = m.CreatedOn,
                    Response = m.Response,
                    RespondedAt = m.RespondedAt
                })
                .ToListAsync();
        }
       
        public async Task MarkMessageAsReadAsync(Guid messageId, string userId)
        {
            var message = await _messageRepository
                .FirstOrDefaultAsync(m => m.Id == messageId && m.ReceiverId == userId);

            if (message != null)
            {
                message.IsReadByAdmin = true;
                await _messageRepository.UpdateAsync(message);
            }
        }
    }
}