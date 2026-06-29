using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Interfaces.Catalog;
using FurnitureGardenDesign.Data.Repository.Interfaces.Interactions;
using FurnitureGardenDesign.Services.Core.Implementations.Interactions;
using FurnitureGardenDesign.Web.ViewModels.Review;
using Moq;

namespace FurnitureGardenDesign.Unit.Tests.Services.User.Interactions
{
    [TestFixture]
    public class ReviewServiceTests
    {
        private Mock<IReviewRepository> _reviewRepoMock;
        private Mock<ICatalogRepository> _catalogRepoMock;
        private ReviewService _reviewService;

        private string _testUserId;
        private Guid _testCatalogDesignId;
        private Guid _testReviewId;
        private CatalogDesign _testCatalogDesign;
        private Review _testReview;
        private List<Review> _testReviews;
        private AppUser _testUser;

        [SetUp]
        public void SetUp()
        {
            _reviewRepoMock = new Mock<IReviewRepository>(MockBehavior.Strict);
            _catalogRepoMock = new Mock<ICatalogRepository>(MockBehavior.Strict);
            _reviewService = new ReviewService(_reviewRepoMock.Object, _catalogRepoMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _testUserId = "22222222-2222-2222-2222-222222222222";
            _testCatalogDesignId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            _testReviewId = Guid.Parse("44444444-4444-4444-4444-444444444444");

            _testUser = new AppUser
            {
                Id = _testUserId,
                Email = "testuser@example.com",
                UserName = "testuser@example.com"
            };

            _testCatalogDesign = new CatalogDesign
            {
                Id = _testCatalogDesignId,
                Title = "Test Design",
                Description = "Test Description",
                Image2DUrl = "/images/test.jpg",
                Model3DUrl = "/models/test.glb",
                Price = 99.99m,
                IsActive = true,
                Reviews = new List<Review>()
            };

            _testReview = new Review
            {
                Id = _testReviewId,
                CatalogDesignId = _testCatalogDesignId,
                UserId = _testUserId,
                Rating = 5,
                Comment = "Excellent design!",
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false,
                User = _testUser,
                CatalogDesign = _testCatalogDesign
            };

            _testReviews = new List<Review>
            {
                _testReview,
                new Review
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    CatalogDesignId = _testCatalogDesignId,
                    UserId = "other-user",
                    Rating = 4,
                    Comment = "Good design",
                    CreatedOn = DateTime.UtcNow.AddDays(-1),
                    IsDeleted = false,
                    User = new AppUser { Id = "other-user", Email = "other@example.com" },
                    CatalogDesign = _testCatalogDesign
                },

                new Review
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    CatalogDesignId = _testCatalogDesignId,
                    UserId = "another-user",
                    Rating = 3,
                    Comment = "Average",
                    CreatedOn = DateTime.UtcNow.AddDays(-2),
                    IsDeleted = true,
                    User = new AppUser { Id = "another-user", Email = "another@example.com" },
                    CatalogDesign = _testCatalogDesign
                }
            };
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

            await _reviewService.AddReviewAsync(_testUserId, model);

            _reviewRepoMock.Verify(r => r.AddAsync(It.Is<Review>(review =>
                review.CatalogDesignId == _testCatalogDesignId &&
                review.UserId == _testUserId &&
                review.Rating == 4 &&
                review.Comment == "Great product!")), Times.Once);
            _reviewRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        #endregion AddReviewAsync Tests

        #region HasUserReviewedAsync Tests

        [Test]
        public async Task HasUserReviewedAsync_ReturnsTrue_WhenUserHasReviewed()
        {
            _reviewRepoMock.Setup(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId))
                .ReturnsAsync(true);

            var result = await _reviewService.HasUserReviewedAsync(_testUserId, _testCatalogDesignId);

