using FurnitureGardenDesign.Data;
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
        private readonly ApplicationDbContext context;

        public ProfileService(ApplicationDbContext context)
        {
            this.context = context;
        }


        // TODO: Map the rest of the properties (FirstName, LastName, Address) when they are added to the AppUser model
        // get the user profile along with their inbox messages, ordered by most recent first
        public async Task<ProfileViewModel?> GetProfileAsync(string userId)
        {
            var user = await context.Users
                .Include(u => u.InboxMessages)
                    .ThenInclude(m => m.DesignVariant)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            return new ProfileViewModel
            {
                Id = user.Id,
                Email = user.Email!,
            

                Inbox = user.InboxMessages
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
            };
        }

        // mark a specific message as read for the admin,
        // ensuring that only the intended recipient can mark it as read
        public async Task MarkMessageAsReadAsync(Guid messageId, string userId)
        {
            var message = await context.InboxMessages
                .FirstOrDefaultAsync(x => x.Id == messageId && x.ReceiverId == userId);

            if (message == null) return;

            message.IsRead = true;
            await context.SaveChangesAsync();
        }


        // get the count of unread messages for the user,
        // which can be displayed in the UI to notify them of new messages
        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await context.InboxMessages
                .CountAsync(x => x.ReceiverId == userId && !x.IsRead);
        }
    }

}
