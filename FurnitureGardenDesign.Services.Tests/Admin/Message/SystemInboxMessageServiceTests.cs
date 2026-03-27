using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Interfaces.Account;
using FurnitureGardenDesign.Data.Repository.Interfaces.Message;
using FurnitureGardenDesign.Services.Core.Admin.Implementations.Message;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;

namespace FurnitureGardenDesign.Services.Tests.Admin.Message
{
    [TestFixture]
    public class SystemInboxMessageServiceTests
    {
        private Mock<ISystemInboxMessageRepository> _messageRepositoryMock;
        private Mock<IAppUserRepository> _userRepositoryMock;
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private SystemInboxMessageService _service;

        private string _testUserId;
        private string _testAdminId;
        private Guid _testMessageId;
        private SystemInboxMessage _testMessage;
        private List<SystemInboxMessage> _testMessages;

        [SetUp]
        public void SetUp()
        {
            _messageRepositoryMock = new Mock<ISystemInboxMessageRepository>(MockBehavior.Strict);
            _userRepositoryMock = new Mock<IAppUserRepository>(MockBehavior.Strict);

            var userStoreMock = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                roleStoreMock.Object, null, null, null, null);

            _service = new SystemInboxMessageService(
                _messageRepositoryMock.Object,
                _userManagerMock.Object,
                _userRepositoryMock.Object,
                _roleManagerMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _testUserId = "test-user-123";
            _testAdminId = "admin-456";
            _testMessageId = Guid.NewGuid();

            _testMessage = new SystemInboxMessage
            {
                Id = _testMessageId,
                ReceiverId = _testUserId,
                SenderId = _testAdminId,
                Description = "Test system message",
                Type = InboxMessageType.SystemMessage,
                IsRead = false,
                CreatedOn = DateTime.UtcNow.AddDays(-1)
            };

            _testMessages = new List<SystemInboxMessage>
            {
                _testMessage,
                new SystemInboxMessage
                {
                    Id = Guid.NewGuid(),
                    ReceiverId = _testUserId,
                    SenderId = _testAdminId,
                    Description = "Another system message",
                    Type = InboxMessageType.SystemMessage,
                    IsRead = true,
                    CreatedOn = DateTime.UtcNow.AddDays(-2)
                },
                new SystemInboxMessage
                {
                    Id = Guid.NewGuid(),
                    ReceiverId = _testAdminId,
                    SenderId = _testUserId,
                    Description = "Message for admin",
                    Type = InboxMessageType.SystemMessage,
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow
                }
            };
        }

        #region MarkMessageAsReadAsync Tests

      
       

       

        [Test]
        public async Task GetUnreadCountAsync_ReturnsCorrectCount()
        {
           
            var messages = new List<SystemInboxMessage>
            {
                new SystemInboxMessage 
                { 
                    ReceiverId = _testUserId, 
                    IsRead = false 
                },
                new SystemInboxMessage 
                {
                    ReceiverId = _testUserId,
                    IsRead = false 
                },
                new SystemInboxMessage 
                {
                    ReceiverId = _testUserId
                    , IsRead = true 
                }
            };
            var mockQueryable = messages.BuildMockDbSet();
            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            
            var result = await _service.GetUnreadCountAsync(_testUserId);

            
            Assert.That(result, Is.EqualTo(2));
            _messageRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetUnreadCountAsync_ReturnsZero_WhenNoUnreadMessages()
        {
            
            var messages = new List<SystemInboxMessage>
            {
                new SystemInboxMessage 
                {
                    ReceiverId = _testUserId,
                    IsRead = true 
                },
                new SystemInboxMessage 
                {
                    ReceiverId = _testUserId, 
                    IsRead = true 
                }
            };

            var mockQueryable = messages.BuildMockDbSet();
            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            
            var result = await _service.GetUnreadCountAsync(_testUserId);

            
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetUnreadCountAsync_ReturnsZero_WhenNoMessages()
        {
            
            var emptyList = new List<SystemInboxMessage>();
            var mockQueryable = emptyList.BuildMockDbSet();
            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            
            var result = await _service.GetUnreadCountAsync(_testUserId);

            
            Assert.That(result, Is.EqualTo(0));
        }

        #endregion

        #region GetMessageDetailsAsync Tests

        [Test]
        public async Task GetMessageDetailsAsync_ValidMessage_ReturnsDetailsAndMarksAsRead()
        {
            
            var messages = new List<SystemInboxMessage> { _testMessage };
            var mockQueryable = messages.BuildMockDbSet();
            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            _messageRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<SystemInboxMessage>()))
                .ReturnsAsync(true);

            
            var result = await _service.GetMessageDetailsAsync(_testMessageId, _testUserId);

            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testMessageId));
            Assert.That(result.Description, Is.EqualTo(_testMessage.Description));
            Assert.That(result.IsRead, Is.True);
            Assert.That(result.Type, Is.EqualTo(_testMessage.Type));

