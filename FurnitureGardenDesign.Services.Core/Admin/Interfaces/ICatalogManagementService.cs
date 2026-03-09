using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Admin.Interfaces
{
    public interface ICatalogManagementService: ICatalogService
    {
        Task<IEnumerable<CatalogViewModelList>> GetAllActiveCataloguesAsync();

        Task AddCatalogAsync(CatalogViewModelCreate model);

        Task<CatalogViewModelEdit?> GetCatalogForEditByIdAsync(Guid id);
        Task EditCatalogAsync(Guid id, CatalogViewModelEdit model);

        Task ToggleCatalogAsync(Guid id);

        Task<IEnumerable<CatalogViewModelList>> GetAllCataloguesForAdminAsync();
    }
}
