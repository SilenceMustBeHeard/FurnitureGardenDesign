using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Services.Core.Implementations.Catalog;
using FurnitureGardenDesign.Data.Repository.Interfaces.Catalog;
using FurnitureGardenDesign.Data.Repository.Interfaces.Interactions;

namespace FurnitureGardenDesign.Services.Core.Admin.Implementations.Catalog
{
    public class CatalogManagementService : CatalogService, ICatalogManagementService
    {
        private readonly ICatalogRepository _catalogRepository;
        public CatalogManagementService(ICatalogRepository catalogRepo, 
            IFavoriteRepository favoriteRepo, 
            IReviewRepository reviewRepo) 
            : base(catalogRepo, favoriteRepo, reviewRepo)
        {
            _catalogRepository = catalogRepo;
        }

        // adds new catalog design 
        public async Task AddCatalogAsync(CatalogViewModelCreate model)
        {
            var catalog = new CatalogDesign
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                Description = model.Description,
                Image2DUrl = model.Image2DUrl,
                Model3DUrl = model.Model3DUrl,
                Materials = model.Materials,
                Price = decimal.Parse(model.Price),
                CategoryId = model.CategoryId,
                IsDeleted = model.IsDeleted,
                Model3DStatus = model.Model3DStatus
            };


            await _catalogRepository.AddAsync(catalog);
        }


        // edits an existing catalog design 
        public async Task EditCatalogAsync(Guid id, CatalogViewModelEdit model)
        {
            var catalog = await _catalogRepository.GetByIdAsync(id);
            if (catalog == null)
            {
                throw new Exception("Catalog not found");
            }

            catalog.Title = model.Title;
            catalog.Description = model.Description;
            catalog.Image2DUrl = model.Image2DUrl;
            catalog.Model3DUrl = model.Model3DUrl;
            catalog.Materials = model.Materials;
            catalog.Price = decimal.Parse(model.Price);
            catalog.CategoryId = model.CategoryId;
            catalog.IsDeleted = model.IsDeleted;
            catalog.Model3DStatus = model.Model3DStatus;

            await _catalogRepository.UpdateAsync(catalog); 
        }


        // retrieves all active catalog designs for display in the admin panel
        public async Task<IEnumerable<CatalogViewModelList>> GetAllActiveCataloguesAsync()
        {

            var catalogues = await _catalogRepository.GetAllActiveAsync();

            return catalogues.Select(c => new CatalogViewModelList
            {
                Id = c.Id,
                Title = c.Title,
                IsDeleted = c.IsDeleted
            });



        }


        // retrieves all catalog designs, including deleted ones
        public async Task<IEnumerable<CatalogViewModelList>> GetAllCataloguesForAdminAsync()
        {
            var catalogues = await _catalogRepository.GetAllForAdminAsync();

            return catalogues.Select(c => new CatalogViewModelList
            {
                Id = c.Id,
                Title = c.Title,
                CategoryName = c.Category?.Name, 
                Price = c.Price,
                Model3DStatus = c.Model3DStatus,
                IsDeleted = c.IsDeleted
            });
        }


        // retrieves a specific catalog design by its ID
        // Fixed version
        public async Task<CatalogViewModelEdit?> GetCatalogForEditByIdAsync(Guid id)
        {
            var catalog = await _catalogRepository.GetByIdAsync(id);
            if (catalog == null)
                return null;

            return new CatalogViewModelEdit
            {
                Id = catalog.Id,
                Title = catalog.Title,
                Description = catalog.Description,
                Image2DUrl = catalog.Image2DUrl,
                Model3DUrl = catalog.Model3DUrl,
                Materials = catalog.Materials,
                Price = catalog.Price.ToString(),
                CategoryId = catalog.CategoryId,
                IsDeleted = catalog.IsDeleted,
                Model3DStatus = catalog.Model3DStatus
            };
        }
        // toggles the active/deleted status of a catalog design
        public async Task ToggleCatalogAsync(Guid id)
        {
            var catalog = await _catalogRepository.GetByIdIncludingDeletedAsync(id);

            if (catalog == null)
            {
                throw new Exception("Catalog not found");
            }

            await _catalogRepository.ToggleCatalogStatusAsync(catalog);
        }
    }
}
