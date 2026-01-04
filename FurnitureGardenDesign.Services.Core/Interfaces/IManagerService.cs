namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface IManagerService
    {
        Task<bool> IsUserManagerAsync(string userId);
    }
}
