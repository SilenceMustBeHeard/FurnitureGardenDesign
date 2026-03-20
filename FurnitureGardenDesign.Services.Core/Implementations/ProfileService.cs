using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class ProfileService : IProfileService
    {
       
        private readonly IAppUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IInboxMessageService _inboxMessageService; 
        private readonly ISystemInboxMessageService _systemInboxMessageService; 
        private readonly IContactMessageClientService _contactMessageClientService; 
        public ProfileService(

            IAppUserRepository userRepository,
            UserManager<AppUser> userManager,
            IInboxMessageService inboxMessageService, 
            ISystemInboxMessageService systemInboxMessageService,
            IContactMessageClientService contactMessageClientService

            ) 

        {

            _userRepository = userRepository;
            _userManager = userManager;
            _inboxMessageService = inboxMessageService;
            _systemInboxMessageService = systemInboxMessageService; 
            _contactMessageClientService = contactMessageClientService;
        }




        public async Task<ProfileViewModel?> GetProfileAsync(string userId)
        {
            var user = await _userRepository
                .GetAllAttached()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            // Get all types of messages
            var inboxMessages = await _inboxMessageService.GetUserMessagesAsync(userId);
            var systemMessages = await _systemInboxMessageService.GetUserMessagesAsync(userId);
            var contactMessages = await _contactMessageClientService.GetUserMessagesAsync(userId); 

            return new ProfileViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Inbox = inboxMessages?.ToList() 
                ?? new List<InboxMessageViewModel>(),

                SystemInbox = systemMessages?.ToList()
                ?? new List<SystemInboxMessageViewModel>(),

                ContactMessages = contactMessages?.ToList() 
                ?? new List<ContactMessageDetailsViewModel>() 
            };
        }

        private async Task<HashSet<string>> GetAllAdminAndManagerIds()
        {
            var adminIds = new HashSet<string>();
            var allUsers = await _userRepository.GetAllAttached().ToListAsync();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin") || roles.Contains("Manager"))
                {
                    adminIds.Add(user.Id);
                }
            }

            return adminIds;
        }

    }
}
    
