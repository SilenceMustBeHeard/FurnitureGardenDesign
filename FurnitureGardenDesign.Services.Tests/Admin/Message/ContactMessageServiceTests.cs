using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Interfaces.Message;
using FurnitureGardenDesign.Services.Core.Admin.Implementations.Message;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;

namespace FurnitureGardenDesign.Services.Tests.Admin.Message
{
    [TestFixture]
    public class ContactMessageServiceTests
    {
        private Mock<IContactMessageRepository> _messageRepositoryMock;
        private ContactMessageService _service;

        private string _testAdminId;
        private string _testUserId;
        private Guid _testMessageId;
        private ContactMessage _testMessage;
        private List<ContactMessage> _testMessages;

        [SetUp]
        public void SetUp()
        {
            _messageRepositoryMock = new Mock<IContactMessageRepository>(MockBehavior.Strict);
            _service = new ContactMessageService(_messageRepositoryMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _testUserId = "test-user-123";
            _testAdminId = "admin-456";
            _testMessageId = Guid.NewGuid();

            var senderUser = new AppUser
            {
                Id = _testUserId,
                UserName = "john@example.com",
                Email = "john@example.com"
            };

            var receiverUser = new AppUser
            {
                Id = _testAdminId,
                UserName = "admin@example.com",
                Email = "admin@example.com"
            };

            _testMessage = new ContactMessage
            {
                Id = _testMessageId,
                Subject = "Test Subject",
                Message = "Test Message Content",
                ReceiverId = _testAdminId,
                SenderId = _testUserId,
                Sender = senderUser,
                Receiver = receiverUser,
                CreatedOn = DateTime.UtcNow.AddDays(-1),
                IsRead = false,
                IsReadByAdmin = false,
                Response = null
            };

            _testMessages = new List<ContactMessage>
            {
                _testMessage,
                new ContactMessage
                {
                    Id = Guid.NewGuid(),
                    Subject = "Old Message",
                    Message = "Old Content",
                    ReceiverId = _testAdminId,
                    SenderId = _testUserId,
                    Sender = new AppUser
                    {
                        Id = "user-1",
                        UserName = "jane@example.com",
                        Email = "jane@example.com"
                    },
                    CreatedOn = DateTime.UtcNow.AddDays(-2),
                    IsRead = true,
                    IsReadByAdmin = false
                },

                new ContactMessage
                {
                    Id = Guid.NewGuid(),
                    Subject = "New Message",
                    Message = "New Content",
                    ReceiverId = _testAdminId,
                    SenderId = _testUserId,
                    Sender = new AppUser
                    {
                        Id = "user-2",
                        UserName = "bob@example.com",
                        Email = "bob@example.com"
                    },
                    CreatedOn = DateTime.UtcNow,
                    IsRead = false,
                    IsReadByAdmin = false
                },

                new ContactMessage
                {
                    Id = Guid.NewGuid(),
                    Subject = "Other Admin",
                    Message = "Other Content",
                    ReceiverId = "other-admin",
                    SenderId = _testUserId,
                    Sender = new AppUser
                    {
                        Id = "user-3",
                        UserName = "alice@example.com",
                        Email = "alice@example.com"
                    },
                    CreatedOn = DateTime.UtcNow,
                    IsRead = false,
                    IsReadByAdmin = false
                }
            };
        }

        #region GetAdminMessagesAsync Tests

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
        public async Task GetAdminMessagesAsync_ReturnsEmptyList_WhenNoMessagesExist()
        {
            var emptyList = new List<ContactMessage>();

            var mockQueryable = emptyList.BuildMockDbSet();

            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetAdminMessagesAsync(_testAdminId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        #endregion GetAdminMessagesAsync Tests

        #region RespondToMessageAsync Tests

        [Test]
        public void RespondToMessageAsync_WhenMessageNotFound_ThrowsArgumentException()
        {
            var response = "Test response";
            var adminId = "admin-456";

            _messageRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It
                .IsAny<Expression<Func<ContactMessage, bool>>>()))
                .ReturnsAsync((ContactMessage)null!);

            var ex = Assert.ThrowsAsync<ArgumentException>(
                async () => await _service.RespondToMessageAsync(_testMessageId, response, adminId));

            Assert.That(ex.Message, Is.EqualTo("Message not found"));

            _messageRepositoryMock.Verify(r => r.UpdateAsync(It
                .IsAny<ContactMessage>()), Times.Never);
        }

        [Test]
        public void RespondToMessageAsync_WhenAlreadyResponded_ThrowsInvalidOperationException()
        {
            var response = "Test response";
            var adminId = "admin-456";
            var message = new ContactMessage
            {
                Id = _testMessageId,
                Response = "Previous response already sent"
            };

            _messageRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It
                .IsAny<Expression<Func<ContactMessage, bool>>>()))
                .ReturnsAsync(message);

            var ex = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _service.RespondToMessageAsync(_testMessageId, response, adminId));

            Assert.That(ex.Message, Is.EqualTo("This message has already been responded to."));

