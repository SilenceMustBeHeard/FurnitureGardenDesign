using FurnitureGardenDesign.Web.ViewModels.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Interfaces.Message
{
    public interface IInboxMessageService
    {
        Task<List<InboxMessageViewModel>> GetUserMessagesAsync(string userId);
        Task MarkMessageAsReadAsync(Guid messageId, string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<int> GetUnreadCountForAdminAndManagerAsync(string userId);
        Task<int> GetUserUnreadContactResponsesCountAsync(string userId);  
        Task<InboxMessageViewModel?> GetMessageDetailsAsync(Guid messageId, string userId);
        Task<InboxMessageViewModel?> ApproveDesignAsync(Guid messageId, string userId);
        Task<List<InboxMessageViewModel>> GetAdminMessagesAsync(string adminId);
    }
}