            Assert.That(result, Is.True);
            _reviewRepoMock.Verify(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId), Times.Once);
        }

        [Test]
        public async Task HasUserReviewedAsync_ReturnsFalse_WhenUserHasNotReviewed()
        {
            _reviewRepoMock.Setup(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId))
                .ReturnsAsync(false);

            var result = await _reviewService.HasUserReviewedAsync(_testUserId, _testCatalogDesignId);

            Assert.That(result, Is.False);
            _reviewRepoMock.Verify(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId), Times.Once);
        }

        #endregion HasUserReviewedAsync Tests

        #region GetReviewsByDesignIdAsync Tests

        [Test]
        public async Task GetReviewsByDesignIdAsync_ReturnsReviewsForDesign()
        {
            var activeReviews = _testReviews.Where(r => !r.IsDeleted).ToList();

            _reviewRepoMock.Setup(r => r.GetReviewsByDesignIdAsync(_testCatalogDesignId))
                .ReturnsAsync(activeReviews);

            var result = await _reviewService.GetReviewsByDesignIdAsync(_testCatalogDesignId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Rating, Is.EqualTo(5));
            Assert.That(result.First().Comment, Is.EqualTo("Excellent design!"));
            Assert.That(result.First().CatalogDesignTitle, Is.EqualTo("Test Design"));
            _reviewRepoMock.Verify(r => r.GetReviewsByDesignIdAsync(_testCatalogDesignId), Times.Once);
        }

        [Test]
        public async Task GetReviewsByDesignIdAsync_ReturnsEmptyList_WhenNoReviews()
        {
            var emptyList = new List<Review>();

            _reviewRepoMock.Setup(r => r.GetReviewsByDesignIdAsync(_testCatalogDesignId))
                .ReturnsAsync(emptyList);

            var result = await _reviewService.GetReviewsByDesignIdAsync(_testCatalogDesignId);

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

            var result = await _reviewService.GetWriteReviewModelAsync(_testUserId, _testCatalogDesignId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testCatalogDesignId));
            Assert.That(result.Title, Is.EqualTo("Test Design"));
            Assert.That(result.Description, Is.EqualTo("Test Description"));
            Assert.That(result.Image2DUrl, Is.EqualTo("/images/test.jpg"));
            Assert.That(result.Model3DUrl, Is.EqualTo("/models/test.glb"));
            Assert.That(result.Price, Is.EqualTo(99.99m));
            Assert.That(result.AverageRating, Is.EqualTo(0));
            Assert.That(result.ReviewCount, Is.EqualTo(0));

            _reviewRepoMock.Verify(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId), Times.Once);
            _catalogRepoMock.Verify(r => r.GetByIdWithReviewsAsync(_testCatalogDesignId), Times.Once);
        }

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

            var result = await _reviewService.GetWriteReviewModelAsync(_testUserId, _testCatalogDesignId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AverageRating, Is.EqualTo(4.0));
            Assert.That(result.ReviewCount, Is.EqualTo(3));
        }

        [Test]
        public async Task GetWriteReviewModelAsync_ReturnsNull_WhenUserAlreadyReviewed()
        {
            _reviewRepoMock.Setup(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId))
                .ReturnsAsync(true);

            var result = await _reviewService.GetWriteReviewModelAsync(_testUserId, _testCatalogDesignId);

            Assert.That(result, Is.Null);
            _reviewRepoMock.Verify(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId), Times.Once);
            _catalogRepoMock.Verify(r => r.GetByIdWithReviewsAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task GetWriteReviewModelAsync_ReturnsNull_WhenDesignNotFound()
        {
            _reviewRepoMock.Setup(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId))
                .ReturnsAsync(false);
            _catalogRepoMock.Setup(r => r.GetByIdWithReviewsAsync(_testCatalogDesignId))
                .ReturnsAsync((CatalogDesign)null!);

            var result = await _reviewService.GetWriteReviewModelAsync(_testUserId, _testCatalogDesignId);

            Assert.That(result, Is.Null);
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

            var result = await _reviewService.CreateReviewAsync(_testUserId, model);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Error, Is.Null);

            _reviewRepoMock.Verify(r => r.AddAsync(It.Is<Review>(review =>
                review.CatalogDesignId == _testCatalogDesignId &&
                review.UserId == _testUserId &&
                review.Rating == 4 &&
                review.Comment == "Great product!")), Times.Once);

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

            var result = await _reviewService.CreateReviewAsync(_testUserId, model);

            Assert.That(result.Success, Is.False);

            Assert.That(result.Error, Is.EqualTo("You have already reviewed this design."));

            _reviewRepoMock.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Never);
            _reviewRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task CreateReviewAsync_HandlesMinimumRating()
        {
            var model = new AddReviewViewModel
            {
                CatalogDesignId = _testCatalogDesignId,
                Rating = 1,
                Comment = "Poor quality"
            };

            _reviewRepoMock.Setup(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId))
                .ReturnsAsync(false);

            _reviewRepoMock.Setup(r => r.AddAsync(It.IsAny<Review>()))
                .Returns(Task.CompletedTask);

            _reviewRepoMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _reviewService.CreateReviewAsync(_testUserId, model);

            Assert.That(result.Success, Is.True);
            _reviewRepoMock.Verify(r => r.AddAsync(It.Is<Review>(review => review.Rating == 1)), Times.Once);
        }

        [Test]
        public async Task CreateReviewAsync_HandlesMaximumRating()
        {
            var model = new AddReviewViewModel
            {
                CatalogDesignId = _testCatalogDesignId,
                Rating = 5,
                Comment = "Excellent!"
            };

            _reviewRepoMock.Setup(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId))
                .ReturnsAsync(false);

            _reviewRepoMock.Setup(r => r.AddAsync(It.IsAny<Review>()))
                .Returns(Task.CompletedTask);

            _reviewRepoMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _reviewService.CreateReviewAsync(_testUserId, model);

            Assert.That(result.Success, Is.True);

            _reviewRepoMock.Verify(r => r.AddAsync(It.Is<Review>(review => review.Rating == 5)), Times.Once);
        }

        [Test]
        public async Task CreateReviewAsync_HandlesEmptyComment()
        {
            var model = new AddReviewViewModel
            {
                CatalogDesignId = _testCatalogDesignId,
                Rating = 3,
                Comment = ""
            };

            _reviewRepoMock.Setup(r => r.HasUserReviewedAsync(_testUserId, _testCatalogDesignId))
                .ReturnsAsync(false);

            _reviewRepoMock.Setup(r => r.AddAsync(It.IsAny<Review>()))
                .Returns(Task.CompletedTask);

            _reviewRepoMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _reviewService.CreateReviewAsync(_testUserId, model);

            Assert.That(result.Success, Is.True);

            _reviewRepoMock.Verify(r => r.AddAsync(It.Is<Review>(review => review.Comment == "")), Times.Once);
        }

        #endregion CreateReviewAsync Tests
    }
}