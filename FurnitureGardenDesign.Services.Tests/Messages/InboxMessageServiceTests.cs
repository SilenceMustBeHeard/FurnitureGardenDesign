using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Implementations;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;

namespace FurnitureGardenDesign.Services.Tests.Messages
{
    [TestFixture]
    public class InboxMessageServiceTests
    {
        private Mock<IInboxMessageRepository> _messageRepositoryMock;
        private Mock<ISystemInboxMessageRepository> _systemMessageRepositoryMock;
        private Mock<IContactMessageRepository> _contactMessageRepositoryMock;
        private Mock<IAppUserRepository> _userRepositoryMock;
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private InboxMessageService _inboxMessageService;

        private string _testUserId;
        private string _testAdminId;

        private string _testManagerId;
        private string _testSenderId;
        private Guid _testMessageId;
        private Guid _testDesignVariantId;
        private Guid _testOrderId;
        private InboxMessage _testMessage;
        private ContactMessage _testContactMessage;
        private DesignVariant _testDesignVariant;
        private Order _testOrder;
        private List<InboxMessage> _testMessages;
        private List<AppUser> _testUsers;

        [SetUp]
        public void SetUp()
        {
            _contactMessageRepositoryMock = new Mock<IContactMessageRepository>(MockBehavior.Strict);
            _messageRepositoryMock = new Mock<IInboxMessageRepository>(MockBehavior.Strict);
            _systemMessageRepositoryMock = new Mock<ISystemInboxMessageRepository>(MockBehavior.Strict);
            _userRepositoryMock = new Mock<IAppUserRepository>(MockBehavior.Strict);


            var store = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(
                store.Object, null, null, null, null, null, null, null, null);


            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object, null, null, null, null);

            _inboxMessageService = new InboxMessageService(
                _messageRepositoryMock.Object,
                _contactMessageRepositoryMock.Object,
                _systemMessageRepositoryMock.Object,
                _userManagerMock.Object,
                _userRepositoryMock.Object,
                _roleManagerMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _testUserId = "test-user-123";
            _testAdminId = "admin-123";
            _testManagerId = "manager-123";
            _testSenderId = "sender-123";
            _testMessageId = Guid.NewGuid();
            _testDesignVariantId = Guid.NewGuid();
            _testOrderId = Guid.NewGuid();

            _testOrder = new Order
            {
                Id = _testOrderId,
                Description = "Modern sofa with wooden legs",
                Dimensions = "200x80x75 cm",
                ReferenceImageUrl = "reference.jpg",
                FurnitureType = "Sofa",
                UserId = _testUserId
            };

            _testDesignVariant = new DesignVariant
            {
                Id = _testDesignVariantId,
                OrderId = _testOrderId,
                Image2DUrl = "design.jpg",
                Model3DUrl = "design.glb",
                Notes = "Initial design concept",
                IsApproved = false,
                IsDeleted = false,
                Order = _testOrder
            };

            _testMessage = new InboxMessage
            {
                Id = _testMessageId,
                DesignVariantId = _testDesignVariantId,
                ReceiverId = _testUserId,
                SenderId = _testSenderId,
                Type = InboxMessageType.DesignSent,
                IsRead = false,
                CreatedOn = DateTime.UtcNow.AddDays(-1),
                Notes = "Please review this design",
                DesignVariant = _testDesignVariant
            };

            _testMessages = new List<InboxMessage>
            {
                _testMessage,
                new InboxMessage
                {
                    Id = Guid.NewGuid(),
                    DesignVariantId = _testDesignVariantId,
                    ReceiverId = _testUserId,
                    SenderId = _testSenderId,
                    Type = InboxMessageType.ChangesRequested,
                    IsRead = true,
                    CreatedOn = DateTime.UtcNow.AddDays(-2),
                    DesignVariant = _testDesignVariant
                },
                new InboxMessage
                {
                    Id = Guid.NewGuid(),
                    DesignVariantId = _testDesignVariantId,
                    ReceiverId = _testAdminId,
                    SenderId = _testSenderId,
                    Type = InboxMessageType.DesignSent,
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow.AddDays(-3),
                    DesignVariant = _testDesignVariant
                }
            };

            _testUsers = new List<AppUser>
            {
                new AppUser { Id = _testAdminId, UserName = "admin@test.com" },
                new AppUser { Id = _testManagerId, UserName = "manager@test.com" },
                new AppUser { Id = _testUserId, UserName = "user@test.com" },
                new AppUser { Id = "other-user", UserName = "other@test.com" }
            };
        }

