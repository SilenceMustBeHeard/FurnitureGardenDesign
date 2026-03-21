using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class InboxMessageService : IInboxMessageService
    {

        private readonly IInboxMessageRepository _messageRepository;
        private readonly ISystemInboxMessageRepository _systemMessageRepository;
        
        private readonly IAppUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public InboxMessageService(
            IInboxMessageRepository messageRepository, 
           
            ISystemInboxMessageRepository systemMessageRepository,
            UserManager<AppUser> userManager,
                IAppUserRepository userRepository,
            RoleManager<IdentityRole> roleManager)
        {
           
            _systemMessageRepository = systemMessageRepository;
            _messageRepository = messageRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _roleManager = roleManager;
        }







        public async Task<List<InboxMessageViewModel>> GetUserMessagesAsync(string userId)
        {
            return await _messageRepository
                .GetAllAttached()
                .Include(m => m.DesignVariant)
                .Where(m => m.ReceiverId == userId && m.DesignVariant != null && !m.DesignVariant.IsDeleted)
                .OrderByDescending(m => m.CreatedOn)
                .Select(m => new InboxMessageViewModel
                {
                    Id = m.Id,
                    DesignVariantId = m.DesignVariant!.Id,
                    DesignImage2DUrl = m.DesignVariant.Image2DUrl,
                    Model3DUrl = m.DesignVariant.Model3DUrl,
                    Notes = m.DesignVariant.Notes,
                   OrderDescription = m.DesignVariant.Order != null ? m.DesignVariant.Order.Description : null,
                     OrderDimensions = m.DesignVariant.Order != null ? m.DesignVariant.Order.Dimensions : null,
                    IsRead = m.IsRead,
                    CreatedOn = m.CreatedOn,
                    Type = m.Type,
                    IsApproved = m.DesignVariant.IsApproved
                })
                .ToListAsync();
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
           
            var inboxUnreadCount = await _messageRepository
                .GetAllAttached()
                .CountAsync(x => x.ReceiverId == userId && !x.IsRead);

            
            var systemUnreadCount = await _systemMessageRepository
                .GetAllAttached()
                .CountAsync(x => x.ReceiverId == userId && !x.IsRead);

            
         


            return inboxUnreadCount + systemUnreadCount;
        }


        public async Task<InboxMessageViewModel?> GetMessageDetailsAsync(Guid messageId, string userId)
        {
            var message = await _messageRepository
                .GetAllAttached()
                .Include(m => m.DesignVariant)
                    .ThenInclude(d => d.Order)
                .FirstOrDefaultAsync(m => m.Id == messageId && m.ReceiverId == userId);

            if (message == null || message.DesignVariant == null)
                return null;

            message.IsRead = true;
            await _messageRepository.UpdateAsync(message);

            var design = message.DesignVariant;

            return new InboxMessageViewModel
            {
                Id = message.Id,
                DesignVariantId = design.Id,
                DesignImage2DUrl = design.Image2DUrl,
                Model3DUrl = design.Model3DUrl,
                Notes = design.Notes,
                IsRead = message.IsRead,
                IsApproved = design.IsApproved,
                CreatedOn = message.CreatedOn,
                Type = message.Type,
                OrderDescription = design.Order?.Description,
                OrderDimensions = design.Order?.Dimensions,
                ReferenceImageUrl = design.Order?.ReferenceImageUrl
            };
        }

        public async Task<InboxMessageViewModel?> ApproveDesignAsync(Guid messageId, string userId)
        {
            var message = await _messageRepository
                .GetAllAttached()
                .Include(m => m.DesignVariant)
                    .ThenInclude(d => d.Order)
                .FirstOrDefaultAsync(m => m.Id == messageId && m.ReceiverId == userId);

            if (message == null || message.DesignVariant == null)
                return null;

            var design = message.DesignVariant;
            var order = design.Order;

            if (!design.IsApproved)
            {
                design.IsApproved = true;



                var recipientIds = new HashSet<string>();


                if (!string.IsNullOrEmpty(message.SenderId))
                {
                    recipientIds.Add(message.SenderId);
                }


                var originalMessage = await _messageRepository
                    .GetAllAttached()
                    .FirstOrDefaultAsync(m => m.DesignVariantId == design.Id
                    && m.Type == InboxMessageType.DesignSent);

                if (originalMessage != null && !string.IsNullOrEmpty(originalMessage.SenderId))
                {
                    recipientIds.Add(originalMessage.SenderId);
                }

                // 3. IMPORTANT: Find ALL admins and managers, not just the original sender!
                var allUsers = await _userRepository.GetAllAttached().ToListAsync();

                foreach (var user in allUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Admin") || roles.Contains("Manager"))
                    {
                        recipientIds.Add(user.Id);
                    }
                }


                foreach (var recipientId in recipientIds)
                {
                    if (recipientId == userId) continue;

                    var approvalMessage = new InboxMessage
                    {
                        Id = Guid.NewGuid(),
                        DesignVariantId = design.Id,
                        ReceiverId = recipientId,
                        SenderId = userId,
                        Type = InboxMessageType.DesignApproved,
                        IsRead = false,
                        CreatedOn = DateTime.UtcNow,
                        Notes = $"Design for Order #{order?.Id.ToString().Substring(0, 8)} has been approved by the client."
                    };

                    await _messageRepository.AddAsync(approvalMessage);
                }
            }

            return new InboxMessageViewModel
            {
                Id = message.Id,
                DesignVariantId = design.Id,
                DesignImage2DUrl = design.Image2DUrl,
                Model3DUrl = design.Model3DUrl,
                Notes = design.Notes,
                IsRead = message.IsRead,
                IsApproved = design.IsApproved,
                CreatedOn = message.CreatedOn,
                Type = message.Type,
                OrderDescription = order?.Description,
                OrderDimensions = order?.Dimensions,
                ReferenceImageUrl = order?.ReferenceImageUrl
            };
        }

        public async Task<List<InboxMessageViewModel>> GetAdminMessagesAsync(string adminId)
        {
            return await _messageRepository
                .GetAllAttached()
                .Include(m => m.DesignVariant)
                .ThenInclude(d => d.Order)
                .Where(m => m.ReceiverId == adminId)
                .OrderByDescending(m => m.CreatedOn)
                .Select(m => new InboxMessageViewModel
                {
                    Id = m.Id,
                    DesignVariantId = m.DesignVariant!.Id,
                    DesignImage2DUrl = m.DesignVariant.Image2DUrl,
                    Model3DUrl = m.DesignVariant.Model3DUrl,
                    Notes = m.Notes ?? m.DesignVariant.Notes,
                    IsRead = m.IsRead,
                    IsApproved = m.DesignVariant.IsApproved,
                    CreatedOn = m.CreatedOn,
                    Type = m.Type,
                    OrderDescription = m.DesignVariant.Order != null ? m.DesignVariant.Order.Description : null,
                    OrderDimensions = m.DesignVariant.Order != null ? m.DesignVariant.Order.Dimensions : null,
                    ReferenceImageUrl = m.DesignVariant.Order != null ? m.DesignVariant.Order.ReferenceImageUrl : null
                })
                .ToListAsync();
        }
    }
}
