using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class ContactMessageClientService: IContactMessageService
    {
        private readonly IContactMessageRepository _messageRepository;
        private readonly IAppUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ContactMessageClientService(IContactMessageRepository messageRepository,
            UserManager<AppUser> userManager,
                IAppUserRepository userRepository,
            RoleManager<IdentityRole> roleManager)
        {
            _messageRepository = messageRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _roleManager = roleManager;
        }

        public async Task CreateMessageAsync(ContactMessage message)
        {
            await _messageRepository.AddAsync(message);
        }
    }
}
