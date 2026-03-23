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
        private readonly IContactMessageService _contactMessageService;

        public ProfileService(
            IAppUserRepository userRepository,
            UserManager<AppUser> userManager,
            IInboxMessageService inboxMessageService,
            ISystemInboxMessageService systemInboxMessageService,
            IContactMessageClientService contactMessageClientService,
            IContactMessageService contactMessageService)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _inboxMessageService = inboxMessageService;
            _systemInboxMessageService = systemInboxMessageService;
            _contactMessageClientService = contactMessageClientService;
            _contactMessageService = contactMessageService;
        }

        public async Task<ProfileViewModel?> GetProfileAsync(string userId)
        {
            var user = await _userRepository
                .GetAllAttached()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            var inboxMessages = await _inboxMessageService.GetUserMessagesAsync(userId);
            var systemMessages = await _systemInboxMessageService.GetUserMessagesAsync(userId);

            var roles = await _userManager.GetRolesAsync(user);
            var isAdmin = roles.Contains("Admin");  
            var isManager = roles.Contains("Manager");

            List<ContactMessageDetailsViewModel> contactMessages = new List<ContactMessageDetailsViewModel>();

            if (isAdmin)
            {
              
                contactMessages = await _contactMessageService.GetAdminMessagesAsync(userId);
            }
            else if (!isManager)
            {
               
                contactMessages = await _contactMessageClientService.GetUserMessagesAsync(userId);
            }
          

            return new ProfileViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Inbox = inboxMessages?.ToList() ?? new List<InboxMessageViewModel>(),
                SystemInbox = systemMessages?.ToList() ?? new List<SystemInboxMessageViewModel>(),
                ContactMessages = contactMessages ?? new List<ContactMessageDetailsViewModel>()
            };
        }
    }
}