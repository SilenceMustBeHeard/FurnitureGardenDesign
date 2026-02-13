using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Interfaces
{
    public interface ICatalogRepository :
        IRepository<CatalogDesign, Guid>, IRepositoryAsync<CatalogDesign, Guid>
    {
            Task<CatalogDesign?> GetByIdWithReviewsAsync(Guid id);
    }
}
