using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations
{
    public class ReviewRepository:
         BaseRepository<Review, Guid>, IReviewRepository
    {
        protected ReviewRepository(ApplicationDbContext context) :
            base(context)
        {
        }
    }
}
