using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Implementations.Interactions;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Tests.Integration.Repositories.Interactions
{
    [TestFixture]
    public class ReviewRepositoryTests
    {
        private ApplicationDbContext _context;
        private ReviewRepository _repository;
        private Guid _testCatalogDesignId1;
        private Guid _testCatalogDesignId2;
        private string _testUserId1;
        private string _testUserId2;
        private string _testUserId3;
        private Guid _testReviewId1;
        private Guid _testReviewId2;
        private Guid _testReviewId3;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new ReviewRepository(_context);
            _testCatalogDesignId1 = Guid.NewGuid();
            _testCatalogDesignId2 = Guid.NewGuid();
            _testUserId1 = "11111111-1111-1111-1111-111111111111";
            _testUserId2 = "22222222-2222-2222-2222-222222222222";
            _testUserId3 = "33333333-3333-3333-3333-333333333333";
            _testReviewId1 = Guid.NewGuid();
            _testReviewId2 = Guid.NewGuid();
            _testReviewId3 = Guid.NewGuid();

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
            
            var users = new[]
            {
                new AppUser 
                {
                    Id = _testUserId1, 
                    Email = "user1@example.com",
                    UserName = "user1@example.com" 
                },

                new AppUser 
                { 
                    Id = _testUserId2,
                    Email = "user2@example.com",
                    UserName = "user2@example.com"
                },

                new AppUser
                {
                    Id = _testUserId3,
                    Email = "user3@example.com",
                    UserName = "user3@example.com"
                }
            };

           
            var catalogDesigns = new[]
            {
                new CatalogDesign
                {
                    Id = _testCatalogDesignId1,
                    Title = "Design 1",
                    Description = "First test design",
                    Image2DUrl = "/images/design1.jpg",
                    Price = 99.99m,
                    CategoryId = Guid.NewGuid(),
                    IsDeleted = false,
                    CreatedOn = DateTime.UtcNow
                },
                new CatalogDesign
                {
                    Id = _testCatalogDesignId2,
                    Title = "Design 2",
                    Description = "Second test design",
                    Image2DUrl = "/images/design2.jpg",
                    Price = 149.99m,
                    CategoryId = Guid.NewGuid(),
                    IsDeleted = false,
                    CreatedOn = DateTime.UtcNow
                }
            };

           
            var reviews = new[]
            {
                new Review
                {
                    Id = _testReviewId1,
                    CatalogDesignId = _testCatalogDesignId1,
                    UserId = _testUserId1,
                    Rating = 5,
                    Comment = "Excellent design! Highly recommended.",
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Review
                {
                    Id = _testReviewId2,
                    CatalogDesignId = _testCatalogDesignId1,
                    UserId = _testUserId2,
                    Rating = 4,
                    Comment = "Very good design, but a bit expensive.",
                    CreatedOn = DateTime.UtcNow.AddDays(-1),
                    IsDeleted = false
                },
                new Review
                {
                    Id = _testReviewId3,
                    CatalogDesignId = _testCatalogDesignId2,
                    UserId = _testUserId1,
                    Rating = 3,
                    Comment = "Average design, could be improved.",
                    CreatedOn = DateTime.UtcNow.AddDays(-2),
                    IsDeleted = false
                },
                new Review
                {
                    Id = Guid.NewGuid(),
                    CatalogDesignId = _testCatalogDesignId2,
                    UserId = _testUserId3,
                    Rating = 5,
                    Comment = "Perfect! Exactly what I needed.",
                    CreatedOn = DateTime.UtcNow.AddDays(-3),
                    IsDeleted = false
                }
            };

            _context.Users.AddRange(users);
            _context.CatalogDesigns.AddRange(catalogDesigns);
            _context.Reviews.AddRange(reviews);
            _context.SaveChanges();
        }

        #region HasUserReviewedAsync Tests

        [Test]
        public async Task HasUserReviewedAsync_ReturnsTrue_WhenUserHasReviewedDesign()
        {
          
            var result = await _repository.HasUserReviewedAsync(_testUserId1, _testCatalogDesignId1);

           
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task HasUserReviewedAsync_ReturnsFalse_WhenUserHasNotReviewedDesign()
        {
           
            var result = await _repository.HasUserReviewedAsync(_testUserId3, _testCatalogDesignId1);

           
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task HasUserReviewedAsync_ReturnsFalse_WhenUserReviewedDifferentDesign()
        {
           
            var result = await _repository.HasUserReviewedAsync(_testUserId1, _testCatalogDesignId2);

          
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task HasUserReviewedAsync_ReturnsFalse_WhenDesignDoesNotExist()
        {
            
            var result = await _repository.HasUserReviewedAsync(_testUserId1, Guid.NewGuid());

           
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task HasUserReviewedAsync_ReturnsFalse_WhenUserDoesNotExist()
        {
           
            var result = await _repository.HasUserReviewedAsync("non-existent-user", _testCatalogDesignId1);

          
            Assert.That(result, Is.False);
        }

        #endregion

        #region GetReviewsByDesignIdAsync Tests

        [Test]
        public async Task GetReviewsByDesignIdAsync_ReturnsAllReviews_ForSpecificDesign()
        {
           
            var result = await _repository.GetReviewsByDesignIdAsync(_testCatalogDesignId1);

          
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(r => r.CatalogDesignId == _testCatalogDesignId1), Is.True);
        }

        [Test]
        public async Task GetReviewsByDesignIdAsync_ReturnsEmptyList_WhenDesignHasNoReviews()
        {
            var newDesignId = Guid.NewGuid();

           
            var result = await _repository.GetReviewsByDesignIdAsync(newDesignId);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetReviewsByDesignIdAsync_IncludesCatalogDesignNavigation()
        {
          
            var result = await _repository.GetReviewsByDesignIdAsync(_testCatalogDesignId1);
            var review = result.First();

          
            Assert.That(review.CatalogDesign, Is.Not.Null);
            Assert.That(review.CatalogDesign.Title, Is.EqualTo("Design 1"));
        }

        [Test]
        public async Task GetReviewsByDesignIdAsync_ReturnsReviewsWithCorrectProperties()
        {
           
            var result = await _repository.GetReviewsByDesignIdAsync(_testCatalogDesignId1);
            var review = result.First(r => r.Id == _testReviewId1);

           
            Assert.That(review.Id, Is.EqualTo(_testReviewId1));
            Assert.That(review.UserId, Is.EqualTo(_testUserId1));
            Assert.That(review.Rating, Is.EqualTo(5));
            Assert.That(review.Comment, Is.EqualTo("Excellent design! Highly recommended."));
            Assert.That(review.IsDeleted, Is.False);
        }

        [Test]
        public async Task GetReviewsByDesignIdAsync_ReturnsMultipleReviewsWithDifferentUsers()
        {
          
            var result = await _repository.GetReviewsByDesignIdAsync(_testCatalogDesignId1);
            var resultList = result.ToList();

          
            Assert.That(resultList[0].UserId, Is.EqualTo(_testUserId1));
            Assert.That(resultList[1].UserId, Is.EqualTo(_testUserId2));
        }

        [Test]
        public async Task GetReviewsByDesignIdAsync_ReturnsReviewsWithDifferentRatings()
        {
            
            var result = await _repository.GetReviewsByDesignIdAsync(_testCatalogDesignId1);
            var resultList = result.ToList();

           
            Assert.That(resultList[0].Rating, Is.EqualTo(5));
            Assert.That(resultList[1].Rating, Is.EqualTo(4));
        }

        [Test]
        public async Task GetReviewsByDesignIdAsync_DoesNotReturnReviewsForOtherDesigns()
        {
          
            var resultForDesign1 = await _repository.GetReviewsByDesignIdAsync(_testCatalogDesignId1);
            var resultForDesign2 = await _repository.GetReviewsByDesignIdAsync(_testCatalogDesignId2);

            Assert.That(resultForDesign1.Count(), Is.EqualTo(2));
            Assert.That(resultForDesign2.Count(), Is.EqualTo(2));
            Assert.That(resultForDesign1.All(r => r.CatalogDesignId == _testCatalogDesignId1), Is.True);
            Assert.That(resultForDesign2.All(r => r.CatalogDesignId == _testCatalogDesignId2), Is.True);
        }

        #endregion

        #region Edge Cases and Validation Tests

        [Test]
        public async Task HasUserReviewedAsync_HandlesEmptyUserId()
        {
           
            var result = await _repository.HasUserReviewedAsync(string.Empty, _testCatalogDesignId1);

          
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task HasUserReviewedAsync_HandlesEmptyDesignId()
        {
            
            var result = await _repository.HasUserReviewedAsync(_testUserId1, Guid.Empty);

        
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetReviewsByDesignIdAsync_HandlesNonExistentDesignId()
        {
          
            var result = await _repository.GetReviewsByDesignIdAsync(Guid.NewGuid());

           
            Assert.That(result, Is.Empty);
        }

       

        #endregion

        #region Additional Repository Method Tests (Inherited)

        [Test]
        public async Task AddAsync_AddsReviewSuccessfully()
        {
           
            var newReview = new Review
            {
                Id = Guid.NewGuid(),
                CatalogDesignId = _testCatalogDesignId1,
                UserId = _testUserId3,
                Rating = 5,
                Comment = "New test review",
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

        
            await _repository.AddAsync(newReview);

          
            var savedReview = await _context.Reviews.FindAsync(newReview.Id);

            Assert.That(savedReview, Is.Not.Null);
            Assert.That(savedReview.Comment, Is.EqualTo("New test review"));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsReview_WhenExists()
        {
           
            var result = await _repository.GetByIdAsync(_testReviewId1);

         
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testReviewId1));
            Assert.That(result.Comment, Is.EqualTo("Excellent design! Highly recommended."));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenReviewDoesNotExist()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateAsync_UpdatesReviewSuccessfully()
        {
            
            var review = await _repository.GetByIdAsync(_testReviewId1);

            Assert.That(review, Is.Not.Null);
            review.Rating = 3;
            review.Comment = "Updated comment";

            var result = await _repository.UpdateAsync(review);

          
            Assert.That(result, Is.True);
            var updatedReview = await _context.Reviews.FindAsync(_testReviewId1);

            Assert.That(updatedReview, Is.Not.Null);
            Assert.That(updatedReview.Rating, Is.EqualTo(3));
            Assert.That(updatedReview.Comment, Is.EqualTo("Updated comment"));
        }

        [Test]
        public async Task HardDeleteAsync_RemovesReviewPermanently()
        {
            var review = await _repository.GetByIdAsync(_testReviewId1);
            Assert.That(review, Is.Not.Null);


            var result = await _repository.HardDeleteAsync(review);

           
            Assert.That(result, Is.True);
            var deletedReview = await _context.Reviews.FindAsync(_testReviewId1);
            Assert.That(deletedReview, Is.Null);
        }

        #endregion
    }
}