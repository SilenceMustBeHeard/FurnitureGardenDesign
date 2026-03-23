using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Services.Core.Manager.Interfaces;

using FurnitureGardenDesign.Web.ViewModels.User;

namespace FurnitureGardenDesign.Services.Core.Manager.Implementations
{
    public class ManagerContactMessageService : IManagerContactMessageService
    {
        private readonly IContactMessageService _contactMessageService;

        public ManagerContactMessageService(IContactMessageService contactMessageService)
        {
            _contactMessageService = contactMessageService;
        }

        public async Task<List<ContactMessageDetailsViewModel>> GetAdminMessagesAsync(string managerId)
        {
            return await _contactMessageService.GetAdminMessagesAsync(managerId);
        }

        public async Task<ContactMessageDetailsViewModel?> GetMessageDetailsAsync(Guid messageId, string managerId)
        {
            return await _contactMessageService.GetMessageDetailsAsync(messageId, managerId);
        }

        public async Task RespondToMessageAsync(Guid messageId, string response, string managerId)
        {
            await _contactMessageService.RespondToMessageAsync(messageId, response, managerId);
        }

        public async Task MarkMessageAsReadAsync(Guid messageId, string managerId)
        {
            await _contactMessageService.MarkMessageAsReadAsync(messageId, managerId);
        }

        public async Task<int> GetUnreadCountAsync(string managerId)
        {
            return await _contactMessageService.GetUnreadCountAsync(managerId);
        }
    }
}