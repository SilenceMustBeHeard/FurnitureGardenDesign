using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Interfaces.Account;
using FurnitureGardenDesign.Data.Repository.Interfaces.Message;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace FurnitureGardenDesign.Services.Core.Admin.Implementations
{
    public class SystemInboxMessageService : ISystemInboxMessageService
    {

        private readonly ISystemInboxMessageRepository _messageRepository;
        private readonly IAppUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SystemInboxMessageService(ISystemInboxMessageRepository messageRepository, 
            UserManager<AppUser> userManager,
                IAppUserRepository userRepository,
            RoleManager<IdentityRole> roleManager)
        {
            _messageRepository = messageRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _roleManager = roleManager;
        }















        // marks a message as read
        public async Task MarkMessageAsReadAsync(Guid messageId, string userId)
        {
            var message = await _messageRepository
                .FirstOrDefaultAsync(x => x.Id == messageId && x.ReceiverId == userId);

            if (message == null)
                return;

            message.IsRead = true;
            await _messageRepository.UpdateAsync(message);
        }

        // gets the count of unread messages
        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _messageRepository
                .GetAllAttached()
                .CountAsync(x => x.ReceiverId == userId && !x.IsRead);
        }
        

        public async Task<SystemInboxMessageViewModel?> GetMessageDetailsAsync(Guid messageId, string userId)
        {
            var message = await _messageRepository
                .GetAllAttached()
                .FirstOrDefaultAsync(m => m.Id == messageId && m.ReceiverId == userId);

            if (message == null)
                return null;

            message.IsRead = true;
            await _messageRepository.UpdateAsync(message);

            

            return new SystemInboxMessageViewModel
            {
               Id = messageId,
               Description = message.Description,
                IsRead = message.IsRead,
                CreatedOn = message.CreatedOn,
                Type = message.Type,
                SenderId = message.SenderId
            };
        }
      
        public async Task CreateMessageAsync(SystemInboxMessage message)
        {
            await _messageRepository.AddAsync(message);
        }

        public async Task<List<SystemInboxMessageViewModel>> GetUserMessagesAsync(string userId)
        {
            return await _messageRepository
                .GetAllAttached()
                .Where(m => m.ReceiverId == userId)
                .OrderByDescending(m => m.CreatedOn)
                .Select(m => new SystemInboxMessageViewModel
                {
                    Id = m.Id,
                    Description = m.Description,
                    IsRead = m.IsRead,
                    CreatedOn = m.CreatedOn,
                    Type = m.Type,
                    
                    SenderId = m.SenderId 
                    
                })
                .ToListAsync();
        }

        public async Task<List<SystemInboxMessageViewModel>> GetAdminMessagesAsync(string adminId)
        {
            return await _messageRepository
                .GetAllAttached()
                
               
                .Where(m => m.ReceiverId == adminId)
                .OrderByDescending(m => m.CreatedOn)
                .Select(m => new SystemInboxMessageViewModel
                {
                    Id = m.Id,
                   Description = m.Description,
                   IsRead = m.IsRead,
                   CreatedOn = m.CreatedOn,
                   Type = m.Type
                })
                .ToListAsync();
        }
    }
}
