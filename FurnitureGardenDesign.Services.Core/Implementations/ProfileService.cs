using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class ProfileService : IProfileService
    {

        // seed repositories
        private readonly IAppUserRepository userRepository;
        private readonly IInboxMessageRepository messageRepository;
      

        public ProfileService(
            IAppUserRepository userRepository,
            IInboxMessageRepository messageRepository
             )
        {
            this.userRepository = userRepository;
            this.messageRepository = messageRepository;
        }


        // gets the profile data for the user, including their inbox messages
        // this is used in the profile page to display user information and messages
        public async Task<ProfileViewModel?> GetProfileAsync(string userId)
        {
            var model = await userRepository
                .GetAllAttached()
                .Where(u => u.Id == userId)
                .Select(u => new ProfileViewModel
                {
                    Id = u.Id,
                    Email = u.Email!,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Address = u.Address,

                    Inbox = u.InboxMessages
                        .OrderByDescending(x => x.CreatedOn)
                        .Select(x => new InboxMessageViewModel
                        {
                            Id = x.Id,
                            DesignImageUrl = x.DesignVariant!.ImageUrl,
                            Notes = x.DesignVariant!.Notes,
                            IsRead = x.IsRead,
                            CreatedOn = x.CreatedOn
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return model;
        }


        // marks a message as read when the user views it in their inbox,
        // this is used to update the message status in the database and reflect it in the UI
        public async Task MarkMessageAsReadAsync(Guid messageId, string userId)
        {
            var message = await messageRepository
                .FirstOrDefaultAsync(x => x.Id == messageId && x.ReceiverId == userId);

            if (message == null)
                return;

            message.IsRead = true;

            await messageRepository.UpdateAsync(message);

        }

        // gets the count of unread messages for the user, this is used to display a notification badge in the UI
        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await messageRepository
                .GetAllAttached()
                .CountAsync(x => x.ReceiverId == userId && !x.IsRead);
        }
    }


}
