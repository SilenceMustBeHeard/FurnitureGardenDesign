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

        //  view all messages sent to admin
        // along with sender info and response status

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
                    SenderName = (m.Sender.FirstName ?? "") + " " + (m.Sender.LastName ?? ""),
                    SenderEmail = m.Sender.Email ?? string.Empty,
                    IsRead = m.IsRead,
                    CreatedOn = m.CreatedOn,
                    Response = m.Response,
                    RespondedAt = m.RespondedAt,
                    RespondedByName = m.RespondedBy != null
                        ? (m.RespondedBy.FirstName ?? "") + " " + (m.RespondedBy.LastName ?? "")
                        : null
                })
                .ToListAsync();
        }

        // allow admin to respond to a message
        public async Task RespondToMessageAsync(Guid messageId, string response, string adminId)
        {
            var message = await _messageRepository
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null)
              { 
                throw new ArgumentException("Message not found"); 
            }

         


            message.Response = response;
            message.RespondedAt = DateTime.UtcNow;
            message.RespondedById = adminId;
            message.IsRead = true;

            await _messageRepository.UpdateAsync(message);
        }

        // view message details along with sender and receiver info

        public async Task<ContactMessageDetailsViewModel?> GetMessageDetailsAsync(Guid messageId, string userId)
        {
            var message = await _messageRepository
                .GetAllAttached()
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.RespondedBy)
                .FirstOrDefaultAsync(m => m.Id == messageId 
                && (m.SenderId == userId 
                || m.ReceiverId == userId));

            if (message == null)
                return null;

           
            if (message.ReceiverId == userId
                && !message.IsRead)
            {
                message.IsRead = true;
                await _messageRepository.UpdateAsync(message);
            }

            return new ContactMessageDetailsViewModel
            {
                Id = message.Id,
                Subject = message.Subject,
                Message = message.Message,
                SenderName = (message.Sender.FirstName ?? "") + " " + (message.Sender.LastName ?? ""),
                SenderEmail = message.Sender.Email ?? string.Empty,
                ReceiverName = (message.Receiver.FirstName ?? "") + " " + (message.Receiver.LastName ?? ""),
                ReceiverEmail = message.Receiver.Email ?? string.Empty,
                IsRead = message.IsRead,
                CreatedOn = message.CreatedOn,
                Response = message.Response,
                RespondedAt = message.RespondedAt,
                RespondedByName = message.RespondedBy != null
                    ? (message.RespondedBy.FirstName ?? "") + " " + (message.RespondedBy.LastName ?? "")
                    : null
               
            };
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _messageRepository
                .GetAllAttached()
                .CountAsync(m => m.ReceiverId == userId && !m.IsRead);
        }



        // view all messages sent by user along with receiver info and response status

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
                    SenderName = (m.Sender.FirstName ?? "") + " " + (m.Sender.LastName ?? ""),
                    SenderEmail = m.Sender.Email ?? string.Empty,
                    ReceiverName = (m.Receiver.FirstName ?? "") + " " + (m.Receiver.LastName ?? ""),
                    ReceiverEmail = m.Receiver.Email ?? string.Empty,
                    IsRead = m.IsRead,
                    CreatedOn = m.CreatedOn,
                    Response = m.Response,
                    RespondedAt = m.RespondedAt
                })
                .ToListAsync();
        }

        public async Task MarkMessageAsReadAsync(Guid messageId, string userId)
        {
            var message = await _messageRepository
                .FirstOrDefaultAsync(m => m.Id == messageId 
                && m.ReceiverId == userId);

            if (message == null)
                return;

            message.IsRead = true;
            await _messageRepository.UpdateAsync(message);
        }
    }
}