            _messageRepositoryMock.Verify(r => r.UpdateAsync(It
                .IsAny<ContactMessage>()), Times.Never);
        }

        #endregion RespondToMessageAsync Tests

        #region GetMessageDetailsAsync Tests

        [Test]
        public async Task GetMessageDetailsAsync_MessageNotFound_ReturnsNull()
        {
            var emptyList = new List<ContactMessage>();

            var mockQueryable = emptyList.BuildMockDbSet();

            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetMessageDetailsAsync(_testMessageId, _testAdminId);

            Assert.That(result, Is.Null);

            _messageRepositoryMock.Verify(r => r.UpdateAsync(It
                .IsAny<ContactMessage>()), Times.Never);
        }

        [Test]
        public async Task GetMessageDetailsAsync_WrongAdminId_ReturnsNull()
        {
            var message = new ContactMessage
            {
                Id = _testMessageId,
                ReceiverId = _testAdminId,
                Sender = new AppUser(),
                Receiver = new AppUser()
            };

            var messages = new List<ContactMessage> { message };

            var mockQueryable = messages.BuildMockDbSet();

            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetMessageDetailsAsync(_testMessageId, "wrong-admin");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetMessageDetailsAsync_AlreadyRead_DoesNotUpdateAgain()
        {
            var message = new ContactMessage
            {
                Id = _testMessageId,
                ReceiverId = _testAdminId,
                IsReadByAdmin = true,
                Sender = new AppUser(),
                Receiver = new AppUser()
            };
            var messages = new List<ContactMessage> { message };

            var mockQueryable = messages.BuildMockDbSet();

            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetMessageDetailsAsync(_testMessageId, _testAdminId);

            Assert.That(result, Is.Not.Null);

            _messageRepositoryMock.Verify(r => r.UpdateAsync(It
                .IsAny<ContactMessage>()), Times.Never);
        }

        #endregion GetMessageDetailsAsync Tests

        #region GetUnreadCountAsync Tests

        [Test]
        public async Task GetUnreadCountAsync_ReturnsCorrectCount()
        {
            var messages = new List<ContactMessage>
            {
                new ContactMessage
                {
                    ReceiverId = _testAdminId,
                    IsReadByAdmin = false
                },
                new ContactMessage
                {
                    ReceiverId = _testAdminId,
                    IsReadByAdmin = false
                },
                new ContactMessage
                {
                    ReceiverId = _testAdminId,
                    IsReadByAdmin = true
                },
                new ContactMessage
                {
                    ReceiverId = "other-admin",
                    IsReadByAdmin = false
                }
            };
            var mockQueryable = messages.BuildMockDbSet();

            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetUnreadCountAsync(_testAdminId);

            Assert.That(result, Is.EqualTo(2));

            _messageRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetUnreadCountAsync_ReturnsZero_WhenNoUnreadMessages()
        {
            var messages = new List<ContactMessage>
            {
                new ContactMessage
                {
                    ReceiverId = _testAdminId,
                    IsReadByAdmin = true
                },
                new ContactMessage
                {
                    ReceiverId = _testAdminId,
                    IsReadByAdmin = true
                }
            };
            var mockQueryable = messages.BuildMockDbSet();

            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetUnreadCountAsync(_testAdminId);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetUnreadCountAsync_ReturnsZero_WhenNoMessages()
        {
            var emptyList = new List<ContactMessage>();

            var mockQueryable = emptyList.BuildMockDbSet();

            _messageRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _service.GetUnreadCountAsync(_testAdminId);

            Assert.That(result, Is.EqualTo(0));
        }

        #endregion GetUnreadCountAsync Tests

        #region MarkMessageAsReadAsync Tests

        [Test]
        public async Task MarkMessageAsReadAsync_WhenAlreadyRead_DoesNotUpdate()
        {
            var message = new ContactMessage
            {
                Id = _testMessageId,
                ReceiverId = _testAdminId,
                IsReadByAdmin = true
            };

            _messageRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It
                .IsAny<Expression<Func<ContactMessage, bool>>>()))
                .ReturnsAsync(message);

            await _service.MarkMessageAsReadAsync(_testMessageId, _testAdminId);

            _messageRepositoryMock.Verify(r => r.UpdateAsync(It
                .IsAny<ContactMessage>()), Times.Never);
        }

        [Test]
        public async Task MarkMessageAsReadAsync_WhenMessageNotFound_DoesNotUpdate()
        {
            _messageRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It
                .IsAny<Expression<Func<ContactMessage, bool>>>()))
                .ReturnsAsync((ContactMessage)null!);

            await _service.MarkMessageAsReadAsync(_testMessageId, _testAdminId);

            _messageRepositoryMock.Verify(r => r.UpdateAsync(It
                .IsAny<ContactMessage>()), Times.Never);
        }

        #endregion MarkMessageAsReadAsync Tests

        #region Constructor Tests

        [Test]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {
            var service = new ContactMessageService(_messageRepositoryMock.Object);

            Assert.That(service, Is.Not.Null);
        }

        #endregion Constructor Tests
    }
}