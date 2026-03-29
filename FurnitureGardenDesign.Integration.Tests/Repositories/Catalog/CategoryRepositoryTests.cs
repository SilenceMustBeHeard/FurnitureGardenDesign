using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Repository.Implementations.Catalog;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Tests.Integration.Repositories.Catalog
{
    [TestFixture]
    public class CategoryRepositoryTests
    {
        private ApplicationDbContext _context;
        private CategoryRepository _repository;
        private Guid _activeCategoryId;
        private Guid _deletedCategoryId;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new CategoryRepository(_context);
            _activeCategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            _deletedCategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

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
            var categories = new[]
            {
                new Category
                {
                    Id = _activeCategoryId,
                    Name = "Active Category 1",
                    Description = "First active category",
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Active Category 2",
                    Description = "Second active category",
                    IsDeleted = false
                },
                new Category
                {
                    Id = _deletedCategoryId,
                    Name = "Deleted Category",
                    Description = "This category is soft deleted",
                    IsDeleted = true
                },
                new Category
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "Another Active Category",
                    Description = "Third active category",
                    IsDeleted = false
                }
            };

            _context.Categories.AddRange(categories);
            _context.SaveChanges();
        }

        #region GetAllActiveAsync Tests

        [Test]
        public async Task GetAllActiveAsync_ReturnsOnlyActiveCategories()
        {
            var result = await _repository.GetAllActiveAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result.All(c => !c.IsDeleted), Is.True);
            Assert.That(result.Select(c => c.Name), Contains.Item("Active Category 1"));
            Assert.That(result.Select(c => c.Name), Contains.Item("Active Category 2"));
            Assert.That(result.Select(c => c.Name), Contains.Item("Another Active Category"));
            Assert.That(result.Select(c => c.Name), Does.Not.Contain("Deleted Category"));
        }

        [Test]
        public async Task GetAllActiveAsync_ReturnsEmptyList_WhenNoActiveCategories()
        {
            foreach (var category in _context.Categories)
            {
                category.IsDeleted = true;
            }
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllActiveAsync();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllActiveAsync_ReturnsCategoriesOrderedById()
        {
            var result = await _repository.GetAllActiveAsync();
            var resultList = result.ToList();

            Assert.That(resultList[0].Id, Is.EqualTo(_activeCategoryId));
        }

        #endregion GetAllActiveAsync Tests

        #region GetAllForAdminAsync Tests

        [Test]
        public async Task GetAllForAdminAsync_ReturnsAllCategoriesIncludingDeleted()
        {
            var result = await _repository.GetAllForAdminAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(4));
            Assert.That(result.Any(c => c.IsDeleted), Is.True);
            Assert.That(result.Select(c => c.Name), Contains.Item("Deleted Category"));
        }

        [Test]
        public async Task GetAllForAdminAsync_ReturnsNotEmptyList_WhenNoCategories()
        {
            _context.Categories.RemoveRange(_context.Categories);

            await _context.SaveChangesAsync();

            var result = await _repository.GetAllForAdminAsync();

            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public async Task GetAllForAdminAsync_IncludesDeletedCategoriesWithCorrectFlag()
        {
            var result = await _repository.GetAllForAdminAsync();

            var deletedCategory = result.First(c => c.Id == _deletedCategoryId);
            Assert.That(deletedCategory.IsDeleted, Is.True);
            Assert.That(deletedCategory.Name, Is.EqualTo("Deleted Category"));
        }

        #endregion GetAllForAdminAsync Tests

        #region GetByName Tests

        [Test]
        public void GetByName_ReturnsCategory_WhenNameMatchesExactly()
        {
            var result = _repository.GetByName("Active Category 1");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_activeCategoryId));
            Assert.That(result.Name, Is.EqualTo("Active Category 1"));
        }

        [Test]
        public void GetByName_ReturnsNull_WhenNameDoesNotMatch()
        {
            var result = _repository.GetByName("Nonexistent Category");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetByName_IsCaseSensitive()
        {
            var result = _repository.GetByName("ACTIVE CATEGORY 1");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByName_ReturnsFirstMatch_WhenMultipleWithSameName()
        {
            var duplicateCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Duplicate Name",
                Description = "Duplicate",
                IsDeleted = false
            };
            _context.Categories.Add(duplicateCategory);
            await _context.SaveChangesAsync();

            var result = _repository.GetByName("Duplicate Name");

            Assert.That(result, Is.Not.Null);
        }

        #endregion GetByName Tests

        #region ToggleCategoryStatusAsync Tests

        [Test]
        public async Task ToggleCategoryStatusAsync_TogglesIsDeletedFlag()
        {
            var category = await _context.Categories.FirstAsync(c => c.Id == _activeCategoryId);
            var initialStatus = category.IsDeleted;

            await _repository.ToggleCategoryStatusAsync(category);

            Assert.That(category.IsDeleted, Is.EqualTo(!initialStatus));
        }

        [Test]
        public async Task ToggleCategoryStatusAsync_ChangesFromFalseToTrue()
        {
            var category = await _context.Categories.FirstAsync(c => c.Id == _activeCategoryId);
            Assert.That(category.IsDeleted, Is.False);

            await _repository.ToggleCategoryStatusAsync(category);

            Assert.That(category.IsDeleted, Is.True);
        }

        #endregion ToggleCategoryStatusAsync Tests

        #region GetByIdIncludingDeletedAsync Tests

        [Test]
        public async Task GetByIdIncludingDeletedAsync_ReturnsActiveCategory_WhenExists()
        {
            var result = await _repository.GetByIdIncludingDeletedAsync(_activeCategoryId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_activeCategoryId));
            Assert.That(result.Name, Is.EqualTo("Active Category 1"));
            Assert.That(result.IsDeleted, Is.False);
        }

        [Test]
        public async Task GetByIdIncludingDeletedAsync_ReturnsDeletedCategory_WhenExists()
        {
            var result = await _repository.GetByIdIncludingDeletedAsync(_deletedCategoryId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Deleted Category"));
            Assert.That(result.IsDeleted, Is.True);
        }

        [Test]
        public async Task GetByIdIncludingDeletedAsync_ReturnsNull_WhenCategoryDoesNotExist()
        {
            var result = await _repository.GetByIdIncludingDeletedAsync(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByIdIncludingDeletedAsync_IncludesAllProperties()
        {
            var result = await _repository.GetByIdIncludingDeletedAsync(_activeCategoryId);

            Assert.That(result.Id, Is.EqualTo(_activeCategoryId));
            Assert.That(result.Name, Is.EqualTo("Active Category 1"));
            Assert.That(result.Description, Is.EqualTo("First active category"));
            Assert.That(result.IsDeleted, Is.False);
        }

        #endregion GetByIdIncludingDeletedAsync Tests

        #region Edge Cases and Validation Tests

        [Test]
        public async Task GetAllActiveAsync_DoesNotReturnSoftDeletedCategories()
        {
            var result = await _repository.GetAllActiveAsync();

            Assert.That(result.Any(c => c.IsDeleted), Is.False);
        }

        [Test]
        public async Task ToggleCategoryStatusAsync_CanBeCalledMultipleTimes()
        {
            var category = await _context.Categories.FirstAsync(c => c.Id == _activeCategoryId);

            await _repository.ToggleCategoryStatusAsync(category);
            await _repository.ToggleCategoryStatusAsync(category);

            Assert.That(category.IsDeleted, Is.False);
        }

        [Test]
        public async Task Repository_HandlesLargeNumberOfCategories()
        {
            for (int i = 0; i < 100; i++)
            {
                _context.Categories.Add(new Category
                {
                    Id = Guid.NewGuid(),
                    Name = $"Category {i}",
                    Description = $"Description {i}",
                    IsDeleted = false
                });
            }
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllActiveAsync();

            Assert.That(result.Count(), Is.EqualTo(103));
        }

        #endregion Edge Cases and Validation Tests
    }
}