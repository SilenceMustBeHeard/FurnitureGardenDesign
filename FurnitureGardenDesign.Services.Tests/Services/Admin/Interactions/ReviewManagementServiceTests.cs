using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Interfaces.Catalog;
using FurnitureGardenDesign.Data.Repository.Interfaces.Interactions;
using FurnitureGardenDesign.Services.Core.Admin.Implementations.Interactions;
using FurnitureGardenDesign.Web.ViewModels.Review;
using MockQueryable.Moq;
using Moq;

namespace FurnitureGardenDesign.Unit.Tests.Services.Admin.Interactions
{
    [TestFixture]
    public class ReviewManagementServiceTests
    {
        private Mock<IReviewManagementRepository> _reviewRepoMock;
        private Mock<ICatalogRepository> _catalogRepoMock;
        private ReviewManagementService _service;

        private string _testUserId;
        private Guid _testCatalogDesignId;
        private Guid _testReviewId;
        private Review _testReview;
        private CatalogDesign _testCatalogDesign;
        private List<Review> _testReviews;
        private List<CatalogDesign> _testCatalogDesigns;

        [SetUp]
        public void SetUp()
        {
            _reviewRepoMock = new Mock<IReviewManagementRepository>(MockBehavior.Strict);
            _catalogRepoMock = new Mock<ICatalogRepository>(MockBehavior.Strict);
            _service = new ReviewManagementService(_reviewRepoMock.Object, _catalogRepoMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _testUserId = "test-user-123";
            _testCatalogDesignId = Guid.NewGuid();
            _testReviewId = Guid.NewGuid();

            _testCatalogDesign = new CatalogDesign
            {
                Id = _testCatalogDesignId,
                Title = "Test Design",
                Description = "Test Description",
                Image2DUrl = "/images/test.jpg",
                Model3DUrl = "/models/test.glb",
                Price = 99.99m,
                IsDeleted = false
            };

            _testReview = new Review
            {
                Id = _testReviewId,
                CatalogDesignId = _testCatalogDesignId,
                UserId = _testUserId,
                Rating = 5,
                Comment = "Excellent design!",
                CreatedOn = DateTime.UtcNow.AddDays(-1),
                IsDeleted = false,
                User = new AppUser
                {
                    Id = _testUserId,
                    UserName = "testuser",
                    Email = "test@example.com"
                },
                CatalogDesign = _testCatalogDesign
            };

            _testReviews = new List<Review>
            {
                _testReview,
                new Review
                {
                    Id = Guid.NewGuid(),
                    CatalogDesignId = _testCatalogDesignId,
                    UserId = "other-user",
                    Rating = 4,
                    Comment = "Good design",
                    CreatedOn = DateTime.UtcNow.AddDays(-2),
                    IsDeleted = false,
                    User = new AppUser
                    {
                        Id = "other-user",
                        UserName = "otheruser",
                        Email = "other@example.com"
                    }
                },

                new Review
                {
                    Id = Guid.NewGuid(),
                    CatalogDesignId = _testCatalogDesignId,
                    UserId = "deleted-user",
                    Rating = 3,
                    Comment = "Deleted review",
                    CreatedOn = DateTime.UtcNow.AddDays(-3),
                    IsDeleted = true,
                    User = new AppUser
                    {
                        Id = "deleted-user",
                        UserName = "deleteduser",
                        Email = "deleted@example.com"
                    }
                }
            };

            _testCatalogDesigns = new List<CatalogDesign> { _testCatalogDesign };
        }

        #region AddReviewAsync Tests

        [Test]
        public async Task AddReviewAsync_AddsReviewSuccessfully()
        {
            var model = new AddReviewViewModel
            {
                CatalogDesignId = _testCatalogDesignId,
                Rating = 4,
                Comment = "Great product!"
            };

            _reviewRepoMock.Setup(r => r.AddAsync(It.IsAny<Review>()))
                .Returns(Task.CompletedTask);

            _reviewRepoMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _service.AddReviewAsync(_testUserId, model);

            _reviewRepoMock.Verify(r => r.AddAsync(It.Is<Review>(review =>
                review.CatalogDesignId == _testCatalogDesignId
                && review.UserId == _testUserId
                && review.Rating == 4
                && review.Comment == "Great product!"
                && !review.IsDeleted)), Times.Once);
            _reviewRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        #endregion AddReviewAsync Tests

        #region HasUserReviewedAsync Tests

        [Test]
        public async Task HasUserReviewedAsync_ReturnsTrue_WhenUserHasReviewed()
        {
            _reviewRepoMock.Setup(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId))
                .ReturnsAsync(true);

            var result = await _service.HasUserReviewedAsync(_testUserId, _testCatalogDesignId);

            Assert.That(result, Is.True);
            _reviewRepoMock.Verify(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId), Times.Once);
        }