            _messageRepositoryMock.Verify(r => r.UpdateAsync(It.Is<SystemInboxMessage>(m => m.IsRead == true)), Times.Once);
        }

        [Test]
        public async Task GetMessageDetailsAsync_MessageNotFound_ReturnsNull()
        {
            
            var emptyList = new List<SystemInboxMessage>();
            var mockQueryable = emptyList.BuildMockDbSet();
            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            
            var result = await _service.GetMessageDetailsAsync(_testMessageId, _testUserId);

            
            Assert.That(result, Is.Null);
            _messageRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SystemInboxMessage>()), Times.Never);
        }

        [Test]
        public async Task GetMessageDetailsAsync_WrongUser_ReturnsNull()
        {
            
            var messages = new List<SystemInboxMessage> { _testMessage };
            var mockQueryable = messages.BuildMockDbSet();
            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            
            var result = await _service.GetMessageDetailsAsync(_testMessageId, "wrong-user");

            
            Assert.That(result, Is.Null);
        }

        #endregion

        #region CreateMessageAsync Tests

        [Test]
        public async Task CreateMessageAsync_AddsMessage()
        {
            
            var newMessage = new SystemInboxMessage
            {
                Id = Guid.NewGuid(),
                ReceiverId = _testUserId,
                SenderId = _testAdminId,
                Description = "New message",
                Type = InboxMessageType.SystemMessage,
                IsRead = false,
                CreatedOn = DateTime.UtcNow
            };

            _messageRepositoryMock.Setup(r => r.AddAsync(It.IsAny<SystemInboxMessage>()))
                .Returns(Task.CompletedTask);

            
            await _service.CreateMessageAsync(newMessage);

            
            _messageRepositoryMock.Verify(r => r.AddAsync(newMessage), Times.Once);
        }

        #endregion

        #region GetUserMessagesAsync Tests

       

        [Test]
        public async Task GetUserMessagesAsync_ReturnsCorrectMessageProperties()
        {
            
            var mockQueryable = _testMessages.BuildMockDbSet();
            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            
            var result = await _service.GetUserMessagesAsync(_testUserId);

           
            var firstMessage = result.First(m => m.Id == _testMessageId);
            Assert.That(firstMessage.Id, Is.EqualTo(_testMessageId));
            Assert.That(firstMessage.Description, Is.EqualTo(_testMessage.Description));
            Assert.That(firstMessage.IsRead, Is.EqualTo(_testMessage.IsRead));
            Assert.That(firstMessage.Type, Is.EqualTo(_testMessage.Type));
            Assert.That(firstMessage.SenderId, Is.EqualTo(_testMessage.SenderId));
        }

        [Test]
        public async Task GetUserMessagesAsync_ReturnsEmptyList_WhenNoMessages()
        {
           
            var emptyList = new List<SystemInboxMessage>();
            var mockQueryable = emptyList.BuildMockDbSet();
            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetUserMessagesAsync(_testUserId);

           
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        #endregion

        #region GetAdminMessagesAsync Tests

        [Test]
        public async Task GetAdminMessagesAsync_ReturnsMessagesForAdmin()
        {
            
            var mockQueryable = _testMessages.BuildMockDbSet();
            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

           
            var result = await _service.GetAdminMessagesAsync(_testAdminId);

        
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(_testMessages[2].Id));
            _messageRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetAdminMessagesAsync_ReturnsEmptyList_WhenNoMessagesForAdmin()
        {
          
            var mockQueryable = _testMessages.BuildMockDbSet();
            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

           
            var result = await _service.GetAdminMessagesAsync("non-existent-admin");

           
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAdminMessagesAsync_OrdersByCreatedOnDesc()
        {
           
            var adminMessages = new List<SystemInboxMessage>
            {
                new SystemInboxMessage
                {
                    Id = Guid.NewGuid(),
                    ReceiverId = _testAdminId,
                    CreatedOn = DateTime.UtcNow.AddDays(-1),
                    Description = "Older"
                },
                new SystemInboxMessage
                {
                    Id = Guid.NewGuid(),
                    ReceiverId = _testAdminId,
                    CreatedOn = DateTime.UtcNow,
                    Description = "Newer"
                }
            };
            var mockQueryable = adminMessages.BuildMockDbSet();
            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

          
            var result = await _service.GetAdminMessagesAsync(_testAdminId);

           
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Description, Is.EqualTo("Newer"));
            Assert.That(result[1].Description, Is.EqualTo("Older"));
        }

        #endregion

        #region Constructor Tests

        [Test]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {
           
            var service = new SystemInboxMessageService(
                _messageRepositoryMock.Object,
                _userManagerMock.Object,
                _userRepositoryMock.Object,
                _roleManagerMock.Object);

           
            Assert.That(service, Is.Not.Null);
        }

        #endregion
    }
}