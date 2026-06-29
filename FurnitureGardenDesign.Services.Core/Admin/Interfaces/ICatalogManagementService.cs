using FurnitureGardenDesign.Services.Core.Interfaces.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Catalog;

namespace FurnitureGardenDesign.Services.Core.Admin.Interfaces;

public interface ICatalogManagementService : ICatalogService
{
    Task<IEnumerable<CatalogViewModelList>> GetAllActiveCataloguesAsync();

    Task AddCatalogAsync(CatalogViewModelCreate model);

    Task<CatalogViewModelEdit?> GetCatalogForEditByIdAsync(Guid id);

    Task EditCatalogAsync(Guid id, CatalogViewModelEdit model);

    Task ToggleCatalogAsync(Guid id);

    Task<IEnumerable<CatalogViewModelList>> GetAllCataloguesForAdminAsync();
}