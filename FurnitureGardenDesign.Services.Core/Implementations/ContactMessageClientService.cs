using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class ContactMessageClientService : IContactMessageClientService
    {
        private readonly IContactMessageRepository _messageRepository;
        private readonly IAppUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ContactMessageClientService(
            IContactMessageRepository messageRepository,
            UserManager<AppUser> userManager,
            IAppUserRepository userRepository,
            RoleManager<IdentityRole> roleManager)
        {
            _messageRepository = messageRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _roleManager = roleManager;
        }

        public async Task SendContactMessageAsync(ContactMessageCreateViewModel model, ClaimsPrincipal userPrincipal)
        {
            var user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null)
            {
                throw new ArgumentException("You must be logged in to send a contact message.");
            }

            var adminAndManagerIds = await GetAllAdminAndManagerIds();

            foreach (var adminId in adminAndManagerIds)
            {
                var contactMessage = new ContactMessage
                {
                    Id = Guid.NewGuid(),
                    SenderId = user.Id,
                    ReceiverId = adminId,
                    Subject = model.Subject,
                    Message = model.Message,
                    CreatedOn = DateTime.UtcNow,
                    IsRead = false
                };

                await _messageRepository.AddAsync(contactMessage);
            }
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