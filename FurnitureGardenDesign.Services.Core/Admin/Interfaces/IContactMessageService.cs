using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Web.ViewModels.User;

namespace FurnitureGardenDesign.Services.Core.Admin.Interfaces
{
    public interface IContactMessageService
    {
        Task<List<ContactMessageDetailsViewModel>> GetAdminMessagesAsync(string adminId);
        Task<ContactMessageDetailsViewModel?> GetMessageDetailsAsync(Guid messageId, string userId);
        Task RespondToConversationAsync(Guid messageId, string response, string adminId); 
        Task MarkMessageAsReadAsync(Guid messageId, string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<List<ContactMessageDetailsViewModel>> GetUserMessagesAsync(string userId);
    }
}
