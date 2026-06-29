using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Repository.Interfaces.Catalog;
using FurnitureGardenDesign.Data.Repository.Interfaces.Interactions;
using FurnitureGardenDesign.Services.Core.Admin.Implementations.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using Moq;

namespace FurnitureGardenDesign.Unit.Tests.Services.Admin.Catalog
{
    [TestFixture]
    public class CatalogManagementServiceTests
    {
        private Mock<ICatalogRepository> _catalogRepositoryMock;
        private Mock<IFavoriteRepository> _favoriteRepositoryMock;
        private Mock<IReviewRepository> _reviewRepositoryMock;
        private CatalogManagementService _service;

        private Guid _testCatalogId;
        private Guid _testCategoryId;
        private CatalogDesign _testCatalog;
        private List<CatalogDesign> _testCatalogs;
        private Category _testCategory;

        [SetUp]
        public void SetUp()
        {
            _catalogRepositoryMock = new Mock<ICatalogRepository>(MockBehavior.Strict);
            _favoriteRepositoryMock = new Mock<IFavoriteRepository>(MockBehavior.Strict);
            _reviewRepositoryMock = new Mock<IReviewRepository>(MockBehavior.Strict);

            _service = new CatalogManagementService(
                _catalogRepositoryMock.Object,
                _favoriteRepositoryMock.Object,
                _reviewRepositoryMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _testCatalogId = Guid.NewGuid();
            _testCategoryId = Guid.NewGuid();

            _testCategory = new Category
            {
                Id = _testCategoryId,
                Name = "Test Category"
            };

            _testCatalog = new CatalogDesign
            {
                Id = _testCatalogId,
                Title = "Test Catalog",
                Description = "Test Description",
                Image2DUrl = "/images/test.jpg",
                Model3DUrl = "/models/test.glb",
                Materials = "Wood, Metal",
                Price = 99.99m,
                CategoryId = _testCategoryId,
                Category = _testCategory,
                IsDeleted = false,
                Model3DStatus = Model3DStatus.Ready,
                CreatedOn = DateTime.UtcNow
            };

            _testCatalogs = new List<CatalogDesign>
            {
                _testCatalog,
                new CatalogDesign
                {
                    Id = Guid.NewGuid(),
                    Title = "Active Catalog",
                    Description = "Active Description",
                    Image2DUrl = "/images/active.jpg",
                    Price = 49.99m,
                    CategoryId = _testCategoryId,
                    Category = _testCategory,
                    IsDeleted = false,
                    Model3DStatus = Model3DStatus.Generating,
                    CreatedOn = DateTime.UtcNow.AddDays(-1)
                },
                new CatalogDesign
                {
                    Id = Guid.NewGuid(),
                    Title = "Deleted Catalog",
                    Description = "Deleted Description",
                    Image2DUrl = "/images/deleted.jpg",
                    Price = 149.99m,
                    CategoryId = _testCategoryId,
                    Category = _testCategory,
                    IsDeleted = true,
                    Model3DStatus = Model3DStatus.None,
                    CreatedOn = DateTime.UtcNow.AddDays(-2)
                }
            };
        }

        #region AddCatalogAsync Tests

        [Test]
        public async Task AddCatalogAsync_CreatesAndAddsCatalog()
        {
            var model = new CatalogViewModelCreate
            {
                Title = "New Catalog",
                Description = "New Description",
                Image2DUrl = "/images/new.jpg",
                Model3DUrl = "/models/new.glb",
                Materials = "Glass, Steel",
                Price = "199.99",
                CategoryId = _testCategoryId,
                IsDeleted = false,
                Model3DStatus = Model3DStatus.Ready
            };

            _catalogRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<CatalogDesign>()))
                .Returns(Task.CompletedTask);

            await _service.AddCatalogAsync(model);

            _catalogRepositoryMock.Verify(r => r.AddAsync(It.Is<CatalogDesign>(c =>
                c.Title == "New Catalog"
               && c.Description == "New Description"
               && c.Image2DUrl == "/images/new.jpg"
               && c.Model3DUrl == "/models/new.glb"
               && c.Materials == "Glass, Steel"
               && c.Price == 199.99m
               && c.CategoryId == _testCategoryId
               && c.IsDeleted == false
               && c.Model3DStatus == Model3DStatus.Ready
               && c.Id != Guid.Empty)), Times.Once);
        }

