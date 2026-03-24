namespace FurnitureGardenDesign.Services.Core.Interfaces.Account
{
    public interface IManagerService
    {
        Task<bool> IsUserManagerAsync(string userId);
    }
}
