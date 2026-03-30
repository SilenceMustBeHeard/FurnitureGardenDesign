using Furniture_GardenDesign.Data.Enums;
using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Interfaces.Interactions;
using FurnitureGardenDesign.Data.Repository.Interfaces.Message;
using FurnitureGardenDesign.Services.Core.Manager.Implementations.Interactions;
using FurnitureGardenDesign.Web.ViewModels.DesignVariants;
using MockQueryable.Moq;
using Moq;

namespace FurnitureGardenDesign.Tests.Unit.Services.Manager.Implementations.Interactions
{
    [TestFixture]
    public class ManagerDesignVariantServiceTests
    {
        private Mock<IDesignVariantRepository> _designVariantRepositoryMock;
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<IInboxMessageRepository> _inboxMessageRepositoryMock;
        private ManagerDesignVariantService _service;

        private Guid _testOrderId;
        private Guid _testDesignVariantId;
        private DesignVariant _testDesignVariant;
        private Order _testOrder;
        private List<DesignVariant> _testDesignVariants;

        [SetUp]
        public void SetUp()
        {
            _designVariantRepositoryMock = new Mock<IDesignVariantRepository>(MockBehavior.Strict);
            _orderRepositoryMock = new Mock<IOrderRepository>(MockBehavior.Strict);
            _inboxMessageRepositoryMock = new Mock<IInboxMessageRepository>(MockBehavior.Strict);

            _service = new ManagerDesignVariantService(
                _designVariantRepositoryMock.Object,
                _orderRepositoryMock.Object,
                _inboxMessageRepositoryMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _testOrderId = Guid.NewGuid();
            _testDesignVariantId = Guid.NewGuid();

            _testOrder = new Order
            {
                Id = _testOrderId,
                UserId = "customer-123",
                Status = OrderStatus.Pending
            };

            _testDesignVariant = new DesignVariant
            {
                Id = _testDesignVariantId,
                OrderId = _testOrderId,
                Order = _testOrder,
                Image2DUrl = "/images/test.jpg",
                Model3DUrl = "/models/test.glb",
                Notes = "Test notes",
                IsApproved = false
            };

            _testDesignVariants = new List<DesignVariant>
            {
                _testDesignVariant,
                new DesignVariant
                {
                    Id = Guid.NewGuid(),
                    OrderId = _testOrderId,
                    Image2DUrl = "/images/test2.jpg",
                    Model3DUrl = null,
                    Notes = "Another design",
                    IsApproved = true
                },
                new DesignVariant
                {
                    Id = Guid.NewGuid(),
                    OrderId = Guid.NewGuid(),
                    Image2DUrl = "/images/test3.jpg",
                    Model3DUrl = "/models/test3.glb",
                    Notes = "Different order",
                    IsApproved = false
                }
            };
        }

        #region GetDesignVariantsByOrderIdAsync Tests

        [Test]
        public async Task GetDesignVariantsByOrderIdAsync_ReturnsVariants_WhenOrderExists()
        {
            var variantsForOrder = _testDesignVariants
                .Where(v => v.OrderId == _testOrderId)
                .ToList();

            _designVariantRepositoryMock
                .Setup(r => r.GetByOrderId(_testOrderId))
                .ReturnsAsync(variantsForOrder);

            var result = await _service.GetDesignVariantsByOrderIdAsync(_testOrderId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(_testDesignVariantId));
            _designVariantRepositoryMock.Verify(r => r.GetByOrderId(_testOrderId), Times.Once);
        }

        [Test]
        public async Task GetDesignVariantsByOrderIdAsync_ReturnsEmptyList_WhenNoVariantsForOrder()
        {
            var emptyList = new List<DesignVariant>();
            _designVariantRepositoryMock
                .Setup(r => r.GetByOrderId(_testOrderId))
                .ReturnsAsync(emptyList);

            var result = await _service.GetDesignVariantsByOrderIdAsync(_testOrderId);

            Assert.That(result, Is.Empty);
        }

        #endregion GetDesignVariantsByOrderIdAsync Tests

        #region GetDesignVariantByIdAsync Tests

        [Test]
        public async Task GetDesignVariantByIdAsync_ReturnsVariant_WhenExists()
        {
            var mockQueryable = _testDesignVariants.BuildMockDbSet();
            _designVariantRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetDesignVariantByIdAsync(_testDesignVariantId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testDesignVariantId));
            Assert.That(result.Notes, Is.EqualTo("Test notes"));
            Assert.That(result.OrderId, Is.EqualTo(_testOrderId));
            _designVariantRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public void GetDesignVariantByIdAsync_ThrowsKeyNotFoundException_WhenVariantDoesNotExist()
        {
            var nonExistentId = Guid.NewGuid();
            var emptyList = new List<DesignVariant>();
            var mockQueryable = emptyList.BuildMockDbSet();
            _designVariantRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(
                async () => await _service.GetDesignVariantByIdAsync(nonExistentId));

            Assert.That(ex.Message, Is.EqualTo($"Design variant with ID {nonExistentId} not found."));
        }

        #endregion GetDesignVariantByIdAsync Tests

        #region CreateDesignVariantAsync Tests

        [Test]
        public async Task CreateDesignVariantAsync_CreatesAndReturnsVariant()
        {
            var model = new DesignVariantViewModel
            {
                OrderId = _testOrderId,
                Image2DUrl = "/images/new.jpg",
                Model3DUrl = "/models/new.glb",
                Notes = "New design notes"
            };

            _designVariantRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<DesignVariant>()))
                .Returns(Task.CompletedTask);
            _designVariantRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.CreateDesignVariantAsync(model);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.OrderId, Is.EqualTo(_testOrderId));
            Assert.That(result.Image2DUrl, Is.EqualTo("/images/new.jpg"));
            Assert.That(result.Model3DUrl, Is.EqualTo("/models/new.glb"));
            Assert.That(result.Notes, Is.EqualTo("New design notes"));
            Assert.That(result.IsApproved, Is.False);
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));

            _designVariantRepositoryMock.Verify(r => r.AddAsync(It.IsAny<DesignVariant>()), Times.Once);
            _designVariantRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task CreateDesignVariantAsync_CreatesVariantWithNullModel3DUrl()
        {
            var model = new DesignVariantViewModel
            {
                OrderId = _testOrderId,
                Image2DUrl = "/images/new.jpg",
                Model3DUrl = null,
                Notes = "No 3D model"
            };

            _designVariantRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<DesignVariant>()))
                .Returns(Task.CompletedTask);
            _designVariantRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.CreateDesignVariantAsync(model);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model3DUrl, Is.Null);
        }

        #endregion CreateDesignVariantAsync Tests

        #region UpdateDesignVariantAsync Tests

        [Test]
        public async Task UpdateDesignVariantAsync_UpdatesExistingVariant()
        {
            var existingVariant = new DesignVariant
            {
                Id = _testDesignVariantId,
                Image2DUrl = "/images/old.jpg",
                Model3DUrl = "/models/old.glb",
                Notes = "Old notes",
                IsApproved = false
            };

            var updatedVariant = new DesignVariant
            {
                Id = _testDesignVariantId,
                Image2DUrl = "/images/updated.jpg",
                Model3DUrl = "/models/updated.glb",
                Notes = "Updated notes",
                IsApproved = true
            };

            _designVariantRepositoryMock
                .Setup(r => r.GetByIdAsync(_testDesignVariantId))
                .ReturnsAsync(existingVariant);
            _designVariantRepositoryMock
                .Setup(r => r.Update(It.IsAny<DesignVariant>()))
                .Returns(true);
            _designVariantRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _service.UpdateDesignVariantAsync(updatedVariant);

            Assert.That(existingVariant.Image2DUrl, Is.EqualTo("/images/updated.jpg"));
            Assert.That(existingVariant.Model3DUrl, Is.EqualTo("/models/updated.glb"));
            Assert.That(existingVariant.Notes, Is.EqualTo("Updated notes"));
            Assert.That(existingVariant.IsApproved, Is.True);

            _designVariantRepositoryMock.Verify(r => r.GetByIdAsync(_testDesignVariantId), Times.Once);
            _designVariantRepositoryMock.Verify(r => r.Update(existingVariant), Times.Once);
            _designVariantRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateDesignVariantAsync_ThrowsKeyNotFoundException_WhenVariantNotFound()
        {
            var nonExistentId = Guid.NewGuid();
            var updatedVariant = new DesignVariant
            {
                Id = nonExistentId,
                Image2DUrl = "/images/updated.jpg",
                Notes = "Updated notes"
            };

            _designVariantRepositoryMock
                .Setup(r => r.GetByIdAsync(nonExistentId))
                .ReturnsAsync((DesignVariant)null);

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(
                async () => await _service.UpdateDesignVariantAsync(updatedVariant));

            Assert.That(ex.Message, Is.EqualTo($"Design variant with ID {nonExistentId} not found."));
        }

        #endregion UpdateDesignVariantAsync Tests

        #region SendDesignVariantProposalAsync Tests

        [Test]
        public async Task SendDesignVariantProposalAsync_SendsProposalSuccessfully()
        {
            var designVariant = new DesignVariant
            {
                Id = _testDesignVariantId,
                OrderId = _testOrderId,
                Order = new Order
                {
                    Id = _testOrderId,
                    UserId = "customer-123",
                    Status = OrderStatus.Pending
                }
            };

            var mockQueryable = new List<DesignVariant> { designVariant }.BuildMockDbSet();
            _designVariantRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            _orderRepositoryMock
                .Setup(r => r.UpdateStatusAsync(_testOrderId, OrderStatus.DesignProvided))
                .Returns(Task.CompletedTask);

            _inboxMessageRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<InboxMessage>()))
                .Returns(Task.CompletedTask);
            _inboxMessageRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _service.SendDesignVariantProposalAsync(_testDesignVariantId);

            _designVariantRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
            _orderRepositoryMock.Verify(r => r.UpdateStatusAsync(_testOrderId, OrderStatus.DesignProvided), Times.Once);
            _inboxMessageRepositoryMock.Verify(r => r.AddAsync(It.Is<InboxMessage>(msg =>
                msg.DesignVariantId == _testDesignVariantId &&
                msg.ReceiverId == "customer-123" &&
                msg.Type == InboxMessageType.DesignSent &&
                !msg.IsRead)), Times.Once);
            _inboxMessageRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void SendDesignVariantProposalAsync_ThrowsKeyNotFoundException_WhenVariantNotFound()
        {
            var nonExistentId = Guid.NewGuid();
            var emptyList = new List<DesignVariant>();
            var mockQueryable = emptyList.BuildMockDbSet();
            _designVariantRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(
                async () => await _service.SendDesignVariantProposalAsync(nonExistentId));

            Assert.That(ex.Message, Is.EqualTo("Design variant not found."));
        }

        #endregion SendDesignVariantProposalAsync Tests

        #region DeleteDesignVariantAsync Tests

        [Test]
        public async Task DeleteDesignVariantAsync_DeletesVariant_WhenExists()
        {
            _designVariantRepositoryMock
                .Setup(r => r.GetByIdAsync(_testDesignVariantId))
                .ReturnsAsync(_testDesignVariant);
            _designVariantRepositoryMock
                .Setup(r => r.Delete(_testDesignVariant))
                .Returns(true);
            _designVariantRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _service.DeleteDesignVariantAsync(_testDesignVariantId);

            _designVariantRepositoryMock.Verify(r => r.GetByIdAsync(_testDesignVariantId), Times.Once);
            _designVariantRepositoryMock.Verify(r => r.Delete(_testDesignVariant), Times.Once);
            _designVariantRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteDesignVariantAsync_ThrowsKeyNotFoundException_WhenVariantNotFound()
        {
            var nonExistentId = Guid.NewGuid();
            _designVariantRepositoryMock
                .Setup(r => r.GetByIdAsync(nonExistentId))
                .ReturnsAsync((DesignVariant)null);

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(
                async () => await _service.DeleteDesignVariantAsync(nonExistentId));

            Assert.That(ex.Message, Is.EqualTo($"Design variant with ID {nonExistentId} not found."));
        }

        #endregion DeleteDesignVariantAsync Tests
    }
}