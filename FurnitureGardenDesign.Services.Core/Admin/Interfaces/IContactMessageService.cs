using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Web.ViewModels.User;

namespace FurnitureGardenDesign.Services.Core.Admin.Interfaces
{
    public interface IContactMessageService
    {
        Task MarkMessageAsReadAsync(Guid messageId, string userId);
        Task<int> GetUnreadCountAsync(string userId);

        Task<ContactMessageDetailsViewModel?> GetMessageDetailsAsync(Guid messageId, string userId);

        Task<List<ContactMessageDetailsViewModel>> GetAdminMessagesAsync(string adminId);
        Task RespondToMessageAsync(Guid messageId, string response, string adminId);
        // Task CreateMessageAsync(ContactMessage message);
     
        Task<List<ContactMessageDetailsViewModel>> GetUserMessagesAsync(string userId);

    }
}
