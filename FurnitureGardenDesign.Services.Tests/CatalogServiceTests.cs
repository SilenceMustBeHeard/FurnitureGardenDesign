

using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Implementations;
using MockQueryable;
using Moq;
using NUnit.Framework;

namespace FurnitureGardenDesign.Services.Tests
{
    [TestFixture]
    public class CatalogServiceTests
    {
        private Mock<ICatalogRepository> _catalogRepoMock;
        private Mock<IFavoriteRepository> _favoriteRepoMock;
        private Mock<IReviewRepository> _reviewRepoMock;
        private CatalogService _catalogService;

        private CatalogDesign _testDesign1;
        private CatalogDesign _testDesign2;
        private CatalogDesign _testDesign3;
        private CatalogDesign _testDesign4;
        private CatalogDesign _inactiveDesign;
        private Category _testCategory;
        private Guid _designId1;
        private Guid _designId2;
        private Guid _designId3;
        private Guid _designId4;
        private Guid _inactiveDesignId;
        private string _testUserId;

        [SetUp]
        public void SetUp()
        {
            _catalogRepoMock = new Mock<ICatalogRepository>(MockBehavior.Strict);
            _favoriteRepoMock = new Mock<IFavoriteRepository>(MockBehavior.Strict);
            _reviewRepoMock = new Mock<IReviewRepository>(MockBehavior.Strict);
            _catalogService = new CatalogService(
                _catalogRepoMock.Object,
                _favoriteRepoMock.Object,
                _reviewRepoMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _testUserId = "test-user-123";
            _designId1 = Guid.NewGuid();
            _designId2 = Guid.NewGuid();
            _designId3 = Guid.NewGuid();
            _designId4 = Guid.NewGuid();
            _inactiveDesignId = Guid.NewGuid();

            _testCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Living Room",
                Description = "Living Room Furniture"
            };

            var reviews1 = new List<Review>
            {
                new Review { Id = Guid.NewGuid(), Rating = 5, Comment = "Great!", UserId = "user1" },
                new Review { Id = Guid.NewGuid(), Rating = 4, Comment = "Good", UserId = "user2" }
            };

            var reviews2 = new List<Review>
            {
                new Review { Id = Guid.NewGuid(), Rating = 3, Comment = "Okay", UserId = "user3" }
            };

            var favorites1 = new List<Favorite>
            {
                new Favorite { Id = Guid.NewGuid(), UserId = _testUserId, CatalogDesignId = _designId1, IsDeleted = false }
            };

            _testDesign1 = new CatalogDesign
            {
                Id = _designId1,
                Title = "Modern Sofa",
                Description = "A comfortable modern sofa",
                Image2DUrl = "sofa.jpg",
                Model3DUrl = "sofa.glb",
                Materials = "Leather, Wood",
                Price = 499.99m,
                IsActive = true,
                Model3DStatus = Model3DStatus.Ready,
                Category = _testCategory,
                CategoryId = _testCategory.Id,
                Reviews = reviews1,
                Favorites = favorites1,
                CreatedOn = DateTime.UtcNow.AddDays(-5)
            };

            _testDesign2 = new CatalogDesign
            {
                Id = _designId2,
                Title = "Wooden Table",
                Description = "Solid oak dining table",
                Image2DUrl = "table.jpg",
                Model3DUrl = null,
                Materials = "Oak Wood",
                Price = 299.99m,
                IsActive = true,
                Model3DStatus = Model3DStatus.None,
                Category = _testCategory,
                CategoryId = _testCategory.Id,
                Reviews = reviews2,
                Favorites = new List<Favorite>(),
                CreatedOn = DateTime.UtcNow.AddDays(-3)
            };

            _testDesign3 = new CatalogDesign
            {
                Id = _designId3,
                Title = "Bookshelf",
                Description = "Modern bookshelf",
                Image2DUrl = "bookshelf.jpg",
                Model3DUrl = "bookshelf.glb",
                Materials = "Metal, Glass",
                Price = 199.99m,
                IsActive = true,
                Model3DStatus = Model3DStatus.Ready,
                Category = _testCategory,
                CategoryId = _testCategory.Id,
                Reviews = new List<Review>(),
                Favorites = new List<Favorite>(),
                CreatedOn = DateTime.UtcNow.AddDays(-1)
            };
            _testDesign4 = new CatalogDesign
            {
                Id = _designId4,
                Title = "Bed",
                Description = "Modern bed",
                Image2DUrl = "bed.jpg",
                Model3DUrl = "bed.glb",
                Materials = "Metal, Glass",
                Price = 199.99m,
                IsActive = true,
                Model3DStatus = Model3DStatus.Ready,
                Category = _testCategory,
                CategoryId = _testCategory.Id,
                Reviews = new List<Review>(),
                Favorites = new List<Favorite>(),
                CreatedOn = DateTime.UtcNow.AddDays(-1)
            };  

            _inactiveDesign = new CatalogDesign
            {
                Id = _inactiveDesignId,
                Title = "Old Chair",
                Description = "Old design",
                Image2DUrl = "chair.jpg",
                Price = 99.99m,
                IsActive = false,
                Category = _testCategory,
                CategoryId = _testCategory.Id,
                Reviews = new List<Review>(),
                Favorites = new List<Favorite>()
            };
        }

