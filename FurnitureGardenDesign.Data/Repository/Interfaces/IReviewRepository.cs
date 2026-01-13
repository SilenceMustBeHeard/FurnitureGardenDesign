using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Interfaces
{
    public interface IReviewRepository
         : IRepository<Review, Guid>, IRepositoryAsync<Review, Guid>
    {
    
      Task<bool> HasUserReviewedAsync(string userId, Guid catalogDesignId);
    }
}
