using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations
{
    public class FavoriteRepository:
          BaseRepository<Favorite, Guid>, IFavoriteRepository
    {
        public FavoriteRepository(ApplicationDbContext context)
            : base(context)
        {
        }

      
    }
}