        #region GetAllActiveAsync Tests

        [Test]
        public async Task GetAllActiveAsync_ReturnsOnlyActiveDesignsWithRelatedData()
        {
         
            var allDesigns = new List<CatalogDesign> { _testDesign1, _testDesign2, _testDesign3, _inactiveDesign };
            var mockQueryable = allDesigns.BuildMock();
            _catalogRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

         
            var result = await _catalogService.GetAllActiveAsync();

          
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.IsFalse(result.Any(d => d.Id == _inactiveDesignId));

            var design1 = result.First(d => d.Id == _designId1);
            Assert.AreEqual(_testDesign1.Title, design1.Title);
            Assert.IsNotNull(design1.Category);
            Assert.AreEqual("Living Room", design1.Category.Name);
            Assert.AreEqual(2, design1.Reviews.Count);
            Assert.AreEqual(1, design1.Favorites.Count);

            _catalogRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        #endregion

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_ReturnsDesign_WhenIdExistsAndIsActive()
        {
          
            var allDesigns = new List<CatalogDesign> { _testDesign1, _testDesign2 };
            var mockQueryable = allDesigns.BuildMock();
            _catalogRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            
            var result = await _catalogService.GetByIdAsync(_designId1);

           
            Assert.IsNotNull(result);
            Assert.AreEqual(_designId1, result.Id);
            Assert.AreEqual("Modern Sofa", result.Title);
            Assert.IsNotNull(result.Category);
            Assert.AreEqual(2, result.Reviews.Count);
            Assert.AreEqual(1, result.Favorites.Count);
            _catalogRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
        }









        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenIdDoesNotExist()
        {
           
            var allDesigns = new List<CatalogDesign> { _testDesign1, _testDesign2 };
            var mockQueryable = allDesigns.BuildMock();
            _catalogRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);
            var nonExistentId = Guid.NewGuid();

            var result = await _catalogService.GetByIdAsync(nonExistentId);

         
            Assert.IsNull(result);
            _catalogRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenDesignIsInactive()
        {
            
            var allDesigns = new List<CatalogDesign> { _testDesign1, _inactiveDesign };
            var mockQueryable = allDesigns.BuildMock();
            _catalogRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

     
            var result = await _catalogService.GetByIdAsync(_inactiveDesignId);


            Assert.IsNull(result);
            _catalogRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
        }










        [Test]
        public async Task AddToFavoritesAsync_AddsFavorite_WhenNotAlreadyFavorited()
        {
         
            var favorites = new List<Favorite>();
            var mockQueryable = favorites.BuildMock();
            _favoriteRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);
            _favoriteRepoMock.Setup(r => r.AddAsync(It.IsAny<Favorite>()))
                .Returns(Task.CompletedTask)
                .Callback<Favorite>(f => favorites.Add(f));
            _favoriteRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _catalogService.AddToFavoritesAsync(_testUserId, _designId1);

           
            _favoriteRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
            _favoriteRepoMock.Verify(r => r.AddAsync(It.Is<Favorite>(f =>
                f.UserId == _testUserId &&
                f.CatalogDesignId == _designId1)), Times.Once);
            _favoriteRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }







