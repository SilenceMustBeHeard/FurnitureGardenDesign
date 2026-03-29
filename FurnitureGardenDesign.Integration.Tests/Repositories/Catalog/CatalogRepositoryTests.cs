using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Implementations.Catalog;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Tests.Integration.Repositories.Catalog
{
    [TestFixture]
    public class CatalogRepositoryTests
    {
        private ApplicationDbContext _context;
        private CatalogRepository _repository;
        private Guid _testCategoryId;
        private Guid _testCatalogId;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new CatalogRepository(_context);
            _testCategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            _testCatalogId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            SeedTestData();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void SeedTestData()
        {
            var category = new Category
            {
                Id = _testCategoryId,
                Name = "Test Category",
                Description = "Test Description",
                IsDeleted = false
            };

            var catalogs = new[]
            {
                new CatalogDesign
                {
                    Id = _testCatalogId,
                    Title = "Active Catalog 1",
                    Description = "Description 1",
                    Image2DUrl = "/images/test1.jpg",
                    Price = 99.99m,
                    CategoryId = _testCategoryId,
                    Category = category,
                    IsDeleted = false,
                    CreatedOn = DateTime.UtcNow
                },

                new CatalogDesign
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Title = "Active Catalog 2",
                    Description = "Description 2",
                    Image2DUrl = "/images/test2.jpg",
                    Price = 149.99m,
                    CategoryId = _testCategoryId,
                    Category = category,
                    IsDeleted = false,
                    CreatedOn = DateTime.UtcNow.AddDays(-1)
                },

                new CatalogDesign
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Title = "Deleted Catalog",
                    Description = "Deleted Description",
                    Image2DUrl = "/images/deleted.jpg",
                    Price = 49.99m,
                    CategoryId = _testCategoryId,
                    Category = category,
                    IsDeleted = true,
                    CreatedOn = DateTime.UtcNow.AddDays(-2)
                }
            };

            _context.Categories.Add(category);
            _context.CatalogDesigns.AddRange(catalogs);
            _context.SaveChanges();
        }

        #region GetAllActiveAsync Tests

        [Test]
        public async Task GetAllActiveAsync_ReturnsOnlyActiveCatalogs()
        {
            var result = await _repository.GetAllActiveAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(c => !c.IsDeleted), Is.True);
            Assert.That(result.Select(c => c.Title), Contains.Item("Active Catalog 1"));
            Assert.That(result.Select(c => c.Title), Contains.Item("Active Catalog 2"));
            Assert.That(result.Select(c => c.Title), Does.Not.Contain("Deleted Catalog"));
        }

        [Test]
        public async Task GetAllActiveAsync_IncludesCategoryNavigation()
        {
            var result = await _repository.GetAllActiveAsync();

            var catalog = result.First();

            Assert.That(catalog.Category, Is.Not.Null);
            Assert.That(catalog.Category.Name, Is.EqualTo("Test Category"));
        }

        [Test]
        public async Task GetAllActiveAsync_ReturnsEmptyList_WhenNoActiveCatalogs()
        {
            foreach (var catalog in _context.CatalogDesigns)
            {
                catalog.IsDeleted = true;
            }
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllActiveAsync();

            Assert.That(result, Is.Empty);
        }

        #endregion GetAllActiveAsync Tests

        #region GetByIdWithReviewsAsync Tests

        [Test]
        public async Task GetByIdWithReviewsAsync_ReturnsCatalogWithReviews_WhenExists()
        {
            // Arrange
            var review = new Review
            {
                Id = Guid.NewGuid(),
                CatalogDesignId = _testCatalogId,
                UserId = "user-123",
                Rating = 5,
                Comment = "Great product!",
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Reviews.Add(review);

            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdWithReviewsAsync(_testCatalogId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testCatalogId));
            Assert.That(result.Title, Is.EqualTo("Active Catalog 1"));
            Assert.That(result.Reviews, Is.Not.Null);
            Assert.That(result.Reviews.Count, Is.EqualTo(1));
            Assert.That(result.Reviews.First().Rating, Is.EqualTo(5));
            Assert.That(result.Reviews.First().Comment, Is.EqualTo("Great product!"));
        }

        [Test]
        public async Task GetByIdWithReviewsAsync_IncludesUserInReviews()
        {
            var user = new AppUser
            {
                Id = "user-123",
                Email = "test@example.com",
                UserName = "testuser"
            };

            var review = new Review
            {
                Id = Guid.NewGuid(),
                CatalogDesignId = _testCatalogId,
                UserId = "user-123",
                User = user,
                Rating = 5,
                Comment = "Great!",
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Users.Add(user);
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdWithReviewsAsync(_testCatalogId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Reviews.First().User, Is.Not.Null);
            Assert.That(result.Reviews.First().User.Email, Is.EqualTo("test@example.com"));
        }

        [Test]
        public async Task GetByIdWithReviewsAsync_ReturnsNull_WhenCatalogDoesNotExist()
        {
            // Act
            var result = await _repository.GetByIdWithReviewsAsync(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByIdWithReviewsAsync_ReturnsCatalogWithoutReviews_WhenNoReviewsExist()
        {
            var result = await _repository.GetByIdWithReviewsAsync(_testCatalogId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Reviews, Is.Empty);
        }

        #endregion GetByIdWithReviewsAsync Tests

        #region GetAllForAdminAsync Tests

        [Test]
        public async Task GetAllForAdminAsync_ReturnsAllCatalogsIncludingDeleted()
        {
            var result = await _repository.GetAllForAdminAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result.Any(c => c.IsDeleted), Is.True);
            Assert.That(result.Select(c => c.Title), Contains.Item("Deleted Catalog"));
        }

        [Test]
        public async Task GetAllForAdminAsync_IncludesCategoryNavigation()
        {
            var result = await _repository.GetAllForAdminAsync();

            var catalog = result.First(c => c.Id == _testCatalogId);

            Assert.That(catalog.Category, Is.Not.Null);
            Assert.That(catalog.Category.Name, Is.EqualTo("Test Category"));
        }

        [Test]
        public async Task GetAllForAdminAsync_OrdersByCreatedOnDescending()
        {
            var result = await _repository.GetAllForAdminAsync();
            var resultList = result.ToList();

            Assert.That(resultList[0].CreatedOn, Is.GreaterThan(resultList[1].CreatedOn));
            Assert.That(resultList[1].CreatedOn, Is.GreaterThan(resultList[2].CreatedOn));
        }

        #endregion GetAllForAdminAsync Tests

        #region GetByName Tests

        [Test]
        public void GetByName_ReturnsCatalog_WhenNameMatches()
        {
            var result = _repository.GetByName("Active Catalog 1");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testCatalogId));
            Assert.That(result.Title, Is.EqualTo("Active Catalog 1"));
        }

        [Test]
        public void GetByName_ReturnsNull_WhenNameDoesNotMatch()
        {
            var result = _repository.GetByName("Nonexistent Catalog");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetByName_IsCaseSensitive()
        {
            var result = _repository.GetByName("ACTIVE CATALOG 1");

            Assert.That(result, Is.Null);
        }

        #endregion GetByName Tests

        #region ToggleCatalogStatusAsync Tests

        [Test]
        public async Task ToggleCatalogStatusAsync_TogglesIsDeletedFlag()
        {
            var catalog = await _context.CatalogDesigns.FirstAsync(c => c.Id == _testCatalogId);
            var initialStatus = catalog.IsDeleted;

            await _repository.ToggleCatalogStatusAsync(catalog);

            Assert.That(catalog.IsDeleted, Is.EqualTo(!initialStatus));
        }

        [Test]
        public async Task ToggleCatalogStatusAsync_ChangesFromFalseToTrue()
        {
            var catalog = await _context.CatalogDesigns.FirstAsync(c => c.Id == _testCatalogId);

            Assert.That(catalog.IsDeleted, Is.False);

            await _repository.ToggleCatalogStatusAsync(catalog);

            Assert.That(catalog.IsDeleted, Is.True);
        }

        #endregion ToggleCatalogStatusAsync Tests

        #region GetByIdIncludingDeletedAsync Tests

        [Test]
        public async Task GetByIdIncludingDeletedAsync_ReturnsActiveCatalog_WhenExists()
        {
            var result = await _repository.GetByIdIncludingDeletedAsync(_testCatalogId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testCatalogId));
            Assert.That(result.IsDeleted, Is.False);
        }

        [Test]
        public async Task GetByIdIncludingDeletedAsync_ReturnsDeletedCatalog_WhenExists()
        {
            var deletedId = Guid.Parse("44444444-4444-4444-4444-444444444444");

            var result = await _repository.GetByIdIncludingDeletedAsync(deletedId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Deleted Catalog"));
            Assert.That(result.IsDeleted, Is.True);
        }

        [Test]
        public async Task GetByIdIncludingDeletedAsync_IncludesCategoryNavigation()
        {
            var result = await _repository.GetByIdIncludingDeletedAsync(_testCatalogId);

            Assert.That(result.Category, Is.Not.Null);
            Assert.That(result.Category.Name, Is.EqualTo("Test Category"));
        }

        [Test]
        public async Task GetByIdIncludingDeletedAsync_ReturnsNull_WhenCatalogDoesNotExist()
        {
            var result = await _repository.GetByIdIncludingDeletedAsync(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        #endregion GetByIdIncludingDeletedAsync Tests

        #region Edge Cases and Validation Tests

        [Test]
        public async Task GetAllActiveAsync_DoesNotReturnSoftDeletedCatalogs()
        {
            var result = await _repository.GetAllActiveAsync();

            Assert.That(result.Any(c => c.IsDeleted), Is.False);
        }

        #endregion Edge Cases and Validation Tests
    }
}