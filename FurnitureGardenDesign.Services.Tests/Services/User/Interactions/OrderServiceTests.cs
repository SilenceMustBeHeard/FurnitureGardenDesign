using Furniture_GardenDesign.Data.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Interfaces.Interactions;
using FurnitureGardenDesign.Services.Core.Implementations.Interactions;
using FurnitureGardenDesign.Web.ViewModels;
using MockQueryable.Moq;
using Moq;

namespace FurnitureGardenDesign.Unit.Tests.Services.User.Interactions
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IOrderRepository> _orderRepoMock;
        private OrderService _orderService;

        private string _testUserId;
        private Guid _testOrderId;
        private Guid _testCategoryId;
        private Order _testOrder;
        private List<Order> _testOrders;
        private AppUser _testUser;
        private Category _testCategory;

        [SetUp]
        public void SetUp()
        {
            _orderRepoMock = new Mock<IOrderRepository>(MockBehavior.Strict);
            _orderService = new OrderService(_orderRepoMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _testUserId = "22222222-2222-2222-2222-222222222222";
            _testOrderId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            _testCategoryId = Guid.Parse("44444444-4444-4444-4444-444444444444");

            _testUser = new AppUser
            {
                Id = _testUserId,
                Email = "testuser@example.com",
                UserName = "testuser@example.com"
            };

            _testCategory = new Category
            {
                Id = _testCategoryId,
                Name = "Test Category",
                IsDeleted = false
            };

            _testOrder = new Order
            {
                Id = _testOrderId,
                UserId = _testUserId,
                CategoryId = _testCategoryId,
                FurnitureType = "Modern Chair",
                Dimensions = "80x80x90 cm",
                Description = "A comfortable modern chair",
                ReferenceImageUrl = "https://example.com/chair.jpg",
                Status = OrderStatus.Pending,
                CreatedOn = DateTime.UtcNow,
                User = _testUser,
                Category = _testCategory
            };

            _testOrders = new List<Order>
            {
                _testOrder,
                new Order
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    UserId = _testUserId,
                    CategoryId = _testCategoryId,
                    FurnitureType = "Wooden Table",
                    Dimensions = "150x90x75 cm",
                    Description = "Large wooden dining table",
                    Status = OrderStatus.Pending,
                    CreatedOn = DateTime.UtcNow.AddDays(-1),
                    User = _testUser,
                    Category = _testCategory
                },

                new Order
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    UserId = "other-user",
                    CategoryId = _testCategoryId,
                    FurnitureType = "Office Desk",
                    Dimensions = "120x60x75 cm",
                    Description = "Ergonomic office desk",
                    Status = OrderStatus.Pending,
                    CreatedOn = DateTime.UtcNow.AddDays(-2),
                    User = new AppUser { Id = "other-user", Email = "other@example.com" },
                    Category = _testCategory
                }
            };
        }

        #region CreateOrderAsync Tests

        [Test]
        public async Task CreateOrderAsync_CreatesOrderSuccessfully()
        {
            var model = new OrderFormViewModel
            {
                CategoryId = _testCategoryId,
                FurnitureType = "Executive Desk",
                Dimensions = "180x80x75 cm",
                Description = "Premium executive desk with storage",
                ReferenceImageUrl = "https://example.com/desk.jpg"
            };

            _orderRepoMock.Setup(r => r.AddAsync(It.IsAny<Order>()))
                .Returns(Task.CompletedTask);

            await _orderService.CreateOrderAsync(_testUserId, model);

            _orderRepoMock.Verify(r => r.AddAsync(It.Is<Order>(o =>
                o.UserId == _testUserId &&
                o.CategoryId == _testCategoryId &&
                o.FurnitureType == "Executive Desk" &&
                o.Dimensions == "180x80x75 cm" &&
                o.Description == "Premium executive desk with storage" &&
                o.ReferenceImageUrl == "https://example.com/desk.jpg" &&
                o.Status == OrderStatus.Pending &&
                o.CreatedOn != default)), Times.Once);
        }

        [Test]
        public async Task CreateOrderAsync_CreatesOrderWithNullReferenceImage()
        {
            var model = new OrderFormViewModel
            {
                CategoryId = _testCategoryId,
                FurnitureType = "Simple Chair",
                Dimensions = "50x50x80 cm",
                Description = "Basic chair",
                ReferenceImageUrl = null
            };

            _orderRepoMock.Setup(r => r.AddAsync(It.IsAny<Order>()))
                .Returns(Task.CompletedTask);

            await _orderService.CreateOrderAsync(_testUserId, model);

            _orderRepoMock.Verify(r => r.AddAsync(It.Is<Order>(o =>
                o.ReferenceImageUrl == null)), Times.Once);
        }

        #endregion CreateOrderAsync Tests

        #region GetPendingOrdersCountAsync Tests

        [Test]
        public async Task GetPendingOrdersCountAsync_ReturnsCorrectCount()
        {
            _orderRepoMock.Setup(r => r.CountPendingAsync())
                .ReturnsAsync(5);

            var result = await _orderService.GetPendingOrdersCountAsync();

            Assert.That(result, Is.EqualTo(5));

            _orderRepoMock.Verify(r => r.CountPendingAsync(), Times.Once);
        }

        [Test]
        public async Task GetPendingOrdersCountAsync_ReturnsZero_WhenNoPendingOrders()
        {
            _orderRepoMock.Setup(r => r.CountPendingAsync())
                .ReturnsAsync(0);

            var result = await _orderService.GetPendingOrdersCountAsync();

            Assert.That(result, Is.EqualTo(0));
        }

        #endregion GetPendingOrdersCountAsync Tests

        #region GetPendingOrdersAsync Tests

        [Test]
        public async Task GetPendingOrdersAsync_ReturnsOnlyPendingOrders()
        {
            var pendingOrders = _testOrders.Where(o => o.Status == OrderStatus.Pending).ToList();
            var mockQueryable = pendingOrders.BuildMockDbSet();

            _orderRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _orderService.GetPendingOrdersAsync();

            Assert.That(result, Is.Not.Null);

            Assert.That(result.All(o => o.Status == OrderStatus.Pending), Is.True);
        }

        [Test]
        public async Task GetPendingOrdersAsync_ReturnsEmptyList_WhenNoPendingOrders()
        {
            var ordersWithNoPending = _testOrders.Where(o => o.Status != OrderStatus.Pending).ToList();
            var mockQueryable = ordersWithNoPending.BuildMockDbSet();

            _orderRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _orderService.GetPendingOrdersAsync();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetPendingOrdersAsync_ReturnsCorrectProperties()
        {
            var mockQueryable = _testOrders.BuildMockDbSet();

            _orderRepoMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _orderService.GetPendingOrdersAsync();

            var order = result.First(o => o.Id == _testOrderId);
            Assert.That(order.Id, Is.EqualTo(_testOrderId));
            Assert.That(order.UserEmail, Is.EqualTo("testuser@example.com"));
            Assert.That(order.CategoryName, Is.EqualTo("Test Category"));
            Assert.That(order.Description, Is.EqualTo("A comfortable modern chair"));
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Pending));
            Assert.That(order.CreatedOn, Is.EqualTo(_testOrder.CreatedOn));
        }

        #endregion GetPendingOrdersAsync Tests

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_ReturnsOrder_WhenExists()
        {
            _orderRepoMock.Setup(r => r.GetByIdAsync(_testOrderId))
                .ReturnsAsync(_testOrder);

            var result = await _orderService.GetByIdAsync(_testOrderId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testOrderId));
            Assert.That(result.UserId, Is.EqualTo(_testUserId));
            Assert.That(result.CategoryId, Is.EqualTo(_testCategoryId));
            Assert.That(result.FurnitureType, Is.EqualTo("Modern Chair"));
            Assert.That(result.Dimensions, Is.EqualTo("80x80x90 cm"));
            Assert.That(result.Description, Is.EqualTo("A comfortable modern chair"));
            Assert.That(result.ReferenceImageUrl, Is.EqualTo("https://example.com/chair.jpg"));
            Assert.That(result.Status, Is.EqualTo(OrderStatus.Pending));

            _orderRepoMock.Verify(r => r.GetByIdAsync(_testOrderId), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenOrderNotFound()
        {
            var nonExistentId = Guid.Parse("99999999-9999-9999-9999-999999999999");

            _orderRepoMock.Setup(r => r.GetByIdAsync(nonExistentId))
                .ReturnsAsync((Order)null!);

            var result = await _orderService.GetByIdAsync(nonExistentId);

            Assert.That(result, Is.Null);
        }

        #endregion GetByIdAsync Tests

        #region RejectOrderAsync Tests

        [Test]
        public async Task RejectOrderAsync_SetsStatusToRejected_WhenOrderExists()
        {
            var order = new Order
            {
                Id = _testOrderId,
                Status = OrderStatus.Pending
            };

            _orderRepoMock.Setup(r => r.GetByIdAsync(_testOrderId))
                .ReturnsAsync(order);

            _orderRepoMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _orderService.RejectOrderAsync(_testOrderId);

            Assert.That(result, Is.True);
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Rejected));
            _orderRepoMock.Verify(r => r.GetByIdAsync(_testOrderId), Times.Once);
            _orderRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task RejectOrderAsync_ReturnsFalse_WhenOrderNotFound()
        {
            var nonExistentId = Guid.Parse("99999999-9999-9999-9999-999999999999");

            _orderRepoMock.Setup(r => r.GetByIdAsync(nonExistentId))
                .ReturnsAsync((Order)null!);

            var result = await _orderService.RejectOrderAsync(nonExistentId);

            Assert.That(result, Is.False);
            _orderRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task RejectOrderAsync_CanRejectOrderWithAnyStatus()
        {
            var order = new Order
            {
                Id = _testOrderId,
                Status = OrderStatus.Pending,
            };

            _orderRepoMock.Setup(r => r.GetByIdAsync(_testOrderId))
                .ReturnsAsync(order);

            _orderRepoMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _orderService.RejectOrderAsync(_testOrderId);

            Assert.That(result, Is.True);
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Rejected));
        }

        #endregion RejectOrderAsync Tests

        #region DeleteOrderAsync Tests

        [Test]
        public async Task DeleteOrderAsync_DeletesOrder_WhenExists()
        {
            var order = new Order
            {
                Id = _testOrderId
            };

            _orderRepoMock.Setup(r => r.GetByIdAsync(_testOrderId))
                .ReturnsAsync(order);

            _orderRepoMock.Setup(r => r.DeleteAsync(order))
                .ReturnsAsync(true);

            _orderRepoMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _orderService.DeleteOrderAsync(_testOrderId);

            Assert.That(result, Is.True);

            _orderRepoMock.Verify(r => r.GetByIdAsync(_testOrderId), Times.Once);
            _orderRepoMock.Verify(r => r.DeleteAsync(order), Times.Once);
            _orderRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteOrderAsync_ReturnsFalse_WhenOrderNotFound()
        {
            var nonExistentId = Guid.Parse("99999999-9999-9999-9999-999999999999");

            _orderRepoMock.Setup(r => r.GetByIdAsync(nonExistentId))
                .ReturnsAsync((Order)null!);

            var result = await _orderService.DeleteOrderAsync(nonExistentId);

            Assert.That(result, Is.False);

            _orderRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Order>()), Times.Never);

            _orderRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        #endregion DeleteOrderAsync Tests
    }
}