        [Test]
        public async Task HasUserReviewedAsync_ReturnsFalse_WhenUserHasNotReviewed()
        {
            _reviewRepoMock.Setup(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId))
                .ReturnsAsync(false);

            var result = await _service.HasUserReviewedAsync(_testUserId, _testCatalogDesignId);

            Assert.That(result, Is.False);
        }

        #endregion HasUserReviewedAsync Tests

        #region GetReviewsByDesignIdAsync Tests

        [Test]
        public async Task GetReviewsByDesignIdAsync_ReturnsReviewsForDesign()
        {
            var reviews = new List<Review> { _testReview };

            _reviewRepoMock.Setup(r => r.GetReviewsByDesignIdAsync(_testCatalogDesignId))
                .ReturnsAsync(reviews);

            var result = await _service.GetReviewsByDesignIdAsync(_testCatalogDesignId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Rating, Is.EqualTo(5));
            Assert.That(result.First().Comment, Is.EqualTo("Excellent design!"));
        }

        [Test]
        public async Task GetReviewsByDesignIdAsync_ReturnsEmptyList_WhenNoReviews()
        {
            _reviewRepoMock.Setup(r => r.GetReviewsByDesignIdAsync(_testCatalogDesignId))
                .ReturnsAsync(new List<Review>());

            var result = await _service.GetReviewsByDesignIdAsync(_testCatalogDesignId);

            Assert.That(result, Is.Empty);
        }

        #endregion GetReviewsByDesignIdAsync Tests

        #region GetWriteReviewModelAsync Tests

        [Test]
        public async Task GetWriteReviewModelAsync_ReturnsModel_WhenUserCanReview()
        {
            _reviewRepoMock.Setup(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId))
                .ReturnsAsync(false);

            _catalogRepoMock.Setup(r => r.GetByIdWithReviewsAsync(_testCatalogDesignId))
                .ReturnsAsync(_testCatalogDesign);