        #region GetUserMessagesAsync Tests

        [Test]
        public async Task GetUserMessagesAsync_ReturnsMessagesForUser_OrderedByCreatedOnDesc()
        {

            var mockQueryable = _testMessages.BuildMockDbSet<InboxMessage>(); // FIXED
            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);


            var result = await _inboxMessageService.GetUserMessagesAsync(_testUserId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2)); // Only messages for _testUserId


            Assert.That(result[0].CreatedOn, Is.GreaterThan(result[1].CreatedOn));


            var firstMessage = result[0];
            Assert.That(firstMessage.Id, Is.EqualTo(_testMessageId));
            Assert.That(firstMessage.DesignVariantId, Is.EqualTo(_testDesignVariantId));
            Assert.That(firstMessage.DesignImage2DUrl, Is.EqualTo("design.jpg"));
            Assert.That(firstMessage.Model3DUrl, Is.EqualTo("design.glb"));
            Assert.That(firstMessage.Notes, Is.EqualTo("Initial design concept"));
            Assert.That(firstMessage.OrderDescription, Is.EqualTo("Modern sofa with wooden legs"));
            Assert.That(firstMessage.OrderDimensions, Is.EqualTo("200x80x75 cm"));
            Assert.That(firstMessage.IsRead, Is.False);
            Assert.That(firstMessage.Type, Is.EqualTo(InboxMessageType.DesignSent));
            Assert.That(firstMessage.IsApproved, Is.False);

            _messageRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetUserMessagesAsync_ExcludesMessagesWithNullDesignVariant()
        {

            var messageWithNullDesign = new InboxMessage
            {
                Id = Guid.NewGuid(),
                DesignVariantId = _testDesignVariantId,
                ReceiverId = _testUserId,
                DesignVariant = null
            };

            var messages = new List<InboxMessage>(_testMessages) { messageWithNullDesign };
            var mockQueryable = messages.BuildMockDbSet<InboxMessage>(); // FIXED
            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _inboxMessageService.GetUserMessagesAsync(_testUserId);


            Assert.That(result.Count, Is.EqualTo(2)); // Should exclude the one with null DesignVariant
            _messageRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetUserMessagesAsync_ExcludesMessagesWithDeletedDesignVariant()
        {

            _testDesignVariant.IsDeleted = true;
            var mockQueryable = _testMessages.BuildMockDbSet<InboxMessage>(); // FIXED
            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);


            var result = await _inboxMessageService.GetUserMessagesAsync(_testUserId);


            Assert.That(result, Is.Empty);
            _messageRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetUserMessagesAsync_ReturnsEmptyList_WhenNoMessagesForUser()
        {

            var mockQueryable = _testMessages.BuildMockDbSet<InboxMessage>();
            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);


            var result = await _inboxMessageService.GetUserMessagesAsync("non-existent-user");


            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            _messageRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        #endregion

        #region MarkMessageAsReadAsync Tests

        [Test]
        public async Task MarkMessageAsReadAsync_MarksMessageAsRead_WhenMessageExists()
        {

            _testMessage.IsRead = false;

            _messageRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<InboxMessage, bool>>>()))
                .ReturnsAsync(_testMessage);

