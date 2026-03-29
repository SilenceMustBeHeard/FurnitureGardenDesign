using Furniture_GardenDesign.Data.Enums;
using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Implementations.Interactions;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Tests.Integration.Repositories.Interactions
{
    [TestFixture]
    public class DesignVariantRepositoryTests
    {
        private ApplicationDbContext _context;
        private DesignVariantRepository _repository;
        private Guid _testOrderId1;
        private Guid _testOrderId2;
        private Guid _testDesignVariantId1;
        private Guid _testDesignVariantId2;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new DesignVariantRepository(_context);
            _testOrderId1 = Guid.NewGuid();
            _testOrderId2 = Guid.NewGuid();
            _testDesignVariantId1 = Guid.NewGuid();
            _testDesignVariantId2 = Guid.NewGuid();

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
            var orders = new[]
            {
                new Order
                {
                    Id = _testOrderId1,
                    UserId = "user-123",
                    Status = OrderStatus.Pending,
                    CreatedOn = DateTime.UtcNow,
                    // Required properties
                    Description = "Test order description 1",
                    Dimensions = "100x100x100 cm",
                    FurnitureType = "Chair"
                },
                new Order
                {
                    Id = _testOrderId2,
                    UserId = "user-456",
                    Status = OrderStatus.Pending,
                    CreatedOn = DateTime.UtcNow,
                    // Required properties
                    Description = "Test order description 2",
                    Dimensions = "200x200x200 cm",
                    FurnitureType = "Table"
                }
            };

            var designVariants = new[]
            {
                new DesignVariant
                {
                    Id = _testDesignVariantId1,
                    OrderId = _testOrderId1,
                    Image2DUrl = "/images/variant1.jpg",
                    Model3DUrl = "/models/variant1.glb",
                    Notes = "First design variant",
                    IsApproved = false,
                    CreatedOn = DateTime.UtcNow
                },
                new DesignVariant
                {
                    Id = _testDesignVariantId2,
                    OrderId = _testOrderId1,
                    Image2DUrl = "/images/variant2.jpg",
                    Model3DUrl = null,
                    Notes = "Second design variant for same order",
                    IsApproved = true,
                    CreatedOn = DateTime.UtcNow.AddDays(-1)
                },
                new DesignVariant
                {
                    Id = Guid.NewGuid(),
                    OrderId = _testOrderId2,
                    Image2DUrl = "/images/variant3.jpg",
                    Model3DUrl = "/models/variant3.glb",
                    Notes = "Design variant for different order",
                    IsApproved = false,
                    CreatedOn = DateTime.UtcNow.AddDays(-2)
                },
                new DesignVariant
                {
                    Id = Guid.NewGuid(),
                    OrderId = _testOrderId1,
                    Image2DUrl = "/images/variant4.jpg",
                    Model3DUrl = "/models/variant4.glb",
                    Notes = "Third variant for first order",
                    IsApproved = false,
                    CreatedOn = DateTime.UtcNow.AddDays(-3)
                }
            };

            _context.Orders.AddRange(orders);
            _context.DesignVariants.AddRange(designVariants);
            _context.SaveChanges();
        }

        #region GetByOrderId Tests

        [Test]
        public async Task GetByOrderId_ReturnsAllVariants_ForSpecificOrder()
        {
            var result = await _repository.GetByOrderId(_testOrderId1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result.All(v => v.OrderId == _testOrderId1), Is.True);
        }

        [Test]
        public async Task GetByOrderId_ReturnsEmptyList_WhenOrderHasNoVariants()
        {
            var emptyOrderId = Guid.NewGuid();

            var result = await _repository.GetByOrderId(emptyOrderId);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetByOrderId_ReturnsVariantsWithCorrectProperties()
        {
            var result = await _repository.GetByOrderId(_testOrderId1);
            var variant = result.First(v => v.Id == _testDesignVariantId1);

            Assert.That(variant.Id, Is.EqualTo(_testDesignVariantId1));
            Assert.That(variant.OrderId, Is.EqualTo(_testOrderId1));
            Assert.That(variant.Image2DUrl, Is.EqualTo("/images/variant1.jpg"));
            Assert.That(variant.Model3DUrl, Is.EqualTo("/models/variant1.glb"));
            Assert.That(variant.Notes, Is.EqualTo("First design variant"));
            Assert.That(variant.IsApproved, Is.False);
        }

        [Test]
        public async Task GetByOrderId_ReturnsVariantsWithNullModel3DUrl()
        {
            var result = await _repository.GetByOrderId(_testOrderId1);
            var variant = result.First(v => v.Id == _testDesignVariantId2);

            Assert.That(variant.Model3DUrl, Is.Null);
        }

        [Test]
        public async Task GetByOrderId_ReturnsVariantsWithDifferentApprovalStatuses()
        {
            var result = await _repository.GetByOrderId(_testOrderId1);

            Assert.That(result.Any(v => v.IsApproved == true), Is.True);
            Assert.That(result.Any(v => v.IsApproved == false), Is.True);
        }

        [Test]
        public async Task GetByOrderId_WithEmptyGuid_ReturnsEmptyList()
        {
            var result = await _repository.GetByOrderId(Guid.Empty);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetByOrderId_ReturnsVariantsOnlyForSpecifiedOrder_NotOthers()
        {
            var resultForOrder1 = await _repository.GetByOrderId(_testOrderId1);
            var resultForOrder2 = await _repository.GetByOrderId(_testOrderId2);

            Assert.That(resultForOrder1.Count(), Is.EqualTo(3));
            Assert.That(resultForOrder2.Count(), Is.EqualTo(1));
        }

        #endregion GetByOrderId Tests

        #region Additional Repository Method Tests

        [Test]
        public async Task AddAsync_AddsDesignVariantSuccessfully()
        {
            var newVariant = new DesignVariant
            {
                Id = Guid.NewGuid(),
                OrderId = _testOrderId1,
                Image2DUrl = "/images/new.jpg",
                Model3DUrl = "/models/new.glb",
                Notes = "New variant",
                IsApproved = false,
                CreatedOn = DateTime.UtcNow
            };

            await _repository.AddAsync(newVariant);

            var savedVariant = await _context.DesignVariants.FindAsync(newVariant.Id);

            Assert.That(savedVariant, Is.Not.Null);
            Assert.That(savedVariant.Notes, Is.EqualTo("New variant"));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsVariant_WhenExists()
        {
            var result = await _repository.GetByIdAsync(_testDesignVariantId1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testDesignVariantId1));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenVariantDoesNotExist()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateAsync_UpdatesVariantSuccessfully()
        {
            var variant = await _repository.GetByIdAsync(_testDesignVariantId1);
            Assert.That(variant, Is.Not.Null);
            variant.Notes = "Updated notes";
            variant.IsApproved = true;

            var result = await _repository.UpdateAsync(variant);

            Assert.That(result, Is.True);

            var updatedVariant = await _context.DesignVariants.FindAsync(_testDesignVariantId1);

            Assert.That(updatedVariant.Notes, Is.EqualTo("Updated notes"));
            Assert.That(updatedVariant.IsApproved, Is.True);
        }

        [Test]
        public async Task HardDeleteAsync_RemovesVariantPermanently()
        {
            var variant = await _repository.GetByIdAsync(_testDesignVariantId1);

            var result = await _repository.HardDeleteAsync(variant);

            Assert.That(result, Is.True);

            var deletedVariant = await _context.DesignVariants.FindAsync(_testDesignVariantId1);

            Assert.That(deletedVariant, Is.Null);
        }

        #endregion Additional Repository Method Tests
    }
}