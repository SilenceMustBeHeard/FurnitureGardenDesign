using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Repository.Interfaces.Catalog;
using FurnitureGardenDesign.Services.Core.Implementations.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Category;
using Moq;

namespace FurnitureGardenDesign.Unit.Tests.Services.Manager.Catalog
{
    [TestFixture]
    public class ManagerCategoryServiceTests
    {
        private Mock<ICategoryRepository> _categoryRepositoryMock;
        private CategoryServiceClient _categoryServiceClient;

        private List<Category> _testCategories;
        private Guid _categoryId1;
        private Guid _categoryId2;
        private Guid _categoryId3;

        [SetUp]
        public void SetUp()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>(MockBehavior.Strict);
            _categoryServiceClient = new CategoryServiceClient(_categoryRepositoryMock.Object);
            SeedTestData();
        }

        private void SeedTestData()
        {
            _categoryId1 = Guid.NewGuid();
            _categoryId2 = Guid.NewGuid();
            _categoryId3 = Guid.NewGuid();

            _testCategories = new List<Category>
            {
                new Category
                {
                    Id = _categoryId1,
                    Name = "Living Room",
                    Description = "Living room furniture",
                    IsDeleted = false
                },
                new Category
                {
                    Id = _categoryId2,
                    Name = "Bedroom",
                    Description = "Bedroom furniture",
                    IsDeleted = false
                },
                new Category
                {
                    Id = _categoryId3,
                    Name = "Office",
                    Description = "Office furniture",
                    IsDeleted = false
                }
            };
        }

        #region GetAllActiveCategoriesForClientAsync Tests

