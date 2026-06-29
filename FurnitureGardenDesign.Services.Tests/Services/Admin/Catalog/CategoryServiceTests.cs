using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Repository.Interfaces.Catalog;
using FurnitureGardenDesign.Services.Core.Admin.Implementations.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Category;
using Moq;

namespace FurnitureGardenDesign.Unit.Tests.Services.Admin.Catalog
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private Mock<ICategoryRepository> _categoryRepositoryMock;
        private CategoryService _service;

        private Guid _testCategoryId;
        private Category _testCategory;
        private List<Category> _testCategories;

        [SetUp]
        public void SetUp()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>(MockBehavior.Strict);
            _service = new CategoryService(_categoryRepositoryMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _testCategoryId = Guid.NewGuid();

            _testCategory = new Category
            {
                Id = _testCategoryId,
                Name = "Test Category",
                Description = "Test Description",
                IsDeleted = false
            };

            _testCategories = new List<Category>
            {
                _testCategory,
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Active Category",
                    Description = "Active Description",
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Deleted Category",
                    Description = "Deleted Description",
                    IsDeleted = true
                }
            };
        }

        #region GetAllActiveCategoriesAsync Tests

        [Test]
        public async Task GetAllActiveCategoriesAsync_ReturnsOnlyActiveCategories()
        {
            // Arrange
            var activeCategories = _testCategories.Where(c => !c.IsDeleted).ToList();
            _categoryRepositoryMock
                .Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(activeCategories);

            // Act
            var result = await _service.GetAllActiveCategoriesAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(c => !c.IsDeleted), Is.True);
            Assert.That(result.First().Id, Is.EqualTo(_testCategoryId));
            Assert.That(result.First().Name, Is.EqualTo("Test Category"));
            _categoryRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllActiveCategoriesAsync_ReturnsEmptyList_WhenNoActiveCategories()
        {
            // Arrange
            var emptyList = new List<Category>();
            _categoryRepositoryMock
                .Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(emptyList);

            // Act
            var result = await _service.GetAllActiveCategoriesAsync();

            // Assert
            Assert.That(result, Is.Empty);
            _categoryRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once);
        }

        #endregion GetAllActiveCategoriesAsync Tests

        #region GetAllCategoriesForAdminAsync Tests

        [Test]
        public async Task GetAllCategoriesForAdminAsync_ReturnsAllCategoriesIncludingDeleted()
        {
            // Arrange
            _categoryRepositoryMock
                .Setup(r => r.GetAllForAdminAsync())
                .ReturnsAsync(_testCategories);

            // Act
            var result = await _service.GetAllCategoriesForAdminAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result.Any(c => c.IsDeleted), Is.True);
            _categoryRepositoryMock.Verify(r => r.GetAllForAdminAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllCategoriesForAdminAsync_ReturnsCorrectProperties()
        {
            // Arrange
            _categoryRepositoryMock
                .Setup(r => r.GetAllForAdminAsync())
                .ReturnsAsync(_testCategories);

            // Act
            var result = await _service.GetAllCategoriesForAdminAsync();

            // Assert
            var category = result.First(c => c.Id == _testCategoryId);
            Assert.That(category.Id, Is.EqualTo(_testCategoryId));
            Assert.That(category.Name, Is.EqualTo("Test Category"));
            Assert.That(category.IsDeleted, Is.False);
        }

        [Test]
        public async Task GetAllCategoriesForAdminAsync_ReturnsEmptyList_WhenNoCategories()
        {
            // Arrange
            var emptyList = new List<Category>();
            _categoryRepositoryMock
                .Setup(r => r.GetAllForAdminAsync())
                .ReturnsAsync(emptyList);

            // Act
            var result = await _service.GetAllCategoriesForAdminAsync();

            // Assert
            Assert.That(result, Is.Empty);
            _categoryRepositoryMock.Verify(r => r.GetAllForAdminAsync(), Times.Once);
        }

        #endregion GetAllCategoriesForAdminAsync Tests

        #region AddCategoryAsync Tests

        [Test]
        public async Task AddCategoryAsync_CreatesAndAddsCategory()
        {
            // Arrange
            var model = new CategoryViewModelCreate
            {
                Name = "New Category",
                Description = "New Description"
            };

            _categoryRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Category>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.AddCategoryAsync(model);

            // Assert
            _categoryRepositoryMock.Verify(r => r.AddAsync(It.Is<Category>(c =>
                c.Name == "New Category" &&
                c.Description == "New Description" &&
                c.IsDeleted == false &&
                c.Id != Guid.Empty)), Times.Once);
        }

        [Test]
        public async Task AddCategoryAsync_CreatesCategoryWithGeneratedId()
        {
            // Arrange
            var model = new CategoryViewModelCreate
            {
                Name = "Generated ID Test",
                Description = "Test Description"
            };

            Category capturedCategory = null;

            _categoryRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Category>()))
                .Callback<Category>(c => capturedCategory = c)
                .Returns(Task.CompletedTask);

            // Act
            await _service.AddCategoryAsync(model);

            // Assert
            Assert.That(capturedCategory, Is.Not.Null);
            Assert.That(capturedCategory.Id, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public async Task AddCategoryAsync_CreatesCategoryWithDefaultIsDeleted()
        {
            // Arrange
            var model = new CategoryViewModelCreate
            {
                Name = "Default IsDeleted Test",
                Description = "Test Description"
            };

            Category capturedCategory = null;

            _categoryRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Category>()))
                .Callback<Category>(c => capturedCategory = c)
                .Returns(Task.CompletedTask);

            // Act
            await _service.AddCategoryAsync(model);

            // Assert
            Assert.That(capturedCategory, Is.Not.Null);
            Assert.That(capturedCategory.IsDeleted, Is.False);
        }

        #endregion AddCategoryAsync Tests

        #region GetCategoryForEditByIdAsync Tests

        [Test]
        public async Task GetCategoryForEditByIdAsync_ReturnsCategory_WhenExists()
        {
            // Arrange
            _categoryRepositoryMock
                .Setup(r => r.GetByIdIncludingDeletedAsync(_testCategoryId))
                .ReturnsAsync(_testCategory);

            // Act
            var result = await _service.GetCategoryForEditByIdAsync(_testCategoryId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testCategoryId));
            Assert.That(result.Name, Is.EqualTo("Test Category"));
            Assert.That(result.Description, Is.EqualTo("Test Description"));
            Assert.That(result.IsDeleted, Is.False);
            _categoryRepositoryMock.Verify(r => r.GetByIdIncludingDeletedAsync(_testCategoryId), Times.Once);
        }

        [Test]
        public async Task GetCategoryForEditByIdAsync_ReturnsDeletedCategory_WhenExists()
        {
            // Arrange
            var deletedCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Deleted Category",
                Description = "Deleted Description",
                IsDeleted = true
            };

            _categoryRepositoryMock
                .Setup(r => r.GetByIdIncludingDeletedAsync(deletedCategory.Id))
                .ReturnsAsync(deletedCategory);

            // Act
            var result = await _service.GetCategoryForEditByIdAsync(deletedCategory.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsDeleted, Is.True);
            Assert.That(result.Name, Is.EqualTo("Deleted Category"));
        }

        [Test]
        public async Task GetCategoryForEditByIdAsync_ReturnsNull_WhenCategoryNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            _categoryRepositoryMock
                .Setup(r => r.GetByIdIncludingDeletedAsync(nonExistentId))
                .ReturnsAsync((Category)null);

            // Act
            var result = await _service.GetCategoryForEditByIdAsync(nonExistentId);

            // Assert
            Assert.That(result, Is.Null);
            _categoryRepositoryMock.Verify(r => r.GetByIdIncludingDeletedAsync(nonExistentId), Times.Once);
        }

        [Test]
        public async Task GetCategoryForEditByIdAsync_ReturnsNull_WhenIdIsEmpty()
        {
            // Arrange
            var emptyId = Guid.Empty;
            _categoryRepositoryMock
                .Setup(r => r.GetByIdIncludingDeletedAsync(emptyId))
                .ReturnsAsync((Category)null);

            // Act
            var result = await _service.GetCategoryForEditByIdAsync(emptyId);

            // Assert
            Assert.That(result, Is.Null);
            _categoryRepositoryMock.Verify(r => r.GetByIdIncludingDeletedAsync(emptyId), Times.Once);
        }

        #endregion GetCategoryForEditByIdAsync Tests

        #region EditCategoryAsync Tests

        [Test]
        public async Task EditCategoryAsync_UpdatesExistingCategory()
        {
            // Arrange
            var existingCategory = new Category
            {
                Id = _testCategoryId,
                Name = "Old Name",
                Description = "Old Description",
                IsDeleted = false
            };

            var model = new CategoryViewModelEdit
            {
                Id = _testCategoryId,
                Name = "Updated Name",
                Description = "Updated Description",
                IsDeleted = true
            };

            _categoryRepositoryMock
                .Setup(r => r.GetByIdIncludingDeletedAsync(_testCategoryId))
                .ReturnsAsync(existingCategory);

            _categoryRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Category>()))
                .ReturnsAsync(true);

            // Act
            await _service.EditCategoryAsync(_testCategoryId, model);

            // Assert
            Assert.That(existingCategory.Name, Is.EqualTo("Updated Name"));
            Assert.That(existingCategory.Description, Is.EqualTo("Updated Description"));
            Assert.That(existingCategory.IsDeleted, Is.True);
            _categoryRepositoryMock.Verify(r => r.GetByIdIncludingDeletedAsync(_testCategoryId), Times.Once);
            _categoryRepositoryMock.Verify(r => r.UpdateAsync(existingCategory), Times.Once);
        }

        [Test]
        public async Task EditCategoryAsync_DoesNothing_WhenCategoryNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var model = new CategoryViewModelEdit
            {
                Id = nonExistentId,
                Name = "Updated Name",
                Description = "Updated Description",
                IsDeleted = true
            };

            _categoryRepositoryMock
                .Setup(r => r.GetByIdIncludingDeletedAsync(nonExistentId))
                .ReturnsAsync((Category)null);

            // Act
            await _service.EditCategoryAsync(nonExistentId, model);

            // Assert
            _categoryRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
        }

        [Test]
        public async Task EditCategoryAsync_UpdatesPartialData()
        {
            // Arrange
            var existingCategory = new Category
            {
                Id = _testCategoryId,
                Name = "Original Name",
                Description = "Original Description",
                IsDeleted = false
            };

            var model = new CategoryViewModelEdit
            {
                Id = _testCategoryId,
                Name = "Only Name Updated",
                Description = "Original Description",
                IsDeleted = false
            };

            _categoryRepositoryMock
                .Setup(r => r.GetByIdIncludingDeletedAsync(_testCategoryId))
                .ReturnsAsync(existingCategory);

            _categoryRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Category>()))
                .ReturnsAsync(true);

            // Act
            await _service.EditCategoryAsync(_testCategoryId, model);

            // Assert
            Assert.That(existingCategory.Name, Is.EqualTo("Only Name Updated"));
            Assert.That(existingCategory.Description, Is.EqualTo("Original Description"));
            Assert.That(existingCategory.IsDeleted, Is.False);
        }

        #endregion EditCategoryAsync Tests

        #region ToggleCategoryAsync Tests

        [Test]
        public async Task ToggleCategoryAsync_TogglesCategoryStatus()
        {
            // Arrange
            var category = new Category
            {
                Id = _testCategoryId,
                IsDeleted = false
            };

            _categoryRepositoryMock
                .Setup(r => r.GetByIdIncludingDeletedAsync(_testCategoryId))
                .ReturnsAsync(category);

            _categoryRepositoryMock
                .Setup(r => r.ToggleCategoryStatusAsync(category))
                .Returns(Task.CompletedTask);

            // Act
            await _service.ToggleCategoryAsync(_testCategoryId);

            // Assert
            _categoryRepositoryMock.Verify(r => r.GetByIdIncludingDeletedAsync(_testCategoryId), Times.Once);
            _categoryRepositoryMock.Verify(r => r.ToggleCategoryStatusAsync(category), Times.Once);
        }

        [Test]
        public async Task ToggleCategoryAsync_DoesNothing_WhenCategoryNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            _categoryRepositoryMock
                .Setup(r => r.GetByIdIncludingDeletedAsync(nonExistentId))
                .ReturnsAsync((Category)null);

            // Act
            await _service.ToggleCategoryAsync(nonExistentId);

            // Assert
            _categoryRepositoryMock.Verify(r => r.ToggleCategoryStatusAsync(It.IsAny<Category>()), Times.Never);
        }

        #endregion ToggleCategoryAsync Tests
    }
}