
using FurnitureGardenDesign.Web.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Services.Core.Manager.Interfaces
{
    public interface IManagerContactMessageService
    {
        Task<List<ContactMessageDetailsViewModel>> GetAdminMessagesAsync(string managerId);
        Task<ContactMessageDetailsViewModel?> GetMessageDetailsAsync(Guid messageId, string managerId);
        Task RespondToMessageAsync(Guid messageId, string response, string managerId);
        Task MarkMessageAsReadAsync(Guid messageId, string managerId);
        Task<int> GetUnreadCountAsync(string managerId);
    }
}