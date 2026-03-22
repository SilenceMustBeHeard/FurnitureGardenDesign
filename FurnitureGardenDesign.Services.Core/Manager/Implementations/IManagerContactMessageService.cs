using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Manager.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Services.Core.Manager.Implementations
{
    public class ManagerContactMessageService : IManagerContactMessageService
    {

        private readonly IContactMessageRepository _messageRepository;
        private readonly IAppUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;

        public ManagerContactMessageService(
            IContactMessageRepository messageRepository,
            IAppUserRepository userRepository,
            UserManager<AppUser> userManager)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        // retrieves all contact messages for the admin
        // including sender and responder details, ordered by creation date
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
                    SenderName = m.Sender!.FullName,
                    SenderEmail = m.Sender.Email!,
                    IsRead = m.IsRead,
                    CreatedOn = m.CreatedOn,
                    Response = m.Response,
                    RespondedAt = m.RespondedAt,
                    RespondedByName = m.RespondedBy!.FullName
                })
                .ToListAsync();
        }

        // allows the admin to respond to a contact message
        // updating the message with the response details
        public async Task RespondToMessageAsync(Guid messageId, string response, string adminId)
        {
            var message = await _messageRepository
                .FirstOrDefaultAsync(m => m.Id == messageId)
                ?? throw new ArgumentException("Message not found");

            message.Response = response;
            message.RespondedAt = DateTime.UtcNow;
            message.RespondedById = adminId;
            message.IsRead = false;

            await _messageRepository.UpdateAsync(message);
        }


        // retrieves the details of a specific contact message for a user
        public async Task<ContactMessageDetailsViewModel?> GetMessageDetailsAsync(Guid messageId, string userId)
        {
            var message = await _messageRepository
                .GetAllAttached()
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.RespondedBy)
                .FirstOrDefaultAsync(m => m.Id == messageId
                    && (m.SenderId == userId || m.ReceiverId == userId));

            if (message == null) return null;


            if (message.ReceiverId == userId && !message.IsRead)
            {
                message.IsRead = true;
                await _messageRepository.UpdateAsync(message);
            }

            return new ContactMessageDetailsViewModel
            {
                Id = message.Id,
                Subject = message.Subject,
                Message = message.Message,
                SenderName = message.Sender!.FullName,
                SenderEmail = message.Sender!.Email!,
                ReceiverName = message.Receiver!.FullName,
                ReceiverEmail = message.Receiver!.Email!,
                IsRead = message.IsRead,
                CreatedOn = message.CreatedOn,
                Response = message.Response,
                RespondedAt = message.RespondedAt,
                RespondedByName = message.RespondedBy?.FullName
            };
        }


        // retrieves the count of unread messages for a specific user
        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _messageRepository
                .GetAllAttached()
                .CountAsync(m => m.ReceiverId == userId && !m.IsRead);
        }

        // retrieves all contact messages sent by the user
        // ordered by creation date
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
                    SenderName = m.Sender!.FullName,
                    SenderEmail = m.Sender!.Email!,
                    ReceiverName = m.Receiver!.FullName,
                    ReceiverEmail = m.Receiver!.Email!,
                    IsRead = m.IsRead,
                    CreatedOn = m.CreatedOn,
                    Response = m.Response,
                    RespondedAt = m.RespondedAt
                })
                .ToListAsync();
        }

        // allows the user to mark a specific message as read
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
    }
}