        [Test]
        public async Task AddToFavoritesAsync_DoesNotAddFavorite_WhenAlreadyExists()
        {
           
            var existingFavorite = new Favorite
            {
                UserId = _testUserId,
                CatalogDesignId = _designId1
            };
            var favorites = new List<Favorite> { existingFavorite };
            var mockQueryable = favorites.BuildMock();
            _favoriteRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

          
            await _catalogService.AddToFavoritesAsync(_testUserId, _designId1);

         
            _favoriteRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
            _favoriteRepoMock.Verify(r => r.AddAsync(It.IsAny<Favorite>()), Times.Never);
            _favoriteRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

      
        [Test]
        public async Task RemoveFromFavoritesAsync_RemovesFavorite_WhenExists()
        {
          
            var favoriteToRemove = new Favorite
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                CatalogDesignId = _designId1
            };
            var favorites = new List<Favorite> { favoriteToRemove };
            var mockQueryable = favorites.BuildMock();
            _favoriteRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);
            _favoriteRepoMock.Setup(r => r.DeleteAsync(It.IsAny<Favorite>()))
                .ReturnsAsync(true);
            _favoriteRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

          
            await _catalogService.RemoveFromFavoritesAsync(_testUserId, _designId1);

        
            _favoriteRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
            _favoriteRepoMock.Verify(r => r.DeleteAsync(It.Is<Favorite>(f =>
                f.UserId == _testUserId &&
                f.CatalogDesignId == _designId1)), Times.Once);
            _favoriteRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task RemoveFromFavoritesAsync_DoesNothing_WhenFavoriteDoesNotExist()
        {
           
            var favorites = new List<Favorite>();
            var mockQueryable = favorites.BuildMock();
            _favoriteRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

          
            await _catalogService.RemoveFromFavoritesAsync(_testUserId, _designId1);

         
            _favoriteRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
            _favoriteRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Favorite>()), Times.Never);
            _favoriteRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

       

        [Test]
        public async Task AddReviewAsync_CreatesAndSavesReview()
        {
          
            int rating = 4;
            string comment = "Great design!";

            _reviewRepoMock.Setup(r => r.AddAsync(It.IsAny<Review>()))
                .Returns(Task.CompletedTask);
            _reviewRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

          
            await _catalogService.AddReviewAsync(_testUserId, _designId1, rating, comment);

           
            _reviewRepoMock.Verify(r => r.AddAsync(It.Is<Review>(rev =>
                rev.UserId == _testUserId &&
                rev.CatalogDesignId == _designId1 &&
                rev.Rating == rating &&
                rev.Comment == comment)), Times.Once);
            _reviewRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }








        [Test]
        public async Task AddReviewAsync_CreatesReview_WithNullComment()
        {
       
            int rating = 5;

            _reviewRepoMock.Setup(r => r.AddAsync(It.IsAny<Review>()))
                .Returns(Task.CompletedTask);
            _reviewRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);


