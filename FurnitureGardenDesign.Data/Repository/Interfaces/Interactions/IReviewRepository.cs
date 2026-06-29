using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Interfaces.Account;

namespace FurnitureGardenDesign.Data.Repository.Interfaces.Interactions
{
    public interface IReviewRepository
         : IRepository<Review, Guid>, IRepositoryAsync<Review, Guid>
    {
        Task<bool> HasUserReviewedAsync(string userId, Guid catalogDesignId);

        Task<IEnumerable<Review>> GetReviewsByDesignIdAsync(Guid catalogDesignId);
    }
}