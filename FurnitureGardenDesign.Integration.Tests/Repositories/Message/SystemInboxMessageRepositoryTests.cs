using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Implementations.Message;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Tests.Integration.Repositories.Message
{
    [TestFixture]
    public class SystemInboxMessageRepositoryTests
    {
        private ApplicationDbContext _context;
        private SystemInboxMessageRepository _repository;
        private Guid _testMessageId1;
        private Guid _testMessageId2;
        private Guid _testMessageId3;
        private string _testSenderId;
        private string _testReceiverId;
        private string _testAdminId;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new SystemInboxMessageRepository(_context);
            _testMessageId1 = Guid.NewGuid();
            _testMessageId2 = Guid.NewGuid();
            _testMessageId3 = Guid.NewGuid();
            _testSenderId = "11111111-1111-1111-1111-111111111111";
            _testReceiverId = "22222222-2222-2222-2222-222222222222";
            _testAdminId = "33333333-3333-3333-3333-333333333333";

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
            var sender = new AppUser
            {
                Id = _testSenderId,
                Email = "sender@example.com",
                UserName = "sender@example.com",
                FirstName = "System",
                LastName = "Sender"
            };

            var receiver = new AppUser
            {
                Id = _testReceiverId,
                Email = "receiver@example.com",
                UserName = "receiver@example.com",
                FirstName = "Jane",
                LastName = "Receiver"
            };

            var admin = new AppUser
            {
                Id = _testAdminId,
                Email = "admin@example.com",
                UserName = "admin@example.com",
                FirstName = "Admin",
                LastName = "User"
            };

            var messages = new[]
            {
                new SystemInboxMessage
                {
                    Id = _testMessageId1,
                    SenderId = _testSenderId,
                    ReceiverId = _testReceiverId,
                    Type = InboxMessageType.SystemMessage,
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow,
                    Description = "Welcome to the platform!"
                },
                new SystemInboxMessage
                {
                    Id = _testMessageId2,
                    SenderId = _testAdminId,
                    ReceiverId = _testReceiverId,
                    Type = InboxMessageType.SystemMessage,
                    IsRead = true,
                    CreatedOn = DateTime.UtcNow.AddDays(-1),
                    Description = "Your order #12345 has been shipped."
                },
                new SystemInboxMessage
                {
                    Id = _testMessageId3,
                    SenderId = _testSenderId,
                    ReceiverId = _testAdminId,
                    Type = InboxMessageType.SystemMessage,
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow.AddDays(-2),
                    Description = "New user registered: john@example.com"
                },
                new SystemInboxMessage
                {
                    Id = Guid.NewGuid(),
                    SenderId = _testSenderId,
                    ReceiverId = _testReceiverId,
                    Type = InboxMessageType.SystemMessage,
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow.AddDays(-3),
                    Description = "Your subscription will expire soon."
                }
            };

            _context.Users.AddRange(sender, receiver, admin);
            _context.SystemInboxMessages.AddRange(messages);
            _context.SaveChanges();
        }

        #region AddAsync Tests

        [Test]
        public async Task AddAsync_AddsSystemMessageSuccessfully()
        {
            var newMessage = new SystemInboxMessage
            {
                Id = Guid.NewGuid(),
                SenderId = _testSenderId,
                ReceiverId = _testReceiverId,
                Type = InboxMessageType.SystemMessage,
                IsRead = false,
                CreatedOn = DateTime.UtcNow,
                Description = "New system notification"
            };

            await _repository.AddAsync(newMessage);

            var savedMessage = await _context.SystemInboxMessages.FindAsync(newMessage.Id);

            Assert.That(savedMessage, Is.Not.Null);
            Assert.That(savedMessage.Description, Is.EqualTo("New system notification"));

            Assert.That(savedMessage.IsRead, Is.False);
        }

        [Test]
        public async Task AddAsync_AddsMessageWithNullActionUrl()
        {
            var newMessage = new SystemInboxMessage
            {
                Id = Guid.NewGuid(),
                SenderId = _testSenderId,
                ReceiverId = _testReceiverId,
                Type = InboxMessageType.SystemMessage,
                IsRead = false,
                CreatedOn = DateTime.UtcNow,
                Description = "System message without action URL"
            };

            await _repository.AddAsync(newMessage);

            var savedMessage = await _context.SystemInboxMessages.FindAsync(newMessage.Id);

            Assert.That(savedMessage, Is.Not.Null);
            Assert.That(savedMessage.Description, Is.EqualTo("System message without action URL"));
        }

        #endregion AddAsync Tests

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_ReturnsMessage_WhenExists()
        {
            var result = await _repository.GetByIdAsync(_testMessageId1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testMessageId1));
            Assert.That(result.Description, Is.EqualTo("Welcome to the platform!"));
            Assert.That(result.IsRead, Is.False);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenMessageDoesNotExist()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        #endregion GetByIdAsync Tests

        #region GetAllAttached Tests

        [Test]
        public async Task GetAllAttached_ReturnsAllMessages()
        {
            var result = _repository.GetAllAttached().ToList();

            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(result.Select(m => m.Description), Contains.Item("Welcome to the platform!"));
            Assert.That(result.Select(m => m.Description), Contains.Item("Your order #12345 has been shipped."));
            Assert.That(result.Select(m => m.Description), Contains.Item("New user registered: john@example.com"));
            Assert.That(result.Select(m => m.Description), Contains.Item("Your subscription will expire soon."));
        }

        [Test]
        public async Task GetAllAttached_ReturnsEmptyList_WhenNoMessages()
        {
            _context.SystemInboxMessages.RemoveRange(_context.SystemInboxMessages);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAttached().ToListAsync();

            Assert.That(result, Is.Empty);
        }

        #endregion GetAllAttached Tests

        #region FirstOrDefaultAsync Tests

        [Test]
        public async Task FirstOrDefaultAsync_ReturnsMessage_WhenConditionMatches()
        {
            var result = await _repository.FirstOrDefaultAsync(m => m
            .Description.Contains("shipped") && m.ReceiverId == _testReceiverId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testMessageId2));
            Assert.That(result.IsRead, Is.True);
        }

        [Test]
        public async Task FirstOrDefaultAsync_ReturnsNull_WhenConditionDoesNotMatch()
        {
            var result = await _repository.FirstOrDefaultAsync(m => m.Description == "Non-existent message");

            Assert.That(result, Is.Null);
        }

        #endregion FirstOrDefaultAsync Tests

        #region UpdateAsync Tests

        [Test]
        public async Task UpdateAsync_UpdatesMessageSuccessfully()
        {
            var message = await _repository.GetByIdAsync(_testMessageId1);

            Assert.That(message, Is.Not.Null);

            message.IsRead = true;
            message.Description = "Updated welcome message";

            var result = await _repository.UpdateAsync(message);

            Assert.That(result, Is.True);

            var updatedMessage = await _context.SystemInboxMessages.FindAsync(_testMessageId1);

            Assert.That(updatedMessage, Is.Not.Null);
            Assert.That(updatedMessage.IsRead, Is.True);
            Assert.That(updatedMessage.Description, Is.EqualTo("Updated welcome message"));
        }

        [Test]
        public async Task UpdateAsync_ReturnsFalse_WhenMessageNotFound()
        {
            var nonExistentMessage = new SystemInboxMessage
            {
                Id = Guid.NewGuid(),
                Description = "Non-existent",
                Type = InboxMessageType.SystemMessage
            };

            var result = await _repository.UpdateAsync(nonExistentMessage);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateAsync_UpdatesReadStatusOnly()
        {
            var message = await _repository.GetByIdAsync(_testMessageId3);

            Assert.That(message, Is.Not.Null);

            var originalDescription = message.Description;

            message.IsRead = true;

            var result = await _repository.UpdateAsync(message);

            Assert.That(result, Is.True);

            var updatedMessage = await _context.SystemInboxMessages.FindAsync(_testMessageId3);

            Assert.That(updatedMessage, Is.Not.Null);
            Assert.That(updatedMessage.IsRead, Is.True);
            Assert.That(updatedMessage.Description, Is.EqualTo(originalDescription));
        }

        #endregion UpdateAsync Tests

        #region DeleteAsync Tests

        [Test]
        public void DeleteAsync_ThrowsInvalidOperationException_WhenEntityHasNoIsDeletedProperty()
        {
            var message = new SystemInboxMessage { Id = _testMessageId1 };

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _repository.DeleteAsync(message));
        }

        #endregion DeleteAsync Tests

        #region HardDeleteAsync Tests

        [Test]
        public async Task HardDeleteAsync_RemovesMessagePermanently_WhenExists()
        {
            var message = await _repository.GetByIdAsync(_testMessageId1);
            Assert.That(message, Is.Not.Null);

            var result = await _repository.HardDeleteAsync(message);

            Assert.That(result, Is.True);

            var deletedMessage = await _context.SystemInboxMessages.FindAsync(_testMessageId1);

            Assert.That(deletedMessage, Is.Null);
        }

        [Test]
        public async Task HardDeleteAsync_RemovesOnlySpecifiedMessage()
        {
            var messageToDelete = await _repository.GetByIdAsync(_testMessageId1);

            Assert.That(messageToDelete, Is.Not.Null);

            var result = await _repository.HardDeleteAsync(messageToDelete);

            Assert.That(result, Is.True);

            var remainingMessages = await _context.SystemInboxMessages.ToListAsync();

            Assert.That(remainingMessages.Count, Is.EqualTo(3));
            Assert.That(remainingMessages.Any(m => m.Id == _testMessageId1), Is.False);
            Assert.That(remainingMessages.Any(m => m.Id == _testMessageId2), Is.True);
            Assert.That(remainingMessages.Any(m => m.Id == _testMessageId3), Is.True);
        }

        #endregion HardDeleteAsync Tests

        #region CountAsync Tests

        [Test]
        public async Task CountAsync_ReturnsCorrectCount()
        {
            var result = await _repository.CountAsync();

            Assert.That(result, Is.EqualTo(4));
        }

        [Test]
        public async Task CountAsync_ReturnsZero_WhenNoMessages()
        {
            _context.SystemInboxMessages.RemoveRange(_context.SystemInboxMessages);
            await _context.SaveChangesAsync();

            var result = await _repository.CountAsync();

            Assert.That(result, Is.EqualTo(0));
        }

        #endregion CountAsync Tests

        #region SaveChangesAsync Tests

        [Test]
        public async Task SaveChangesAsync_SavesPendingChanges()
        {
            var newMessage = new SystemInboxMessage
            {
                Id = Guid.NewGuid(),
                SenderId = _testSenderId,
                ReceiverId = _testReceiverId,
                Type = InboxMessageType.SystemMessage,
                Description = "Pending message",
                CreatedOn = DateTime.UtcNow
            };
            await _context.SystemInboxMessages.AddAsync(newMessage);

            await _repository.SaveChangesAsync();

            var savedMessage = await _context.SystemInboxMessages.FindAsync(newMessage.Id);
            Assert.That(savedMessage, Is.Not.Null);
        }

        #endregion SaveChangesAsync Tests

        #region Where Clause Tests

        [Test]
        public async Task GetAllAttached_CanFilterByReadStatus()
        {
            var unreadMessages = await _repository.GetAllAttached()
                .Where(m => !m.IsRead)
                .ToListAsync();

            Assert.That(unreadMessages.Count, Is.EqualTo(3));
            Assert.That(unreadMessages.All(m => !m.IsRead), Is.True);
        }

        [Test]
        public async Task GetAllAttached_CanFilterByReceiver()
        {
            var receiverMessages = await _repository.GetAllAttached()
                .Where(m => m.ReceiverId == _testReceiverId)
                .ToListAsync();

            Assert.That(receiverMessages.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task GetAllAttached_CanFilterBySender()
        {
            var senderMessages = await _repository.GetAllAttached()
                .Where(m => m.SenderId == _testSenderId)
                .ToListAsync();

            Assert.That(senderMessages.Count, Is.EqualTo(3));
        }

        #endregion Where Clause Tests

        #region Edge Cases and Validation Tests

        [Test]
        public async Task AddAsync_AddsMessageWithEmptyDescription()
        {
            var newMessage = new SystemInboxMessage
            {
                Id = Guid.NewGuid(),
                SenderId = _testSenderId,
                ReceiverId = _testReceiverId,
                Type = InboxMessageType.SystemMessage,
                IsRead = false,
                CreatedOn = DateTime.UtcNow,
                Description = ""
            };

            await _repository.AddAsync(newMessage);

            var savedMessage = await _context.SystemInboxMessages.FindAsync(newMessage.Id);

            Assert.That(savedMessage, Is.Not.Null);
            Assert.That(savedMessage.Description, Is.EqualTo(""));
        }

        [Test]
        public async Task UpdateAsync_CanMarkMessageAsRead()
        {
            var message = await _repository.GetByIdAsync(_testMessageId1);

            Assert.That(message, Is.Not.Null);
            Assert.That(message.IsRead, Is.False);

            message.IsRead = true;

            var result = await _repository.UpdateAsync(message);

            Assert.That(result, Is.True);

            var updatedMessage = await _context.SystemInboxMessages.FindAsync(_testMessageId1);

            Assert.That(updatedMessage, Is.Not.Null);
            Assert.That(updatedMessage.IsRead, Is.True);
        }

        #endregion Edge Cases and Validation Tests
    }
}