        [Test]
        public async Task AddCatalogAsync_CreatesCatalogWithNullModel3DUrl()
        {
            var model = new CatalogViewModelCreate
            {
                Title = "No 3D Catalog",
                Description = "Description",
                Image2DUrl = "/images/no3d.jpg",
                Model3DUrl = null,
                Materials = "Wood",
                Price = "49.99",
                CategoryId = _testCategoryId,
                IsDeleted = false,
                Model3DStatus = Model3DStatus.None
            };

            _catalogRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<CatalogDesign>()))
                .Returns(Task.CompletedTask);

            await _service.AddCatalogAsync(model);

            _catalogRepositoryMock.Verify(r => r.AddAsync(It.Is<CatalogDesign>(c =>
                c.Model3DUrl == null
                && c.Model3DStatus == Model3DStatus.None)), Times.Once);
        }

        #endregion AddCatalogAsync Tests

        #region EditCatalogAsync Tests

        [Test]
        public async Task EditCatalogAsync_UpdatesExistingCatalog()
        {
            var existingCatalog = new CatalogDesign
            {
                Id = _testCatalogId,
                Title = "Old Title",
                Description = "Old Description",
                Image2DUrl = "/images/old.jpg",
                Model3DUrl = "/models/old.glb",
                Materials = "Old Materials",
                Price = 49.99m,
                CategoryId = _testCategoryId,
                IsDeleted = false,
                Model3DStatus = Model3DStatus.Generating
            };

            var model = new CatalogViewModelEdit
            {
                Id = _testCatalogId,
                Title = "Updated Title",
                Description = "Updated Description",
                Image2DUrl = "/images/updated.jpg",
                Model3DUrl = "/models/updated.glb",
                Materials = "Updated Materials",
                Price = "199.99",
                CategoryId = _testCategoryId,
                IsDeleted = true,
                Model3DStatus = Model3DStatus.Ready
            };

            _catalogRepositoryMock
                .Setup(r => r.GetByIdAsync(_testCatalogId))
                .ReturnsAsync(existingCatalog);

            _catalogRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<CatalogDesign>()))
                .ReturnsAsync(true);

            await _service.EditCatalogAsync(_testCatalogId, model);

            Assert.That(existingCatalog.Title, Is.EqualTo("Updated Title"));
            Assert.That(existingCatalog.Description, Is.EqualTo("Updated Description"));
            Assert.That(existingCatalog.Image2DUrl, Is.EqualTo("/images/updated.jpg"));
            Assert.That(existingCatalog.Model3DUrl, Is.EqualTo("/models/updated.glb"));
            Assert.That(existingCatalog.Materials, Is.EqualTo("Updated Materials"));
            Assert.That(existingCatalog.Price, Is.EqualTo(199.99m));
            Assert.That(existingCatalog.CategoryId, Is.EqualTo(_testCategoryId));
            Assert.That(existingCatalog.IsDeleted, Is.True);
            Assert.That(existingCatalog.Model3DStatus, Is.EqualTo(Model3DStatus.Ready));

            _catalogRepositoryMock.Verify(r => r.GetByIdAsync(_testCatalogId), Times.Once);
            _catalogRepositoryMock.Verify(r => r.UpdateAsync(existingCatalog), Times.Once);
        }

        [Test]
        public void EditCatalogAsync_ThrowsException_WhenCatalogNotFound()
        {
            var nonExistentId = Guid.NewGuid();
            var model = new CatalogViewModelEdit
            {
                Id = nonExistentId,
                Title = "Updated Title"
            };

            _catalogRepositoryMock
                .Setup(r => r.GetByIdAsync(nonExistentId))
                .ReturnsAsync((CatalogDesign)null!);

            var ex = Assert.ThrowsAsync<Exception>(
                async () => await _service.EditCatalogAsync(nonExistentId, model));

            Assert.That(ex.Message, Is.EqualTo("Catalog not found"));
            _catalogRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CatalogDesign>()), Times.Never);
        }

        [Test]
        public async Task EditCatalogAsync_UpdatesPartialData()
        {
            var existingCatalog = new CatalogDesign
            {
                Id = _testCatalogId,
                Title = "Old Title",
                Description = "Old Description",
                Image2DUrl = "/images/old.jpg",
                Model3DUrl = null,
                Materials = "Old Materials",
                Price = 49.99m,
                CategoryId = _testCategoryId,
                IsDeleted = false,
                Model3DStatus = Model3DStatus.None
            };

            var model = new CatalogViewModelEdit
            {
                Id = _testCatalogId,
                Title = "Updated Title Only",
                Description = "Old Description",
                Image2DUrl = "/images/old.jpg",
                Model3DUrl = null,
                Materials = "Old Materials",
                Price = "49.99",
                CategoryId = _testCategoryId,
                IsDeleted = false,
                Model3DStatus = Model3DStatus.None
            };

            _catalogRepositoryMock
                .Setup(r => r.GetByIdAsync(_testCatalogId))
                .ReturnsAsync(existingCatalog);

            _catalogRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<CatalogDesign>()))
                .ReturnsAsync(true);

            await _service.EditCatalogAsync(_testCatalogId, model);

            Assert.That(existingCatalog.Title, Is.EqualTo("Updated Title Only"));
            Assert.That(existingCatalog.Description, Is.EqualTo("Old Description"));
            Assert.That(existingCatalog.Model3DUrl, Is.Null);
        }

        #endregion EditCatalogAsync Tests

        #region GetAllActiveCataloguesAsync Tests

        [Test]
        public async Task GetAllActiveCataloguesAsync_ReturnsOnlyActiveCatalogs()
        {
            var activeCatalogs = _testCatalogs.Where(c => !c.IsDeleted).ToList();

            _catalogRepositoryMock
                .Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(activeCatalogs);

            var result = await _service.GetAllActiveCataloguesAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(c => !c.IsDeleted), Is.True);
            Assert.That(result.First().Id, Is.EqualTo(_testCatalogId));
            Assert.That(result.First().Title, Is.EqualTo("Test Catalog"));

            _catalogRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllActiveCataloguesAsync_ReturnsEmptyList_WhenNoActiveCatalogs()
        {
            var emptyList = new List<CatalogDesign>();

            _catalogRepositoryMock
                .Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(emptyList);

            var result = await _service.GetAllActiveCataloguesAsync();

            Assert.That(result, Is.Empty);
        }

        #endregion GetAllActiveCataloguesAsync Tests

        #region GetAllCataloguesForAdminAsync Tests

        [Test]
        public async Task GetAllCataloguesForAdminAsync_ReturnsAllCatalogsIncludingDeleted()
        {
            _catalogRepositoryMock
                .Setup(r => r.GetAllForAdminAsync())
                .ReturnsAsync(_testCatalogs);

            var result = await _service.GetAllCataloguesForAdminAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result.Any(c => c.IsDeleted), Is.True);

            _catalogRepositoryMock.Verify(r => r.GetAllForAdminAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllCataloguesForAdminAsync_ReturnsCorrectProperties()
        {
            _catalogRepositoryMock
                .Setup(r => r.GetAllForAdminAsync())
                .ReturnsAsync(_testCatalogs);

            var result = await _service.GetAllCataloguesForAdminAsync();

            var catalog = result.First(c => c.Id == _testCatalogId);

            Assert.That(catalog.Id, Is.EqualTo(_testCatalogId));
            Assert.That(catalog.Title, Is.EqualTo("Test Catalog"));
            Assert.That(catalog.CategoryName, Is.EqualTo("Test Category"));
            Assert.That(catalog.Price, Is.EqualTo(99.99m));
            Assert.That(catalog.Model3DStatus, Is.EqualTo(Model3DStatus.Ready));
            Assert.That(catalog.IsDeleted, Is.False);
        }

        [Test]
        public async Task GetAllCataloguesForAdminAsync_ReturnsEmptyList_WhenNoCatalogs()
        {
            var emptyList = new List<CatalogDesign>();

            _catalogRepositoryMock
                .Setup(r => r.GetAllForAdminAsync())
                .ReturnsAsync(emptyList);

            var result = await _service.GetAllCataloguesForAdminAsync();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllCataloguesForAdminAsync_HandlesNullCategory()
        {
            var catalogWithNullCategory = new CatalogDesign
            {
                Id = Guid.NewGuid(),
                Title = "No Category",
                Price = 29.99m,
                Category = null,
                IsDeleted = false,
                Model3DStatus = Model3DStatus.None,
                CreatedOn = DateTime.UtcNow
            };

            var catalogs = new List<CatalogDesign> { catalogWithNullCategory };

            _catalogRepositoryMock
                .Setup(r => r.GetAllForAdminAsync())
                .ReturnsAsync(catalogs);

            var result = await _service.GetAllCataloguesForAdminAsync();

            Assert.That(result.First().CategoryName, Is.Null);
        }

        #endregion GetAllCataloguesForAdminAsync Tests

        #region GetCatalogForEditByIdAsync Tests

        [Test]
        public async Task GetCatalogForEditByIdAsync_ReturnsCatalog_WhenExists()
        {
            _catalogRepositoryMock
                .Setup(r => r.GetByIdAsync(_testCatalogId))
                .ReturnsAsync(_testCatalog);

            var result = await _service.GetCatalogForEditByIdAsync(_testCatalogId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testCatalogId));
            Assert.That(result.Title, Is.EqualTo("Test Catalog"));
            Assert.That(result.Description, Is.EqualTo("Test Description"));
            Assert.That(result.Image2DUrl, Is.EqualTo("/images/test.jpg"));
            Assert.That(result.Model3DUrl, Is.EqualTo("/models/test.glb"));
            Assert.That(result.Materials, Is.EqualTo("Wood, Metal"));
            Assert.That(result.Price, Is.EqualTo("99.99"));
            Assert.That(result.CategoryId, Is.EqualTo(_testCategoryId));
            Assert.That(result.IsDeleted, Is.False);
            Assert.That(result.Model3DStatus, Is.EqualTo(Model3DStatus.Ready));

            _catalogRepositoryMock.Verify(r => r.GetByIdAsync(_testCatalogId), Times.Once);
        }

        [Test]
        public async Task GetCatalogForEditByIdAsync_ReturnsNull_WhenCatalogNotFound()
        {
            var nonExistentId = Guid.NewGuid();
            _catalogRepositoryMock
                .Setup(r => r.GetByIdAsync(nonExistentId))
                .ReturnsAsync((CatalogDesign)null!);

            var result = await _service.GetCatalogForEditByIdAsync(nonExistentId);

            Assert.That(result, Is.Null);
        }

        #endregion GetCatalogForEditByIdAsync Tests

        #region ToggleCatalogAsync Tests

        [Test]
        public async Task ToggleCatalogAsync_TogglesCatalogStatus()
        {
            var catalog = new CatalogDesign
            {
                Id = _testCatalogId,
                IsDeleted = false
            };

            _catalogRepositoryMock
                .Setup(r => r.GetByIdIncludingDeletedAsync(_testCatalogId))
                .ReturnsAsync(catalog);

            _catalogRepositoryMock
                .Setup(r => r.ToggleCatalogStatusAsync(catalog))
                .Returns(Task.CompletedTask);

            await _service.ToggleCatalogAsync(_testCatalogId);

            _catalogRepositoryMock.Verify(r => r.GetByIdIncludingDeletedAsync(_testCatalogId), Times.Once);
            _catalogRepositoryMock.Verify(r => r.ToggleCatalogStatusAsync(catalog), Times.Once);
        }

        [Test]
        public void ToggleCatalogAsync_ThrowsException_WhenCatalogNotFound()
        {
            var nonExistentId = Guid.NewGuid();

            _catalogRepositoryMock
                .Setup(r => r.GetByIdIncludingDeletedAsync(nonExistentId))
                .ReturnsAsync((CatalogDesign)null!);

            var ex = Assert.ThrowsAsync<Exception>(
                async () => await _service.ToggleCatalogAsync(nonExistentId));

            Assert.That(ex.Message, Is.EqualTo("Catalog not found"));
            _catalogRepositoryMock.Verify(r => r.ToggleCatalogStatusAsync(It
                .IsAny<CatalogDesign>()), Times.Never);
        }

        #endregion ToggleCatalogAsync Tests
    }
}