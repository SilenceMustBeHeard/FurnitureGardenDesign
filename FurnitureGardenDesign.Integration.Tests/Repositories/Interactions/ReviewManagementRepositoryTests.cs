using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Implementations.Interactions;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Tests.Integration.Repositories.Interactions
{
    [TestFixture]
    public class ReviewManagementRepositoryTests
    {
        private ApplicationDbContext _context;
        private ReviewManagementRepository _repository;
        private Guid _testCatalogDesignId;
        private Guid _testUserId1;
        private Guid _testUserId2;
        private Guid _activeReviewId;
        private Guid _deletedReviewId;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new ReviewManagementRepository(_context);
            _testCatalogDesignId = Guid.NewGuid();
            _testUserId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
            _testUserId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
            _activeReviewId = Guid.NewGuid();
            _deletedReviewId = Guid.NewGuid();

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
            var user1 = new AppUser
            {
                Id = _testUserId1.ToString(),
                Email = "user1@example.com",
                UserName = "user1@example.com",
                FirstName = "John",
                LastName = "Doe"
            };

            var user2 = new AppUser
            {
                Id = _testUserId2.ToString(),
                Email = "user2@example.com",
                UserName = "user2@example.com",
                FirstName = "Jane",
                LastName = "Smith"
            };

            var catalogDesign = new CatalogDesign
            {
                Id = _testCatalogDesignId,
                Title = "Test Design",
                Description = "Test Description",
                Image2DUrl = "/images/test.jpg",
                Price = 99.99m,
                CategoryId = Guid.NewGuid(),
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow
            };

            var reviews = new[]
            {
                new Review
                {
                    Id = _activeReviewId,
                    CatalogDesignId = _testCatalogDesignId,
                    UserId = _testUserId1.ToString(),
                    Rating = 5,
                    Comment = "Excellent design!",
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Review
                {
                    Id = Guid.NewGuid(),
                    CatalogDesignId = _testCatalogDesignId,
                    UserId = _testUserId2.ToString(),
                    Rating = 4,
                    Comment = "Good design",
                    CreatedOn = DateTime.UtcNow.AddDays(-1),
                    IsDeleted = false
                },
                new Review
                {
                    Id = _deletedReviewId,
                    CatalogDesignId = _testCatalogDesignId,
                    UserId = _testUserId1.ToString(),
                    Rating = 2,
                    Comment = "Deleted review",
                    CreatedOn = DateTime.UtcNow.AddDays(-2),
                    IsDeleted = true
                },
                new Review
                {
                    Id = Guid.NewGuid(),
                    CatalogDesignId = _testCatalogDesignId,
                    UserId = _testUserId2.ToString(),
                    Rating = 3,
                    Comment = "Average review",
                    CreatedOn = DateTime.UtcNow.AddDays(-3),
                    IsDeleted = false
                }
            };

            _context.Users.AddRange(user1, user2);
            _context.CatalogDesigns.Add(catalogDesign);
            _context.Reviews.AddRange(reviews);
            _context.SaveChanges();
        }

        #region GetAllActiveAsync Tests

        [Test]
        public async Task GetAllActiveAsync_ReturnsOnlyActiveReviews()
        {
            var result = await _repository.GetAllActiveAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result.All(r => !r.IsDeleted), Is.True);
            Assert.That(result.Select(r => r.Id), Does.Not.Contain(_deletedReviewId));
        }

        [Test]
        public async Task GetAllActiveAsync_IncludesCatalogDesignNavigation()
        {
            var result = await _repository.GetAllActiveAsync();

            var review = result.First();
            Assert.That(review.CatalogDesign, Is.Not.Null);
            Assert.That(review.CatalogDesign.Title, Is.EqualTo("Test Design"));
        }

        [Test]
        public async Task GetAllActiveAsync_IncludesUserNavigation()
        {
            var result = await _repository.GetAllActiveAsync();

            var review = result.First();

            Assert.That(review.User, Is.Not.Null);
            Assert.That(review.User.Email, Is.EqualTo("user1@example.com"));
        }

        [Test]
        public async Task GetAllActiveAsync_ReturnsEmptyList_WhenNoActiveReviews()
        {
            var allReviews = await _context.Reviews.ToListAsync();
            foreach (var review in allReviews)
            {
                review.IsDeleted = true;
            }
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllActiveAsync();

            Assert.That(result, Is.Empty);
        }

        #endregion GetAllActiveAsync Tests

        #region GetAllForAdminAsync Tests

        [Test]
        public async Task GetAllForAdminAsync_ReturnsAllReviewsIncludingDeleted()
        {
            var result = await _repository.GetAllForAdminAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(4));
            Assert.That(result.Any(r => r.IsDeleted), Is.True);
            Assert.That(result.Select(r => r.Id), Contains.Item(_deletedReviewId));
        }

        [Test]
        public async Task GetAllForAdminAsync_IncludesCatalogDesignNavigation()
        {
            var result = await _repository.GetAllForAdminAsync();

            var review = result.First(r => r.Id == _deletedReviewId);

            Assert.That(review.CatalogDesign, Is.Not.Null);
            Assert.That(review.CatalogDesign.Title, Is.EqualTo("Test Design"));
        }

        [Test]
        public async Task GetAllForAdminAsync_IncludesUserNavigation()
        {
            var result = await _repository.GetAllForAdminAsync();

            var review = result.First(r => r.Id == _activeReviewId);

            Assert.That(review.User, Is.Not.Null);
            Assert.That(review.User.Email, Is.EqualTo("user1@example.com"));
        }

        [Test]
        public async Task GetAllForAdminAsync_OrdersByCreatedOnDescending()
        {
            var result = await _repository.GetAllForAdminAsync();
            var resultList = result.ToList();

            for (int i = 0; i < resultList.Count - 1; i++)
            {
                Assert.That(resultList[i].CreatedOn, Is.GreaterThanOrEqualTo(resultList[i + 1].CreatedOn));
            }
        }

        [Test]
        public async Task GetAllForAdminAsync_ReturnsEmptyList_WhenNoReviews()
        {
            _context.Reviews.RemoveRange(_context.Reviews);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllForAdminAsync();

            Assert.That(result, Is.Empty);
        }

        #endregion GetAllForAdminAsync Tests

        #region ToggleReviewStatusAsync Tests

        [Test]
        public async Task ToggleReviewStatusAsync_TogglesIsDeletedFlag()
        {
            var review = await _context.Reviews.FindAsync(_activeReviewId);
            Assert.That(review, Is.Not.Null);

            var initialStatus = review.IsDeleted;

            await _repository.ToggleReviewStatusAsync(review);

            Assert.That(review.IsDeleted, Is.EqualTo(!initialStatus));
        }

        [Test]
        public async Task ToggleReviewStatusAsync_ChangesFromFalseToTrue()
        {
            var review = await _context.Reviews.FindAsync(_activeReviewId);
            Assert.That(review, Is.Not.Null);
            Assert.That(review.IsDeleted, Is.False);

            await _repository.ToggleReviewStatusAsync(review);

            Assert.That(review.IsDeleted, Is.True);
        }

        [Test]
        public async Task ToggleReviewStatusAsync_ChangesFromTrueToFalse()
        {
            var review = await _context.Reviews.FindAsync(_deletedReviewId);
            Assert.That(review, Is.Not.Null);
            Assert.That(review.IsDeleted, Is.True);

            await _repository.ToggleReviewStatusAsync(review);

            Assert.That(review.IsDeleted, Is.False);
        }

        [Test]
        public async Task ToggleReviewStatusAsync_SavesChangesToDatabase()
        {
            var review = await _context.Reviews.FindAsync(_activeReviewId);

            Assert.That(review, Is.Not.Null);

            var initialStatus = review.IsDeleted;

            await _repository.ToggleReviewStatusAsync(review);

            var reloadedReview = await _context.Reviews
                .AsNoTracking()
                .FirstAsync(r => r.Id == _activeReviewId);

            Assert.That(reloadedReview.IsDeleted, Is.EqualTo(!initialStatus));
        }

        #endregion ToggleReviewStatusAsync Tests

        #region GetByIdIncludingDeletedAsync Tests

        [Test]
        public async Task GetByIdIncludingDeletedAsync_ReturnsActiveReview_WhenExists()
        {
            var result = await _repository.GetByIdIncludingDeletedAsync(_activeReviewId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_activeReviewId));
            Assert.That(result.IsDeleted, Is.False);
            Assert.That(result.Comment, Is.EqualTo("Excellent design!"));
        }

        [Test]
        public async Task GetByIdIncludingDeletedAsync_ReturnsDeletedReview_WhenExists()
        {
            var result = await _repository.GetByIdIncludingDeletedAsync(_deletedReviewId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_deletedReviewId));
            Assert.That(result.IsDeleted, Is.True);
            Assert.That(result.Comment, Is.EqualTo("Deleted review"));
        }

        [Test]
        public async Task GetByIdIncludingDeletedAsync_IncludesCatalogDesignNavigation()
        {
            var result = await _repository.GetByIdIncludingDeletedAsync(_activeReviewId);

            Assert.That(result, Is.Not.Null);

            Assert.That(result.CatalogDesign, Is.Not.Null);
            Assert.That(result.CatalogDesign.Title, Is.EqualTo("Test Design"));
        }

        [Test]
        public async Task GetByIdIncludingDeletedAsync_IncludesUserNavigation()
        {
            var result = await _repository.GetByIdIncludingDeletedAsync(_activeReviewId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.User, Is.Not.Null);
            Assert.That(result.User.Email, Is.EqualTo("user1@example.com"));
        }

        [Test]
        public async Task GetByIdIncludingDeletedAsync_ReturnsNull_WhenReviewDoesNotExist()
        {
            var result = await _repository.GetByIdIncludingDeletedAsync(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        #endregion GetByIdIncludingDeletedAsync Tests

        #region HardDeleteReviewAsync Tests

        [Test]
        public async Task HardDeleteReviewAsync_RemovesReviewPermanently_WhenExists()
        {
            var result = await _repository.HardDeleteReviewAsync(_activeReviewId);

            Assert.That(result, Is.True);

            var deletedReview = await _context.Reviews.FindAsync(_activeReviewId);

            Assert.That(deletedReview, Is.Null);
        }

        [Test]
        public async Task HardDeleteReviewAsync_ReturnsFalse_WhenReviewDoesNotExist()
        {
            var result = await _repository.HardDeleteReviewAsync(Guid.NewGuid());

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task HardDeleteReviewAsync_RemovesCorrectReview()
        {
            await _repository.HardDeleteReviewAsync(_activeReviewId);

            var remainingReviews = await _context.Reviews.ToListAsync();
            Assert.That(remainingReviews.Count, Is.EqualTo(3));
            Assert.That(remainingReviews.Any(r => r.Id == _activeReviewId), Is.False);
            Assert.That(remainingReviews.Any(r => r.Id == _deletedReviewId), Is.True);
        }

        [Test]
        public async Task HardDeleteReviewAsync_DoesNotAffectOtherReviews()
        {
            await _repository.HardDeleteReviewAsync(_activeReviewId);

            var remainingReviews = await _context.Reviews.ToListAsync();

            Assert.That(remainingReviews.Any(r => r.Id == _deletedReviewId), Is.True);
            Assert.That(remainingReviews.Count(r => r.IsDeleted), Is.EqualTo(1));
        }

        #endregion HardDeleteReviewAsync Tests

        #region Edge Cases and Validation Tests

        [Test]
        public async Task GetAllActiveAsync_DoesNotReturnSoftDeletedReviews()
        {
            var result = await _repository.GetAllActiveAsync();

            Assert.That(result.Any(r => r.IsDeleted), Is.False);
        }

        [Test]
        public async Task ToggleReviewStatusAsync_CanBeCalledMultipleTimes()
        {
            var review = await _context.Reviews.FindAsync(_activeReviewId);

            Assert.That(review, Is.Not.Null);
            await _repository.ToggleReviewStatusAsync(review);
            await _repository.ToggleReviewStatusAsync(review);

            Assert.That(review.IsDeleted, Is.False);
        }

        #endregion Edge Cases and Validation Tests
    }
}