            var result = await _service.GetWriteReviewModelAsync(_testUserId, _testCatalogDesignId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testCatalogDesignId));
            Assert.That(result.Title, Is.EqualTo("Test Design"));
            Assert.That(result.Price, Is.EqualTo(99.99m));
        }

        #endregion GetWriteReviewModelAsync Tests

        #region CreateReviewAsync Tests

        [Test]
        public async Task CreateReviewAsync_CreatesReviewSuccessfully_WhenUserHasNotReviewed()
        {
            var model = new AddReviewViewModel
            {
                CatalogDesignId = _testCatalogDesignId,
                Rating = 4,
                Comment = "Great product!"
            };

            _reviewRepoMock.Setup(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId))
                .ReturnsAsync(false);

            _reviewRepoMock.Setup(r => r.AddAsync(It.IsAny<Review>()))
                .Returns(Task.CompletedTask);

            _reviewRepoMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.CreateReviewAsync(_testUserId, model);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Error, Is.Null);
            _reviewRepoMock.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Once);
            _reviewRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task CreateReviewAsync_ReturnsError_WhenUserAlreadyReviewed()
        {
            var model = new AddReviewViewModel
            {
                CatalogDesignId = _testCatalogDesignId,
                Rating = 4,
                Comment = "Great product!"
            };

            _reviewRepoMock.Setup(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId))
                .ReturnsAsync(true);

            var result = await _service.CreateReviewAsync(_testUserId, model);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Error, Is.EqualTo("You have already reviewed this design."));
            _reviewRepoMock.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Never);
        }

        #endregion CreateReviewAsync Tests

        #region GetAllActiveAsync Tests

        [Test]
        public async Task GetAllActiveAsync_ReturnsOnlyActiveReviews()
        {
            var mockQueryable = _testReviews.BuildMockDbSet();

            _reviewRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetAllActiveAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(r => !r.IsDeleted), Is.True);
        }

        [Test]
        public async Task GetAllActiveAsync_ReturnsEmptyList_WhenNoActiveReviews()
        {
            var deletedReviews = _testReviews.Where(r => r.IsDeleted).ToList();

            var mockQueryable = deletedReviews.BuildMockDbSet();

            _reviewRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetAllActiveAsync();

            Assert.That(result, Is.Empty);
        }

        #endregion GetAllActiveAsync Tests

        #region GetAllIncludingDeletedAsync Tests

        [Test]
        public async Task GetAllIncludingDeletedAsync_ReturnsAllReviews()
        {
            var mockQueryable = _testReviews.BuildMockDbSet();
            _reviewRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetAllIncludingDeletedAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
        }

        #endregion GetAllIncludingDeletedAsync Tests

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_ReturnsReview_WhenExists()
        {
            var mockQueryable = _testReviews.BuildMockDbSet();

            _reviewRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetByIdAsync(_testReviewId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testReviewId));
            Assert.That(result.Rating, Is.EqualTo(5));
            Assert.That(result.Comment, Is.EqualTo("Excellent design!"));
        }

        #endregion GetByIdAsync Tests

        #region GetTotalActiveReviewsAsync Tests

        [Test]
        public async Task GetTotalActiveReviewsAsync_ReturnsCorrectCount()
        {
            var mockQueryable = _testReviews.BuildMockDbSet();
            _reviewRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetTotalActiveReviewsAsync();

            Assert.That(result, Is.EqualTo(2));
        }

        #endregion GetTotalActiveReviewsAsync Tests

        #region ToggleReviewAsync Tests

        [Test]
        public async Task ToggleReviewAsync_TogglesReviewStatus()
        {
            var review = new Review { Id = _testReviewId, IsDeleted = false };

            _reviewRepoMock.Setup(r => r.GetByIdIncludingDeletedAsync(_testReviewId))
                .ReturnsAsync(review);

            _reviewRepoMock.Setup(r => r.ToggleReviewStatusAsync(review))
                .Returns(Task.CompletedTask);

            _reviewRepoMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _service.ToggleReviewAsync(_testReviewId);

            _reviewRepoMock.Verify(r => r.ToggleReviewStatusAsync(review), Times.Once);
            _reviewRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        #endregion ToggleReviewAsync Tests

        #region GetReviewCountForDesignAsync Tests

        [Test]
        public async Task GetReviewCountForDesignAsync_ReturnsCorrectCount()
        {
            var mockQueryable = _testReviews.BuildMockDbSet();

            _reviewRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetReviewCountForDesignAsync(_testCatalogDesignId);

            Assert.That(result, Is.EqualTo(2));
        }

        #endregion GetReviewCountForDesignAsync Tests

        #region Additional GetWriteReviewModelAsync Tests

        [Test]
        public async Task GetWriteReviewModelAsync_ReturnsModelWithAverageRating_WhenReviewsExist()
        {
            var designWithReviews = new CatalogDesign
            {
                Id = _testCatalogDesignId,
                Title = "Design With Reviews",
                Description = "Description",
                Image2DUrl = "/images/test.jpg",
                Price = 199.99m,
                Reviews = new List<Review>
        {
            new Review { Rating = 5 },
            new Review { Rating = 4 },
            new Review { Rating = 3 }
        }
            };

            _reviewRepoMock.Setup(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId))
                .ReturnsAsync(false);
            _catalogRepoMock.Setup(r => r.GetByIdWithReviewsAsync(_testCatalogDesignId))
                .ReturnsAsync(designWithReviews);

            var result = await _service.GetWriteReviewModelAsync(_testUserId, _testCatalogDesignId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AverageRating, Is.EqualTo(4.0));
            Assert.That(result.ReviewCount, Is.EqualTo(3));
        }

        [Test]
        public async Task GetWriteReviewModelAsync_ReturnsModelWithZeroRating_WhenNoReviews()
        {
            var designWithoutReviews = new CatalogDesign
            {
                Id = _testCatalogDesignId,
                Title = "Design Without Reviews",
                Reviews = new List<Review>()
            };

            _reviewRepoMock.Setup(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId))
                .ReturnsAsync(false);
            _catalogRepoMock.Setup(r => r.GetByIdWithReviewsAsync(_testCatalogDesignId))
                .ReturnsAsync(designWithoutReviews);

            var result = await _service.GetWriteReviewModelAsync(_testUserId, _testCatalogDesignId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AverageRating, Is.EqualTo(0));
            Assert.That(result.ReviewCount, Is.EqualTo(0));
        }

        #endregion Additional GetWriteReviewModelAsync Tests

        #region Additional GetAllActiveAsync Tests

        [Test]
        public async Task GetAllActiveAsync_HandlesNullUserAndDesign()
        {
            var reviewsWithNulls = new List<Review>
    {
        new Review
        {
            Id = Guid.NewGuid(),
            CatalogDesignId = _testCatalogDesignId,
            UserId = "user-1",
            Rating = 5,
            Comment = "Great!",
            CreatedOn = DateTime.UtcNow,
            IsDeleted = false,
            User = null,
            CatalogDesign = null
        }
    };

            var mockQueryable = reviewsWithNulls.BuildMockDbSet();
            _reviewRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetAllActiveAsync();

            Assert.That(result, Is.Not.Null);
            var review = result.First();
            Assert.That(review.UserName, Is.EqualTo("Unknown"));
            Assert.That(review.UserEmail, Is.EqualTo("Unknown"));
            Assert.That(review.CatalogDesignTitle, Is.EqualTo("Unknown"));
        }

        [Test]
        public async Task GetAllActiveAsync_HandlesNullComment()
        {
            var reviewsWithNullComment = new List<Review>
    {
        new Review
        {
            Id = Guid.NewGuid(),
            CatalogDesignId = _testCatalogDesignId,
            UserId = "user-1",
            Rating = 5,
            Comment = null,
            CreatedOn = DateTime.UtcNow,
            IsDeleted = false,
            User = new AppUser { UserName = "TestUser", Email = "test@test.com" },
            CatalogDesign = new CatalogDesign { Title = "Test Design" }
        }
    };

            var mockQueryable = reviewsWithNullComment.BuildMockDbSet();
            _reviewRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetAllActiveAsync();

            Assert.That(result, Is.Not.Null);
            var review = result.First();
            Assert.That(review.Comment, Is.EqualTo(string.Empty));
        }

        #endregion Additional GetAllActiveAsync Tests

        #region Additional GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenReviewNotFound()
        {
            var emptyList = new List<Review>();
            var mockQueryable = emptyList.BuildMockDbSet();
            _reviewRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetByIdAsync(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        #endregion Additional GetByIdAsync Tests

        #region Additional ToggleReviewAsync Tests

        [Test]
        public void ToggleReviewAsync_ThrowsException_WhenReviewNotFound()
        {
            var nonExistentId = Guid.NewGuid();
            _reviewRepoMock.Setup(r => r.GetByIdIncludingDeletedAsync(nonExistentId))
                .ReturnsAsync((Review)null);

            var ex = Assert.ThrowsAsync<Exception>(
                async () => await _service.ToggleReviewAsync(nonExistentId));

            Assert.That(ex.Message, Is.EqualTo($"Review with ID {nonExistentId} not found"));
        }

        #endregion Additional ToggleReviewAsync Tests

        #region Additional GetReviewsByUserIdAsync Tests

        [Test]
        public async Task GetReviewsByUserIdAsync_HandlesNullUser()
        {
            var reviews = new List<Review>
    {
        new Review
        {
            Id = Guid.NewGuid(),
            CatalogDesignId = _testCatalogDesignId,
            UserId = _testUserId,
            Rating = 5,
            Comment = "Great!",
            CreatedOn = DateTime.UtcNow,
            IsDeleted = false,
            User = null,
            CatalogDesign = new CatalogDesign { Title = "Test Design" }
        }
    };

            var mockQueryable = reviews.BuildMockDbSet();
            _reviewRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetReviewsByUserIdAsync(_testUserId);

            Assert.That(result, Is.Not.Null);
            var review = result.First();
            Assert.That(review.UserName, Is.EqualTo("Unknown"));
            Assert.That(review.UserEmail, Is.EqualTo("Unknown"));
        }

        #endregion Additional GetReviewsByUserIdAsync Tests

        #region Additional GetDetailedReviewsByDesignIdAsync Tests

        [Test]
        public async Task GetDetailedReviewsByDesignIdAsync_OrdersByRatingThenDate()
        {
            var reviews = new List<Review>
    {
        new Review
        {
            Id = Guid.NewGuid(),
            CatalogDesignId = _testCatalogDesignId,
            Rating = 4,
            CreatedOn = DateTime.UtcNow.AddDays(-1),
            IsDeleted = false,
            User = new AppUser { UserName = "User1", Email = "user1@test.com" },
            CatalogDesign = new CatalogDesign { Title = "Design" }
        },
        new Review
        {
            Id = Guid.NewGuid(),
            CatalogDesignId = _testCatalogDesignId,
            Rating = 5,
            CreatedOn = DateTime.UtcNow,
            IsDeleted = false,
            User = new AppUser { UserName = "User2", Email = "user2@test.com" },
            CatalogDesign = new CatalogDesign { Title = "Design" }
        },
        new Review
        {
            Id = Guid.NewGuid(),
            CatalogDesignId = _testCatalogDesignId,
            Rating = 5,
            CreatedOn = DateTime.UtcNow.AddDays(-2),
            IsDeleted = false,
            User = new AppUser { UserName = "User3", Email = "user3@test.com" },
            CatalogDesign = new CatalogDesign { Title = "Design" }
        }
    };

            var mockQueryable = reviews.BuildMockDbSet();
            _reviewRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetDetailedReviewsByDesignIdAsync(_testCatalogDesignId);
            var resultList = result.ToList();

            Assert.That(resultList[0].Rating, Is.EqualTo(5));
            Assert.That(resultList[1].Rating, Is.EqualTo(5));
            Assert.That(resultList[2].Rating, Is.EqualTo(4));
            Assert.That(resultList[0].CreatedAt, Is.GreaterThan(resultList[1].CreatedAt));
        }

        [Test]
        public async Task GetDetailedReviewsByDesignIdAsync_ReturnsEmptyList_WhenNoReviews()
        {
            var emptyList = new List<Review>();
            var mockQueryable = emptyList.BuildMockDbSet();
            _reviewRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetDetailedReviewsByDesignIdAsync(_testCatalogDesignId);

            Assert.That(result, Is.Empty);
        }

        #endregion Additional GetDetailedReviewsByDesignIdAsync Tests

        #region Additional GetAverageRatingForDesignAsync Tests

        [Test]
        public async Task GetAverageRatingForDesignAsync_ReturnsZero_WhenNoReviews()
        {
            var emptyList = new List<Review>();
            var mockQueryable = emptyList.BuildMockDbSet();
            _reviewRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetAverageRatingForDesignAsync(_testCatalogDesignId);

            Assert.That(result, Is.EqualTo(0));
        }

        #endregion Additional GetAverageRatingForDesignAsync Tests

        #region Additional GetReviewCountForDesignAsync Tests

        [Test]
        public async Task GetReviewCountForDesignAsync_ReturnsZero_WhenNoReviews()
        {
            var emptyList = new List<Review>();
            var mockQueryable = emptyList.BuildMockDbSet();
            _reviewRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetReviewCountForDesignAsync(_testCatalogDesignId);

            Assert.That(result, Is.EqualTo(0));
        }

        #endregion Additional GetReviewCountForDesignAsync Tests
    }
}