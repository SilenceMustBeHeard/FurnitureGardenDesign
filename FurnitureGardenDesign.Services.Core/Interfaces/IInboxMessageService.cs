using FurnitureGardenDesign.Web.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface IInboxMessageService
    {
        Task MarkMessageAsReadAsync(Guid messageId, string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<InboxMessageViewModel?> GetMessageDetailsAsync(Guid messageId, string userId);
        Task<InboxMessageViewModel?> ApproveDesignAsync(Guid messageId, string userId);
        Task<List<InboxMessageViewModel>> GetAdminMessagesAsync(string adminId); 

    }
}