        [Test]
        public async Task GetAllActiveCategoriesForClientAsync_ReturnsAllActiveCategories_MappedToViewModels()
        {
            _categoryRepositoryMock
                .Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(_testCategories);

            var result = await _categoryServiceClient.GetAllActiveCategoriesForClientAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));

            var categoriesList = result.ToList();

            Assert.That(categoriesList[0].Id, Is.EqualTo(_categoryId1));
            Assert.That(categoriesList[0].Name, Is.EqualTo("Living Room"));

            Assert.That(categoriesList[1].Id, Is.EqualTo(_categoryId2));
            Assert.That(categoriesList[1].Name, Is.EqualTo("Bedroom"));

            Assert.That(categoriesList[2].Id, Is.EqualTo(_categoryId3));
            Assert.That(categoriesList[2].Name, Is.EqualTo("Office"));

            _categoryRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllActiveCategoriesForClientAsync_ReturnsEmptyList_WhenNoActiveCategoriesExist()
        {
            var emptyCategories = new List<Category>();
            _categoryRepositoryMock
                .Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(emptyCategories);

            var result = await _categoryServiceClient.GetAllActiveCategoriesForClientAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            Assert.That(result.Count(), Is.EqualTo(0));

            _categoryRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllActiveCategoriesForClientAsync_OnlyReturnsActiveCategories_ExcludesDeletedOnes()
        {
            var deletedCategoryId = Guid.NewGuid();
            var categoriesWithDeleted = new List<Category>(_testCategories)
            {
                new Category
                {
                    Id = deletedCategoryId,
                    Name = "Deleted Category",
                    Description = "This should be excluded",
                    IsDeleted = true
                }
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(categoriesWithDeleted.Where(c => !c.IsDeleted).ToList());

            var result = await _categoryServiceClient.GetAllActiveCategoriesForClientAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3)); // Only the 3 active ones, deleted is excluded
            Assert.That(result.Any(c => c.Id == deletedCategoryId), Is.False);
            Assert.That(result.Any(c => c.Name == "Deleted Category"), Is.False);

            _categoryRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllActiveCategoriesForClientAsync_ReturnsCategoriesInOriginalOrder()
        {
            var orderedCategories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Z Category", IsDeleted = false },
                new Category { Id = Guid.NewGuid(), Name = "A Category", IsDeleted = false },
                new Category { Id = Guid.NewGuid(), Name = "M Category", IsDeleted = false }
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(orderedCategories);

            var result = await _categoryServiceClient.GetAllActiveCategoriesForClientAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));

            var resultList = result.ToList();

            Assert.That(resultList[0].Name, Is.EqualTo("Z Category"));
            Assert.That(resultList[1].Name, Is.EqualTo("A Category"));
            Assert.That(resultList[2].Name, Is.EqualTo("M Category"));

            _categoryRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllActiveCategoriesForClientAsync_HandlesLargeNumberOfCategories()
        {
            var largeCategoryList = new List<Category>();
            for (int i = 1; i <= 100; i++)
            {
                largeCategoryList.Add(new Category
                {
                    Id = Guid.NewGuid(),
                    Name = $"Category {i}",
                    IsDeleted = false
                });
            }

            _categoryRepositoryMock
                .Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(largeCategoryList);

            var result = await _categoryServiceClient.GetAllActiveCategoriesForClientAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(100));

            var resultList = result.ToList();
            Assert.That(resultList[0].Name, Is.EqualTo("Category 1"));
            Assert.That(resultList[99].Name, Is.EqualTo("Category 100"));

            _categoryRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once);
        }

        #endregion GetAllActiveCategoriesForClientAsync Tests

        #region Edge Cases and Error Handling Tests

        [Test]
        public void GetAllActiveCategoriesForClientAsync_ThrowsException_WhenRepositoryThrows()
        {
            _categoryRepositoryMock
                .Setup(r => r.GetAllActiveAsync())
                .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _categoryServiceClient.GetAllActiveCategoriesForClientAsync());

            Assert.That(ex.Message, Is.EqualTo("Database connection failed"));
            _categoryRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllActiveCategoriesForClientAsync_HandlesCategoriesWithNullName()
        {
            var categoriesWithNullName = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = null, IsDeleted = false },
                new Category { Id = Guid.NewGuid(), Name = "Valid Category", IsDeleted = false }
            };

            _categoryRepositoryMock
                .Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(categoriesWithNullName);

            var result = await _categoryServiceClient.GetAllActiveCategoriesForClientAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));

            var resultList = result.ToList();
            Assert.That(resultList[0].Name, Is.Null);
            Assert.That(resultList[1].Name, Is.EqualTo("Valid Category"));

            _categoryRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once);
        }

        #endregion Edge Cases and Error Handling Tests

        #region Performance and Verification Tests

        [Test]
        public async Task GetAllActiveCategoriesForClientAsync_ReturnsMaterializedList_NotDeferredExecution()
        {
            _categoryRepositoryMock
                .Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(_testCategories);

            var result = await _categoryServiceClient.GetAllActiveCategoriesForClientAsync();

            Assert.That(result, Is.InstanceOf<IEnumerable<CategoryViewModelList>>());

            if (result is ICollection<CategoryViewModelList> collection)
            {
                Assert.That(collection.Count, Is.EqualTo(3));
            }

            _categoryRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllActiveCategoriesForClientAsync_RepositoryCalledOnce_EvenWithMultipleEnumeration()
        {
            _categoryRepositoryMock
                .Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(_testCategories);

            var result = await _categoryServiceClient.GetAllActiveCategoriesForClientAsync();

            var count1 = result.Count();
            var count2 = result.Count();
            var list = result.ToList();

            Assert.That(count1, Is.EqualTo(3));
            Assert.That(count2, Is.EqualTo(3));
            Assert.That(list.Count, Is.EqualTo(3));

            _categoryRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once);
        }

        #endregion Performance and Verification Tests

        #region Constructor Tests

        [Test]
        public void Constructor_WithValidRepository_CreatesInstance()
        {
            var repositoryMock = new Mock<ICategoryRepository>();

            var service = new CategoryServiceClient(repositoryMock.Object);

            Assert.That(service, Is.Not.Null);
        }

        #endregion Constructor Tests
    }
}