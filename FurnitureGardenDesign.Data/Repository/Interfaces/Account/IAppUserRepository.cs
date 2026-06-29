using FurnitureGardenDesign.Data.Models;

namespace FurnitureGardenDesign.Data.Repository.Interfaces.Account
{
    public interface IAppUserRepository
        : IRepository<AppUser, string>, IRepositoryAsync<AppUser, string>

    {
    }
}