using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Interfaces
{
    public interface IFavoriteRepository
     : IRepository<Favorite, Guid>, IRepositoryAsync<Favorite, Guid>
    {

        
    }
}

