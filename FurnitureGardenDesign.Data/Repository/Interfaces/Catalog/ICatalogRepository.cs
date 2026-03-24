using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Repository.Interfaces.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Interfaces.Catalog
{
    public interface ICatalogRepository :
        IRepository<CatalogDesign, Guid>, IRepositoryAsync<CatalogDesign, Guid>
    {

        Task<IEnumerable<CatalogDesign>> GetAllActiveAsync();
            Task<CatalogDesign?> GetByIdWithReviewsAsync(Guid id);
        CatalogDesign? GetByName(string name);
        Task<IEnumerable<CatalogDesign>> GetAllForAdminAsync();
        Task ToggleCatalogStatusAsync(CatalogDesign catalog);
        Task<CatalogDesign?> GetByIdIncludingDeletedAsync(Guid id);












    }
}
