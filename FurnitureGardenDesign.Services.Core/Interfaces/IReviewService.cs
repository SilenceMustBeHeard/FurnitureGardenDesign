using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Web.ViewModels;
using FurnitureGardenDesign.Web.ViewModels.Review;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface IReviewService
    {

        Task AddReviewAsync(string userId, AddReviewViewModel model);

        Task<bool> HasUserReviewedAsync(string userId, Guid catalogDesignId);


        Task<IEnumerable<AddReviewViewModel>> GetReviewsByDesignIdAsync(Guid catalogDesignId);
    }
}