            _messageRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<InboxMessage>()))
                .ReturnsAsync(true);


            await _inboxMessageService.MarkMessageAsReadAsync(_testMessageId, _testUserId);


            Assert.That(_testMessage.IsRead, Is.True);
            _messageRepositoryMock.Verify(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<InboxMessage, bool>>>()), Times.Once);
            _messageRepositoryMock.Verify(r => r.UpdateAsync(_testMessage), Times.Once);
        }

        [Test]
        public async Task MarkMessageAsReadAsync_DoesNothing_WhenMessageDoesNotExist()
        {

            _messageRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<InboxMessage, bool>>>()))
                .ReturnsAsync((InboxMessage)null);


            await _inboxMessageService.MarkMessageAsReadAsync(_testMessageId, _testUserId);


            _messageRepositoryMock.Verify(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<InboxMessage, bool>>>()), Times.Once);
            _messageRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<InboxMessage>()), Times.Never);
        }

        [Test]
        public async Task MarkMessageAsReadAsync_DoesNothing_WhenMessageBelongsToDifferentUser()
        {

            _messageRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<InboxMessage, bool>>>()))
                .ReturnsAsync((InboxMessage)null);


            await _inboxMessageService.MarkMessageAsReadAsync(_testMessageId, "different-user");


            _messageRepositoryMock.Verify(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<InboxMessage, bool>>>()), Times.Once);
            _messageRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<InboxMessage>()), Times.Never);
        }

        #endregion

        #region GetUnreadCountAsync Tests

        [Test]
        public async Task GetUnreadCountAsync_ReturnsSumOfInboxAndSystemUnreadMessages()
        {

            var inboxMessages = new List<InboxMessage>
            {
                new InboxMessage { ReceiverId = _testUserId, IsRead = false },
                new InboxMessage { ReceiverId = _testUserId, IsRead = false },
                new InboxMessage { ReceiverId = _testUserId, IsRead = true }
            };

            var systemMessages = new List<SystemInboxMessage>
            {
                new SystemInboxMessage { ReceiverId = _testUserId, IsRead = false },
                new SystemInboxMessage { ReceiverId = _testUserId, IsRead = true }
            };

            var inboxMock = inboxMessages.BuildMockDbSet<InboxMessage>(); // FIXED
            var systemMock = systemMessages.BuildMockDbSet<SystemInboxMessage>(); // FIXED

            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(inboxMock.Object);

            _systemMessageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(systemMock.Object);


            var result = await _inboxMessageService.GetUnreadCountAsync(_testUserId);


            Assert.That(result, Is.EqualTo(3)); // 2 inbox + 1 system = 3
            _messageRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
            _systemMessageRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetUnreadCountAsync_ReturnsZero_WhenNoUnreadMessages()
        {

            var inboxMessages = new List<InboxMessage>
            {
                new InboxMessage { ReceiverId = _testUserId, IsRead = true }
            };

            var systemMessages = new List<SystemInboxMessage>
            {
                new SystemInboxMessage { ReceiverId = _testUserId, IsRead = true }
            };

            var inboxMock = inboxMessages.BuildMockDbSet<InboxMessage>(); // FIXED
            var systemMock = systemMessages.BuildMockDbSet<SystemInboxMessage>(); // FIXED

            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(inboxMock.Object);

            _systemMessageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(systemMock.Object);


            var result = await _inboxMessageService.GetUnreadCountAsync(_testUserId);


            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetUnreadCountAsync_ReturnsZero_WhenNoMessagesAtAll()
        {

            var emptyInbox = new List<InboxMessage>().BuildMockDbSet<InboxMessage>(); // FIXED
            var emptySystem = new List<SystemInboxMessage>().BuildMockDbSet<SystemInboxMessage>(); // FIXED

            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(emptyInbox.Object);

            _systemMessageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(emptySystem.Object);


            var result = await _inboxMessageService.GetUnreadCountAsync(_testUserId);


            Assert.That(result, Is.EqualTo(0));
        }

        #endregion

        #region GetMessageDetailsAsync Tests

        [Test]
        public async Task GetMessageDetailsAsync_ReturnsMessageDetails_AndMarksAsRead()
        {

            var messages = new List<InboxMessage> { _testMessage };
            var mockQueryable = messages.BuildMockDbSet<InboxMessage>(); // FIXED

            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            _messageRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<InboxMessage>()))
                .ReturnsAsync(true);


            var result = await _inboxMessageService.GetMessageDetailsAsync(_testMessageId, _testUserId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testMessageId));
            Assert.That(result.DesignVariantId, Is.EqualTo(_testDesignVariantId));
            Assert.That(result.DesignImage2DUrl, Is.EqualTo("design.jpg"));
            Assert.That(result.Model3DUrl, Is.EqualTo("design.glb"));
            Assert.That(result.Notes, Is.EqualTo("Initial design concept"));
            Assert.That(result.OrderDescription, Is.EqualTo("Modern sofa with wooden legs"));
            Assert.That(result.OrderDimensions, Is.EqualTo("200x80x75 cm"));
            Assert.That(result.ReferenceImageUrl, Is.EqualTo("reference.jpg"));
            Assert.That(result.IsRead, Is.True); // Should be marked as read
            Assert.That(result.Type, Is.EqualTo(InboxMessageType.DesignSent));

            _messageRepositoryMock.Verify(r => r.UpdateAsync(_testMessage), Times.Once);
        }

        [Test]
        public async Task GetMessageDetailsAsync_ReturnsNull_WhenMessageDoesNotExist()
        {

            var messages = new List<InboxMessage>();
            var mockQueryable = messages.BuildMockDbSet<InboxMessage>(); // FIXED

            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);


            var result = await _inboxMessageService.GetMessageDetailsAsync(_testMessageId, _testUserId);


            Assert.That(result, Is.Null);
            _messageRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<InboxMessage>()), Times.Never);
        }

        [Test]
        public async Task GetMessageDetailsAsync_ReturnsNull_WhenMessageBelongsToDifferentUser()
        {

            var messages = new List<InboxMessage> { _testMessage };
            var mockQueryable = messages.BuildMockDbSet<InboxMessage>(); // FIXED

            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);


            var result = await _inboxMessageService.GetMessageDetailsAsync(_testMessageId, "different-user");


            Assert.That(result, Is.Null);
            _messageRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<InboxMessage>()), Times.Never);
        }

        [Test]
        public async Task GetMessageDetailsAsync_ReturnsNull_WhenDesignVariantIsNull()
        {
            _testMessage.DesignVariant = null;
            var messages = new List<InboxMessage> { _testMessage };
            var mockQueryable = messages.BuildMockDbSet<InboxMessage>(); // FIXED

            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);


            var result = await _inboxMessageService.GetMessageDetailsAsync(_testMessageId, _testUserId);


            Assert.That(result, Is.Null);
            _messageRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<InboxMessage>()), Times.Never);
        }

        #endregion

        #region ApproveDesignAsync Tests

        [Test]
        public async Task ApproveDesignAsync_ApprovesDesign_AndNotifiesAllAdminsAndManagers()
        {

            _testDesignVariant.IsApproved = false;

            var messages = new List<InboxMessage> { _testMessage };
            var mockQueryable = messages.BuildMockDbSet<InboxMessage>();

            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            _messageRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<InboxMessage, bool>>>()))
                .ReturnsAsync(_testMessage);

            _messageRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<InboxMessage>()))
                .Returns(Task.CompletedTask);

            var usersMock = _testUsers.BuildMockDbSet<AppUser>();
            _userRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(usersMock.Object);


            _userManagerMock
                .Setup(um => um.GetRolesAsync(It.Is<AppUser>(u => u.Id == _testAdminId)))
                .ReturnsAsync(new List<string> { "Admin" });

            _userManagerMock
                .Setup(um => um.GetRolesAsync(It.Is<AppUser>(u => u.Id == _testManagerId)))
                .ReturnsAsync(new List<string> { "Manager" });

            _userManagerMock
                .Setup(um => um.GetRolesAsync(It.Is<AppUser>(u => u.Id == _testUserId)))
                .ReturnsAsync(new List<string> { "User" });

            _userManagerMock
                .Setup(um => um.GetRolesAsync(It.Is<AppUser>(u => u.Id == "other-user")))
                .ReturnsAsync(new List<string> { "User" });


            var result = await _inboxMessageService.ApproveDesignAsync(_testMessageId, _testUserId);


            Assert.That(result, Is.Not.Null);
            Assert.That(_testDesignVariant.IsApproved, Is.True);


            _messageRepositoryMock.Verify(r => r.AddAsync(It.Is<InboxMessage>(m =>
                m.Type == InboxMessageType.DesignApproved &&
                m.ReceiverId == _testSenderId)), Times.Once);

            _messageRepositoryMock.Verify(r => r.AddAsync(It.Is<InboxMessage>(m =>
                m.Type == InboxMessageType.DesignApproved &&
                m.ReceiverId == _testAdminId)), Times.Once);

            _messageRepositoryMock.Verify(r => r.AddAsync(It.Is<InboxMessage>(m =>
                m.Type == InboxMessageType.DesignApproved &&
                m.ReceiverId == _testManagerId)), Times.Once);


            _messageRepositoryMock.Verify(r => r.AddAsync(It.Is<InboxMessage>(m =>
                m.ReceiverId == _testUserId)), Times.Never);

            _messageRepositoryMock.Verify(r => r.AddAsync(It.Is<InboxMessage>(m =>
                m.ReceiverId == "other-user")), Times.Never);
        }

        [Test]
        public async Task ApproveDesignAsync_DoesNotApprove_WhenAlreadyApproved()
        {

            _testDesignVariant.IsApproved = true;

            var messages = new List<InboxMessage> { _testMessage };
            var mockQueryable = messages.BuildMockDbSet<InboxMessage>(); // FIXED

            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);


            var result = await _inboxMessageService.ApproveDesignAsync(_testMessageId, _testUserId);


            Assert.That(result, Is.Not.Null);
            Assert.That(_testDesignVariant.IsApproved, Is.True);


            _messageRepositoryMock.Verify(r => r.AddAsync(It.IsAny<InboxMessage>()), Times.Never);
            _userRepositoryMock.Verify(r => r.GetAllAttached(), Times.Never);
        }

        [Test]
        public async Task ApproveDesignAsync_ReturnsNull_WhenMessageDoesNotExist()
        {

            var messages = new List<InboxMessage>();
            var mockQueryable = messages.BuildMockDbSet<InboxMessage>(); // FIXED

            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);


            var result = await _inboxMessageService.ApproveDesignAsync(_testMessageId, _testUserId);


            Assert.That(result, Is.Null);
            _messageRepositoryMock.Verify(r => r.AddAsync(It.IsAny<InboxMessage>()), Times.Never);
        }

        [Test]
        public async Task ApproveDesignAsync_ReturnsNull_WhenDesignVariantIsNull()
        {

            _testMessage.DesignVariant = null;
            var messages = new List<InboxMessage> { _testMessage };
            var mockQueryable = messages.BuildMockDbSet<InboxMessage>(); // FIXED

            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);


            var result = await _inboxMessageService.ApproveDesignAsync(_testMessageId, _testUserId);


            Assert.That(result, Is.Null);
            _messageRepositoryMock.Verify(r => r.AddAsync(It.IsAny<InboxMessage>()), Times.Never);
        }



        #endregion

        #region GetAdminMessagesAsync Tests

        [Test]
        public async Task GetAdminMessagesAsync_ReturnsMessagesForAdmin_WithAllDetails()
        {

            var adminMessages = _testMessages.Where(m => m.ReceiverId == _testAdminId).ToList();
            var mockQueryable = _testMessages.BuildMockDbSet<InboxMessage>();

            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);


            var result = await _inboxMessageService.GetAdminMessagesAsync(_testAdminId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));

            var message = result[0];
            Assert.That(message.DesignVariantId, Is.EqualTo(_testDesignVariantId));
            Assert.That(message.DesignImage2DUrl, Is.EqualTo("design.jpg"));
            Assert.That(message.OrderDescription, Is.EqualTo("Modern sofa with wooden legs"));
            Assert.That(message.OrderDimensions, Is.EqualTo("200x80x75 cm"));

            _messageRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetAdminMessagesAsync_ReturnsEmptyList_WhenNoMessagesForAdmin()
        {

            var mockQueryable = _testMessages.BuildMockDbSet<InboxMessage>();

            _messageRepositoryMock
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);


            var result = await _inboxMessageService.GetAdminMessagesAsync("non-existent-admin");


            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            _messageRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        #endregion

        #region Constructor Tests



        [Test]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {

          var service = new InboxMessageService(
                _messageRepositoryMock.Object,
                _contactMessageRepositoryMock.Object,
                _systemMessageRepositoryMock.Object,
                _userManagerMock.Object,
                _userRepositoryMock.Object,
                _roleManagerMock.Object);

            Assert.That(service, Is.Not.Null);
        }

        #endregion
    }
}