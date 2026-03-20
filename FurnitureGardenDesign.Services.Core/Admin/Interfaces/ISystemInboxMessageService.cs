using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Web.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Admin.Interfaces
{
    public interface ISystemInboxMessageService
    {
        Task MarkMessageAsReadAsync(Guid messageId, string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<SystemInboxMessageViewModel?> GetMessageDetailsAsync(Guid messageId, string userId);
      
        Task<List<SystemInboxMessageViewModel>> GetAdminMessagesAsync(string adminId);

        Task CreateMessageAsync(SystemInboxMessage message);
        Task<List<SystemInboxMessageViewModel>> GetUserMessagesAsync(string userId);

    }
}
