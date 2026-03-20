using FurnitureGardenDesign.Data.Models.Interactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Interfaces
{
    public interface IReviewManagementRepository : IReviewRepository
    {
        Task<IEnumerable<Review>> GetAllActiveAsync();
        Task<IEnumerable<Review>> GetAllForAdminAsync();
        Task ToggleReviewStatusAsync(Review review);
        Task<Review?> GetByIdIncludingDeletedAsync(Guid id);
        Task<bool> HardDeleteReviewAsync(Guid id); 
    }
}
