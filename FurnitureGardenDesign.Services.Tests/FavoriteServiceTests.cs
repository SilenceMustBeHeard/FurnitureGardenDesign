using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Implementations;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Services.Tests
{
    [TestFixture]
    public class FavoriteServiceTests
    {
        private Mock<IFavoriteRepository> _favoriteRepositoryMock;
        private FavoriteService _favoriteService;

        private string _testUserId;
        private Guid _testDesignId;
        private Guid _testFavoriteId;
        private Favorite _existingFavorite;

        [SetUp]
        public void SetUp()
        {
            _favoriteRepositoryMock = new Mock<IFavoriteRepository>(MockBehavior.Strict);
            _favoriteService = new FavoriteService(_favoriteRepositoryMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _testUserId = "test-user-123";
            _testDesignId = Guid.NewGuid();
            _testFavoriteId = Guid.NewGuid();

            _existingFavorite = new Favorite
            {
                Id = _testFavoriteId,
                UserId = _testUserId,
                CatalogDesignId = _testDesignId,
                IsDeleted = false
            };
        }

        #region ToggleFavoriteAsync Tests

        [Test]
        public async Task ToggleFavoriteAsync_WhenFavoriteDoesNotExist_AddsNewFavoriteAndReturnsTrue()
        {
          
            _favoriteRepositoryMock
                .Setup(r => r.GetByCompositeKeyAsync(_testUserId, _testDesignId))
                .ReturnsAsync((Favorite)null);

            _favoriteRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Favorite>()))
                .Returns(Task.CompletedTask)
                .Callback<Favorite>(f =>
                {
                    Assert.That(f.UserId, Is.EqualTo(_testUserId));
                    Assert.That(f.CatalogDesignId, Is.EqualTo(_testDesignId));
                    Assert.That(f.IsDeleted, Is.False);
                });

 
            var result = await _favoriteService.ToggleFavoriteAsync(_testUserId, _testDesignId);

  
            Assert.That(result, Is.True);

            _favoriteRepositoryMock.Verify(r => r.GetByCompositeKeyAsync(_testUserId, _testDesignId), Times.Once);
            _favoriteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Favorite>()), Times.Once);
            _favoriteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task ToggleFavoriteAsync_WhenFavoriteExistsAndIsNotDeleted_TogglesToDeletedAndReturnsFalse()
        {
         
            _existingFavorite.IsDeleted = false;

            _favoriteRepositoryMock
                .Setup(r => r.GetByCompositeKeyAsync(_testUserId, _testDesignId))
                .ReturnsAsync(_existingFavorite);

            _favoriteRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _favoriteService.ToggleFavoriteAsync(_testUserId, _testDesignId);

            Assert.That(result, Is.False);
            Assert.That(_existingFavorite.IsDeleted, Is.True);

            _favoriteRepositoryMock.Verify(r => r.GetByCompositeKeyAsync(_testUserId, _testDesignId), Times.Once);
            _favoriteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Favorite>()), Times.Never);
            _favoriteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ToggleFavoriteAsync_WhenFavoriteExistsAndIsDeleted_TogglesToNotDeletedAndReturnsTrue()
        {
         
            _existingFavorite.IsDeleted = true;

            _favoriteRepositoryMock
                .Setup(r => r.GetByCompositeKeyAsync(_testUserId, _testDesignId))
                .ReturnsAsync(_existingFavorite);

            _favoriteRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            
            var result = await _favoriteService.ToggleFavoriteAsync(_testUserId, _testDesignId);

         
            Assert.That(result, Is.True);
            Assert.That(_existingFavorite.IsDeleted, Is.False);

            _favoriteRepositoryMock.Verify(r => r.GetByCompositeKeyAsync(_testUserId, _testDesignId), Times.Once);
            _favoriteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Favorite>()), Times.Never);
            _favoriteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ToggleFavoriteAsync_WithDifferentUserAndDesign_CreatesNewFavorite()
        {
      
            var differentUserId = "different-user-456";
            var differentDesignId = Guid.NewGuid();

            _favoriteRepositoryMock
                .Setup(r => r.GetByCompositeKeyAsync(differentUserId, differentDesignId))
                .ReturnsAsync((Favorite)null);

            _favoriteRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Favorite>()))
                .Returns(Task.CompletedTask);

          
            var result = await _favoriteService.ToggleFavoriteAsync(differentUserId, differentDesignId);

         
            Assert.That(result, Is.True);

            _favoriteRepositoryMock.Verify(r => r.GetByCompositeKeyAsync(differentUserId, differentDesignId), Times.Once);
            _favoriteRepositoryMock.Verify(r => r.AddAsync(It.Is<Favorite>(f =>
                f.UserId == differentUserId &&
                f.CatalogDesignId == differentDesignId)), Times.Once);
        }

        [Test]
        public void ToggleFavoriteAsync_WhenRepositoryThrowsOnGet_PropagatesException()
        {
           
            _favoriteRepositoryMock
                .Setup(r => r.GetByCompositeKeyAsync(_testUserId, _testDesignId))
                .ThrowsAsync(new InvalidOperationException("Database error"));

           
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _favoriteService.ToggleFavoriteAsync(_testUserId, _testDesignId));

            Assert.That(ex.Message, Is.EqualTo("Database error"));

            _favoriteRepositoryMock.Verify(r => r.GetByCompositeKeyAsync(_testUserId, _testDesignId), Times.Once);
            _favoriteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Favorite>()), Times.Never);
            _favoriteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public void ToggleFavoriteAsync_WhenAddAsyncThrows_PropagatesException()
        {
         
            _favoriteRepositoryMock
                .Setup(r => r.GetByCompositeKeyAsync(_testUserId, _testDesignId))
                .ReturnsAsync((Favorite)null);

            _favoriteRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Favorite>()))
                .ThrowsAsync(new InvalidOperationException("Failed to add favorite"));

          
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _favoriteService.ToggleFavoriteAsync(_testUserId, _testDesignId));

            Assert.That(ex.Message, Is.EqualTo("Failed to add favorite"));

            _favoriteRepositoryMock.Verify(r => r.GetByCompositeKeyAsync(_testUserId, _testDesignId), Times.Once);
            _favoriteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Favorite>()), Times.Once);
            _favoriteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public void ToggleFavoriteAsync_WhenSaveChangesAsyncThrows_PropagatesException()
        {
            
            _favoriteRepositoryMock
                .Setup(r => r.GetByCompositeKeyAsync(_testUserId, _testDesignId))
                .ReturnsAsync(_existingFavorite);

            _favoriteRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ThrowsAsync(new InvalidOperationException("Failed to save changes"));

            
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _favoriteService.ToggleFavoriteAsync(_testUserId, _testDesignId));

            Assert.That(ex.Message, Is.EqualTo("Failed to save changes"));

            _favoriteRepositoryMock.Verify(r => r.GetByCompositeKeyAsync(_testUserId, _testDesignId), Times.Once);
            _favoriteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Favorite>()), Times.Never);
            _favoriteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region IsFavoriteAsync Tests

        [Test]
        public async Task IsFavoriteAsync_WhenFavoriteExistsAndNotDeleted_ReturnsTrue()
        {
            
            _favoriteRepositoryMock
                .Setup(r => r.ExistsAsync(_testUserId, _testDesignId))
                .ReturnsAsync(true);

           
            var result = await _favoriteService.IsFavoriteAsync(_testUserId, _testDesignId);

            
            Assert.That(result, Is.True);

            _favoriteRepositoryMock.Verify(r => r.ExistsAsync(_testUserId, _testDesignId), Times.Once);
        }

        [Test]
        public async Task IsFavoriteAsync_WhenFavoriteExistsButIsDeleted_ReturnsFalse()
        {
            
            _favoriteRepositoryMock
                .Setup(r => r.ExistsAsync(_testUserId, _testDesignId))
                .ReturnsAsync(false); 

           
            var result = await _favoriteService.IsFavoriteAsync(_testUserId, _testDesignId);

           
            Assert.That(result, Is.False);

            _favoriteRepositoryMock.Verify(r => r.ExistsAsync(_testUserId, _testDesignId), Times.Once);
        }

        [Test]
        public async Task IsFavoriteAsync_WhenFavoriteDoesNotExist_ReturnsFalse()
        {
            
            _favoriteRepositoryMock
                .Setup(r => r.ExistsAsync(_testUserId, _testDesignId))
                .ReturnsAsync(false);

          
            var result = await _favoriteService.IsFavoriteAsync(_testUserId, _testDesignId);

          
            Assert.That(result, Is.False);

            _favoriteRepositoryMock.Verify(r => r.ExistsAsync(_testUserId, _testDesignId), Times.Once);
        }

        [Test]
        public async Task IsFavoriteAsync_WithDifferentUser_ReturnsFalse()
        {
           
            var differentUserId = "different-user-456";

            _favoriteRepositoryMock
                .Setup(r => r.ExistsAsync(differentUserId, _testDesignId))
                .ReturnsAsync(false);

          
            var result = await _favoriteService.IsFavoriteAsync(differentUserId, _testDesignId);

           
            Assert.That(result, Is.False);

            _favoriteRepositoryMock.Verify(r => r.ExistsAsync(differentUserId, _testDesignId), Times.Once);
        }

        [Test]
        public async Task IsFavoriteAsync_WithDifferentDesign_ReturnsFalse()
        {
          
            var differentDesignId = Guid.NewGuid();

            _favoriteRepositoryMock
                .Setup(r => r.ExistsAsync(_testUserId, differentDesignId))
                .ReturnsAsync(false);

          
            var result = await _favoriteService.IsFavoriteAsync(_testUserId, differentDesignId);

         
            Assert.That(result, Is.False);

            _favoriteRepositoryMock.Verify(r => r.ExistsAsync(_testUserId, differentDesignId), Times.Once);
        }

        [Test]
        public void IsFavoriteAsync_WhenRepositoryThrows_PropagatesException()
        {
            
            _favoriteRepositoryMock
                .Setup(r => r.ExistsAsync(_testUserId, _testDesignId))
                .ThrowsAsync(new InvalidOperationException("Database error"));

           
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _favoriteService.IsFavoriteAsync(_testUserId, _testDesignId));

            Assert.That(ex.Message, Is.EqualTo("Database error"));

            _favoriteRepositoryMock.Verify(r => r.ExistsAsync(_testUserId, _testDesignId), Times.Once);
        }

        #endregion

        #region Edge Cases and Validation Tests

        [Test]
        public async Task ToggleFavoriteAsync_WithEmptyUserId_AttemptsToUseEmptyString()
        {
          
            var emptyUserId = string.Empty;

            _favoriteRepositoryMock
                .Setup(r => r.GetByCompositeKeyAsync(emptyUserId, _testDesignId))
                .ReturnsAsync((Favorite)null);

            _favoriteRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Favorite>()))
                .Returns(Task.CompletedTask);

            
            var result = await _favoriteService.ToggleFavoriteAsync(emptyUserId, _testDesignId);

            Assert.That(result, Is.True);

            _favoriteRepositoryMock.Verify(r => r.GetByCompositeKeyAsync(emptyUserId, _testDesignId), Times.Once);
            _favoriteRepositoryMock.Verify(r => r.AddAsync(It.Is<Favorite>(f =>
                f.UserId == emptyUserId)), Times.Once);
        }

        [Test]
        public async Task ToggleFavoriteAsync_WithNullUserId_AttemptsToUseNull()
        {
           
            _favoriteRepositoryMock
                .Setup(r => r.GetByCompositeKeyAsync(null, _testDesignId))
                .ReturnsAsync((Favorite)null);

            _favoriteRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Favorite>()))
                .Returns(Task.CompletedTask);

         
            var result = await _favoriteService.ToggleFavoriteAsync(null, _testDesignId);

            
            Assert.That(result, Is.True);

            _favoriteRepositoryMock.Verify(r => r.GetByCompositeKeyAsync(null, _testDesignId), Times.Once);
            _favoriteRepositoryMock.Verify(r => r.AddAsync(It.Is<Favorite>(f =>
                f.UserId == null)), Times.Once);
        }

        [Test]
        public async Task IsFavoriteAsync_WithEmptyUserId_ReturnsFalse()
        {
            
            var emptyUserId = string.Empty;

            _favoriteRepositoryMock
                .Setup(r => r.ExistsAsync(emptyUserId, _testDesignId))
                .ReturnsAsync(false);

          
            var result = await _favoriteService.IsFavoriteAsync(emptyUserId, _testDesignId);

      
            Assert.That(result, Is.False);

            _favoriteRepositoryMock.Verify(r => r.ExistsAsync(emptyUserId, _testDesignId), Times.Once);
        }

        [Test]
        public async Task IsFavoriteAsync_WithNullUserId_ReturnsFalse()
        {
            
            _favoriteRepositoryMock
                .Setup(r => r.ExistsAsync(null, _testDesignId))
                .ReturnsAsync(false);

           
            var result = await _favoriteService.IsFavoriteAsync(null, _testDesignId);

         
            Assert.That(result, Is.False);

            _favoriteRepositoryMock.Verify(r => r.ExistsAsync(null, _testDesignId), Times.Once);
        }

        [Test]
        public async Task ToggleFavoriteAsync_WithEmptyGuid_AttemptsToUseEmptyGuid()
        {
         
            var emptyGuid = Guid.Empty;

            _favoriteRepositoryMock
                .Setup(r => r.GetByCompositeKeyAsync(_testUserId, emptyGuid))
                .ReturnsAsync((Favorite)null);

            _favoriteRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Favorite>()))
                .Returns(Task.CompletedTask);

          
            var result = await _favoriteService.ToggleFavoriteAsync(_testUserId, emptyGuid);

           
            Assert.That(result, Is.True);

            _favoriteRepositoryMock.Verify(r => r.GetByCompositeKeyAsync(_testUserId, emptyGuid), Times.Once);
            _favoriteRepositoryMock.Verify(r => r.AddAsync(It.Is<Favorite>(f =>
                f.CatalogDesignId == emptyGuid)), Times.Once);
        }

        #endregion

        #region Constructor Tests

      
        [Test]
        public void Constructor_WithValidRepository_CreatesInstance()
        {
           
            var repositoryMock = new Mock<IFavoriteRepository>();

         
            var service = new FavoriteService(repositoryMock.Object);

          
            Assert.That(service, Is.Not.Null);
        }

        #endregion

    


      
       
    }
}