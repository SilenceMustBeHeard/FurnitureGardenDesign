
using FurnitureGardenDesign.Web.ViewModels.Messages;

namespace FurnitureGardenDesign.Services.Core.Admin.Interfaces
{
    public interface IContactMessageService
    {
        Task<List<ContactMessageDetailsViewModel>> GetAdminMessagesAsync(string adminId);
        Task<ContactMessageDetailsViewModel?> GetMessageDetailsAsync(Guid messageId, string adminId);
        Task RespondToMessageAsync(Guid messageId, string response, string adminId);
        Task MarkMessageAsReadAsync(Guid messageId, string adminId);
        Task<int> GetUnreadCountAsync(string adminId);
    }
}