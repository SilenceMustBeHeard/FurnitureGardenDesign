using Furniture_GardenDesign.Data.Enums;
using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Implementations.Interactions;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Tests.Integration.Repositories.Interactions
{
    [TestFixture]
    public class OrderRepositoryTests
    {
        private ApplicationDbContext _context;
        private OrderRepository _repository;
        private Guid _testOrderId1;
        private Guid _testOrderId2;
        private Guid _testOrderId3;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new OrderRepository(_context);
            _testOrderId1 = Guid.NewGuid();
            _testOrderId2 = Guid.NewGuid();
            _testOrderId3 = Guid.NewGuid();

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
                    Description = "Order description 1",
                    Dimensions = "100x100x100 cm",
                    FurnitureType = "Chair"
                },
                new Order
                {
                    Id = _testOrderId2,
                    UserId = "user-456",
                    Status = OrderStatus.Pending,
                    CreatedOn = DateTime.UtcNow,
                    Description = "Order description 2",
                    Dimensions = "200x200x200 cm",
                    FurnitureType = "Table"
                },
                new Order
                {
                    Id = _testOrderId3,
                    UserId = "user-789",
                    Status = OrderStatus.Approved,
                    CreatedOn = DateTime.UtcNow,
                    Description = "Order description 3",
                    Dimensions = "150x150x150 cm",
                    FurnitureType = "Desk"
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = "user-101",
                    Status = OrderStatus.DesignProvided,
                    CreatedOn = DateTime.UtcNow,
                    Description = "Order description 4",
                    Dimensions = "80x80x80 cm",
                    FurnitureType = "Stool"
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = "user-102",
                    Status = OrderStatus.Rejected,
                    CreatedOn = DateTime.UtcNow,
                    Description = "Order description 5",
                    Dimensions = "90x90x90 cm",
                    FurnitureType = "Lamp"
                }
            };

            var designVariants = new[]
            {
                new DesignVariant
                {
                    Id = Guid.NewGuid(),
                    OrderId = _testOrderId1,
                    Image2DUrl = "/images/variant1.jpg",
                    Notes = "Variant 1 for order 1",
                    IsApproved = false,
                    CreatedOn = DateTime.UtcNow
                },

                new DesignVariant
                {
                    Id = Guid.NewGuid(),
                    OrderId = _testOrderId1,
                    Image2DUrl = "/images/variant2.jpg",
                    Notes = "Variant 2 for order 1",
                    IsApproved = true,
                    CreatedOn = DateTime.UtcNow
                },

                new DesignVariant
                {
                    Id = Guid.NewGuid(),
                    OrderId = _testOrderId2,
                    Image2DUrl = "/images/variant3.jpg",
                    Notes = "Variant 1 for order 2",
                    IsApproved = false,
                    CreatedOn = DateTime.UtcNow
                }
            };

            _context.Orders.AddRange(orders);

            _context.DesignVariants.AddRange(designVariants);

            _context.SaveChanges();
        }

        #region CountPendingAsync Tests

        [Test]
        public async Task CountPendingAsync_ReturnsCorrectCount()
        {
            var result = await _repository.CountPendingAsync();

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public async Task CountPendingAsync_ReturnsZero_WhenNoPendingOrders()
        {
            var allOrders = await _context.Orders.ToListAsync();

            foreach (var order in allOrders)
            {
                order.Status = OrderStatus.Approved;
            }
            await _context.SaveChangesAsync();

            var result = await _repository.CountPendingAsync();

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task CountPendingAsync_DoesNotCountNonPendingStatuses()
        {
            var result = await _repository.CountPendingAsync();

            Assert.That(result, Is.EqualTo(2));
            Assert.That(result, Is.Not.EqualTo(5));
        }

        #endregion CountPendingAsync Tests

        #region GetOrderWithVariantsAsync Tests

        [Test]
        public async Task GetOrderWithVariantsAsync_ReturnsOrderWithVariants_WhenExists()
        {
            var result = await _repository.GetOrderWithVariantsAsync(_testOrderId1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testOrderId1));
            Assert.That(result.DesignVariants, Is.Not.Null);
            Assert.That(result.DesignVariants.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetOrderWithVariantsAsync_ReturnsOrderWithoutVariants_WhenNoVariantsExist()
        {
            var result = await _repository.GetOrderWithVariantsAsync(_testOrderId3);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testOrderId3));
            Assert.That(result.DesignVariants, Is.Empty);
        }

        [Test]
        public async Task GetOrderWithVariantsAsync_ReturnsNull_WhenOrderDoesNotExist()
        {
            var result = await _repository.GetOrderWithVariantsAsync(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetOrderWithVariantsAsync_IncludesAllVariantProperties()
        {
            var result = await _repository.GetOrderWithVariantsAsync(_testOrderId1);
            Assert.That(result, Is.Not.Null);

            var variant = result.DesignVariants.First();

            Assert.That(variant.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(variant.Image2DUrl, Is.Not.Null);
            Assert.That(variant.Notes, Is.Not.Null);
        }

        [Test]
        public async Task GetOrderWithVariantsAsync_ReturnsCorrectVariantCounts()
        {
            var order1 = await _repository.GetOrderWithVariantsAsync(_testOrderId1);
            var order2 = await _repository.GetOrderWithVariantsAsync(_testOrderId2);
            Assert.That(order1, Is.Not.Null);
            Assert.That(order2, Is.Not.Null);

            Assert.That(order1.DesignVariants.Count, Is.EqualTo(2));
            Assert.That(order2.DesignVariants.Count, Is.EqualTo(1));
            Assert.That(order2.DesignVariants, Is.Not.Null);
        }

        #endregion GetOrderWithVariantsAsync Tests

        #region UpdateStatusAsync Tests

        [Test]
        public async Task UpdateStatusAsync_UpdatesOrderStatus_WhenOrderExists()
        {
            var order = await _context.Orders.FindAsync(_testOrderId1);
            Assert.That(order, Is.Not.Null);
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Pending));

            await _repository.UpdateStatusAsync(_testOrderId1, OrderStatus.Approved);

            var updatedOrder = await _context.Orders.FindAsync(_testOrderId1);
            Assert.That(updatedOrder, Is.Not.Null);
            Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.Approved));
        }

        [Test]
        public async Task UpdateStatusAsync_ChangesFromPendingToDesignProvided()
        {
            await _repository.UpdateStatusAsync(_testOrderId1, OrderStatus.DesignProvided);

            var order = await _context.Orders.FindAsync(_testOrderId1);
            Assert.That(order, Is.Not.Null);
            Assert.That(order.Status, Is.EqualTo(OrderStatus.DesignProvided));
        }

        [Test]
        public async Task UpdateStatusAsync_ChangesFromApprovedToRejected()
        {
            await _repository.UpdateStatusAsync(_testOrderId3, OrderStatus.Rejected);

            var order = await _context.Orders.FindAsync(_testOrderId3);
            Assert.That(order, Is.Not.Null);
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Rejected));
        }

        [Test]
        public async Task UpdateStatusAsync_DoesNothing_WhenOrderDoesNotExist()
        {
            var nonExistentId = Guid.NewGuid();
            var orderCount = await _context.Orders.CountAsync();

            await _repository.UpdateStatusAsync(nonExistentId, OrderStatus.Approved);

            var newCount = await _context.Orders.CountAsync();
            Assert.That(newCount, Is.EqualTo(orderCount));
        }

        [Test]
        public async Task UpdateStatusAsync_SavesChangesImmediately()
        {
            await _repository.UpdateStatusAsync(_testOrderId1, OrderStatus.Approved);

            var order = await _context.Orders.FindAsync(_testOrderId1);
            Assert.That(order, Is.Not.Null);
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Approved));
        }

        [Test]
        public async Task UpdateStatusAsync_CanUpdateToAllStatuses()
        {
            await _repository.UpdateStatusAsync(_testOrderId2, OrderStatus.DesignProvided);
            var order1 = await _context.Orders.FindAsync(_testOrderId2);
            Assert.That(order1, Is.Not.Null);
            Assert.That(order1.Status, Is.EqualTo(OrderStatus.DesignProvided));

            await _repository.UpdateStatusAsync(_testOrderId2, OrderStatus.Approved);
            var order2 = await _context.Orders.FindAsync(_testOrderId2);
            Assert.That(order2, Is.Not.Null);
            Assert.That(order2.Status, Is.EqualTo(OrderStatus.Approved));

            await _repository.UpdateStatusAsync(_testOrderId2, OrderStatus.Rejected);
            var order3 = await _context.Orders.FindAsync(_testOrderId2);
            Assert.That(order3, Is.Not.Null);
            Assert.That(order3.Status, Is.EqualTo(OrderStatus.Rejected));
        }

        #endregion UpdateStatusAsync Tests

        #region Additional Repository Method Tests (Inherited)

        [Test]
        public async Task AddAsync_AddsOrderSuccessfully()
        {
            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                UserId = "new-user",
                Status = OrderStatus.Pending,
                CreatedOn = DateTime.UtcNow,
                Description = "New order description",
                Dimensions = "50x50x50 cm",
                FurnitureType = "New Item"
            };

            await _repository.AddAsync(newOrder);

            var savedOrder = await _context.Orders.FindAsync(newOrder.Id);
            Assert.That(savedOrder, Is.Not.Null);
            Assert.That(savedOrder.UserId, Is.EqualTo("new-user"));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsOrder_WhenExists()
        {
            var result = await _repository.GetByIdAsync(_testOrderId1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testOrderId1));
            Assert.That(result.UserId, Is.EqualTo("user-123"));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenOrderDoesNotExist()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetAllAttached_ReturnsAllOrders()
        {
            var result = _repository.GetAllAttached().ToList();

            Assert.That(result.Count, Is.EqualTo(5));
        }

        [Test]
        public async Task UpdateAsync_UpdatesOrderSuccessfully()
        {
            var order = await _repository.GetByIdAsync(_testOrderId1);
            Assert.That(order, Is.Not.Null);
            order.Description = "Updated description";
            order.FurnitureType = "Updated Furniture";

            var result = await _repository.UpdateAsync(order);

            Assert.That(result, Is.True);
            var updatedOrder = await _context.Orders.FindAsync(_testOrderId1);
            Assert.That(updatedOrder, Is.Not.Null);
            Assert.That(updatedOrder.Description, Is.EqualTo("Updated description"));
            Assert.That(updatedOrder.FurnitureType, Is.EqualTo("Updated Furniture"));
        }

        #endregion Additional Repository Method Tests (Inherited)

        #region Edge Cases and Validation Tests

        [Test]
        public async Task GetOrderWithVariantsAsync_HandlesNullVariantsCollection()
        {
            var orderWithoutVariants = new Order
            {
                Id = Guid.NewGuid(),
                UserId = "no-variants-user",
                Status = OrderStatus.Pending,
                CreatedOn = DateTime.UtcNow,
                Description = "No variants order",
                Dimensions = "100x100x100 cm",
                FurnitureType = "Test",
                DesignVariants = null
            };
            _context.Orders.Add(orderWithoutVariants);
            await _context.SaveChangesAsync();

            var result = await _repository.GetOrderWithVariantsAsync(orderWithoutVariants.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.DesignVariants, Is.Empty);
        }

        #endregion Edge Cases and Validation Tests
    }
}