using FurnitureGardenDesign.Web.ViewModels.Messages;
using System.Security.Claims;

namespace FurnitureGardenDesign.Services.Core.Interfaces.Message
{
    public interface IContactMessageClientService
    {
        Task SendContactMessageAsync(ContactMessageCreateViewModel model, ClaimsPrincipal userPrincipal);

        Task<List<ContactMessageDetailsViewModel>> GetUserMessagesAsync(string userId);

        Task<ContactMessageDetailsViewModel?> GetMessageDetailsAsync(Guid messageId, string userId);

        Task<int> GetUserUnreadResponsesCountAsync(string userId);
    }
}