            await _catalogService.AddReviewAsync(_testUserId, _designId1, rating, null);

   
            _reviewRepoMock.Verify(r => r.AddAsync(It.Is<Review>(rev =>
                rev.UserId == _testUserId &&
                rev.CatalogDesignId == _designId1 &&
                rev.Rating == rating &&
                rev.Comment == null)), Times.Once);
            _reviewRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }





      

        [Test]
        public async Task GetPublicCatalogAsync_ReturnsCorrectPagination_ForPage2()
        {
   
            var allDesigns = new List<CatalogDesign> { _testDesign1, _testDesign2, _testDesign3 };
            var mockQueryable = allDesigns.BuildMock();
            _catalogRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

      
            var result = await _catalogService.GetPublicCatalogAsync(_testUserId, 2, 2, false);

         
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count()); // 3 total, page 2 should have 1 item
            Assert.AreEqual(_designId1, result.First().Id); // Oldest design
            _catalogRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
        }






        [Test]
        public async Task GetPublicCatalogAsync_ReturnsEmptyList_WhenNoActiveDesigns()
        {
           
            var allDesigns = new List<CatalogDesign>();
            var mockQueryable = allDesigns.BuildMock();
            _catalogRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

          
            var result = await _catalogService.GetPublicCatalogAsync(_testUserId, 1, 10, false);

          
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
            _catalogRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

     

        [Test]
        public async Task GetTotalActiveDesignsAsync_ReturnsCorrectCount()
        {
            
            var allDesigns = new List<CatalogDesign> { _testDesign1, _testDesign2, _testDesign3, _inactiveDesign };
            var mockQueryable = allDesigns.BuildMock();
            _catalogRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

           
            var result = await _catalogService.GetTotalActiveDesignsAsync();

            
            Assert.AreEqual(3, result);
            _catalogRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetTotalActiveDesignsAsync_ReturnsZero_WhenNoActiveDesigns()
        {
          
            var allDesigns = new List<CatalogDesign> { _inactiveDesign };
            var mockQueryable = allDesigns.BuildMock();
            _catalogRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

           
            var result = await _catalogService.GetTotalActiveDesignsAsync();

            
            Assert.AreEqual(0, result);
            _catalogRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

     

        [Test]
        public async Task GetDetailsAsync_ReturnsViewModel_WhenDesignExistsAndIsActive()
        {
          
            var allDesigns = new List<CatalogDesign> { _testDesign1, _testDesign2 };
            var mockQueryable = allDesigns.BuildMock();
            _catalogRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

           
            var result = await _catalogService.GetDetailsAsync(_designId1, _testUserId);

          
            Assert.IsNotNull(result);
            Assert.AreEqual(_designId1, result.Id);
            Assert.AreEqual("Modern Sofa", result.Title);
            Assert.AreEqual("A comfortable modern sofa", result.Description);
            Assert.AreEqual("sofa.jpg", result.Image2DUrl);
            Assert.AreEqual("sofa.glb", result.Model3DUrl);
            Assert.AreEqual("Leather, Wood", result.Materials);
            Assert.AreEqual(Model3DStatus.Ready, result.Model3DStatus);
            Assert.AreEqual(499.99m, result.Price);
            Assert.AreEqual("Living Room", result.CategoryName);
            Assert.IsTrue(result.IsFavorited); 
            Assert.AreEqual(4.5, result.AverageRating); 
            Assert.AreEqual(2, result.ReviewCount);
            _catalogRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetDetailsAsync_ReturnsNull_WhenDesignDoesNotExist()
        {
            var allDesigns = new List<CatalogDesign> { _testDesign1, _testDesign2 };
            var mockQueryable = allDesigns.BuildMock();
            _catalogRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);
            var nonExistentId = Guid.NewGuid();

         
            var result = await _catalogService.GetDetailsAsync(nonExistentId, _testUserId);

           
            Assert.IsNull(result);
            _catalogRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetDetailsAsync_ReturnsNull_WhenDesignIsInactive()
        {
        
            var allDesigns = new List<CatalogDesign> { _testDesign1, _inactiveDesign };
            var mockQueryable = allDesigns.BuildMock();
            _catalogRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

         
            var result = await _catalogService.GetDetailsAsync(_inactiveDesignId, _testUserId);

          
            Assert.IsNull(result);
            _catalogRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetDetailsAsync_SetsModel3DStatusToNone_WhenModel3DUrlIsEmpty()
        {
          
            var allDesigns = new List<CatalogDesign> { _testDesign2 };
            var mockQueryable = allDesigns.BuildMock();
            _catalogRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            
            var result = await _catalogService.GetDetailsAsync(_designId2, null);

           
            Assert.IsNotNull(result);
            Assert.IsNull(result.Model3DUrl);
            Assert.AreEqual(Model3DStatus.None, result.Model3DStatus);
            _catalogRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetDetailsAsync_SetsIsFavoritedToFalse_WhenUserIdIsNull()
        {
         
            var allDesigns = new List<CatalogDesign> { _testDesign1 };
            var mockQueryable = allDesigns.BuildMock();
            _catalogRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

         
            var result = await _catalogService.GetDetailsAsync(_designId1, null);

           
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsFavorited);
            _catalogRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetDetailsAsync_ReturnsZeroAverageRating_WhenNoReviews()
        {
          
            var allDesigns = new List<CatalogDesign> { _testDesign3 };
            var mockQueryable = allDesigns.BuildMock();
            _catalogRepoMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

          
            var result = await _catalogService.GetDetailsAsync(_designId3, null);

          
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.AverageRating);
            Assert.AreEqual(0, result.ReviewCount);
            _catalogRepoMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        #endregion
    }
} 