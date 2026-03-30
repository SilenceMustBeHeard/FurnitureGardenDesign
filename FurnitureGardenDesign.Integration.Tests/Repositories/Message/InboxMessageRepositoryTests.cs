using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Implementations.Message;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Tests.Integration.Repositories.Message
{
    [TestFixture]
    public class InboxMessageRepositoryTests
    {
        private ApplicationDbContext _context;
        private InboxMessageRepository _repository;
        private Guid _testMessageId1;
        private Guid _testMessageId2;
        private Guid _testMessageId3;
        private Guid _testDesignVariantId;
        private string _testSenderId;
        private string _testReceiverId;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new InboxMessageRepository(_context);
            _testMessageId1 = Guid.NewGuid();
            _testMessageId2 = Guid.NewGuid();
            _testMessageId3 = Guid.NewGuid();
            _testDesignVariantId = Guid.NewGuid();
            _testSenderId = "11111111-1111-1111-1111-111111111111";
            _testReceiverId = "22222222-2222-2222-2222-222222222222";

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
                FirstName = "John",
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

            var designVariant = new DesignVariant
            {
                Id = _testDesignVariantId,
                OrderId = Guid.NewGuid(),
                Image2DUrl = "/images/variant.jpg",
                Notes = "Test variant",
                IsApproved = false,
                CreatedOn = DateTime.UtcNow
            };

            var messages = new[]
            {
                new InboxMessage
                {
                    Id = _testMessageId1,
                    DesignVariantId = _testDesignVariantId,
                    ReceiverId = _testReceiverId,
                    Type = InboxMessageType.DesignSent,
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow,
                    Notes = "Design proposal sent"
                },
                new InboxMessage
                {
                    Id = _testMessageId2,
                    DesignVariantId = _testDesignVariantId,
                    ReceiverId = _testReceiverId,
                    Type = InboxMessageType.DesignApproved,
                    IsRead = true,
                    CreatedOn = DateTime.UtcNow.AddDays(-1),
                    Notes = "Design approved by customer"
                },
                new InboxMessage
                {
                    Id = _testMessageId3,
                    DesignVariantId = _testDesignVariantId,
                    ReceiverId = _testReceiverId,
                    Type = InboxMessageType.DesignSent,
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow.AddDays(-2),
                    Notes = "Customer requested changes"
                },
                new InboxMessage
                {
                    Id = Guid.NewGuid(),
                    DesignVariantId = _testDesignVariantId,
                    ReceiverId = _testReceiverId,
                    Type = InboxMessageType.DesignSent,
                    IsRead = true,
                    CreatedOn = DateTime.UtcNow.AddDays(-3),
                    Notes = "Follow-up design proposal"
                }
            };

            _context.Users.AddRange(sender, receiver);
            _context.DesignVariants.Add(designVariant);
            _context.InboxMessages.AddRange(messages);
            _context.SaveChanges();
        }

        #region AddAsync Tests

        [Test]
        public async Task AddAsync_AddsInboxMessageSuccessfully()
        {
            var newMessage = new InboxMessage
            {
                Id = Guid.NewGuid(),
                DesignVariantId = _testDesignVariantId,
                ReceiverId = _testReceiverId,
                Type = InboxMessageType.DesignSent,
                IsRead = false,
                CreatedOn = DateTime.UtcNow,
                Notes = "New design proposal"
            };

            await _repository.AddAsync(newMessage);

            var savedMessage = await _context.InboxMessages.FindAsync(newMessage.Id);
            Assert.That(savedMessage, Is.Not.Null);
            Assert.That(savedMessage.Type, Is.EqualTo(InboxMessageType.DesignSent));
            Assert.That(savedMessage.Notes, Is.EqualTo("New design proposal"));
            Assert.That(savedMessage.IsRead, Is.False);
        }

        #endregion AddAsync Tests

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_ReturnsMessage_WhenExists()
        {
            var result = await _repository.GetByIdAsync(_testMessageId1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testMessageId1));
            Assert.That(result.Type, Is.EqualTo(InboxMessageType.DesignSent));
            Assert.That(result.Notes, Is.EqualTo("Design proposal sent"));
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
            Assert.That(result.Select(m => m.Type), Contains.Item(InboxMessageType.DesignSent));
            Assert.That(result.Select(m => m.Type), Contains.Item(InboxMessageType.DesignApproved));
            Assert.That(result.Select(m => m.Type), Contains.Item(InboxMessageType.DesignSent));
        }

        [Test]
        public async Task GetAllAttached_ReturnsEmptyList_WhenNoMessages()
        {
            _context.InboxMessages.RemoveRange(_context.InboxMessages);
            await _context.SaveChangesAsync();

            var result = _repository.GetAllAttached().ToList();

            Assert.That(result, Is.Empty);
        }

        #endregion GetAllAttached Tests

        #region FirstOrDefaultAsync Tests

        [Test]
        public async Task FirstOrDefaultAsync_ReturnsMessage_WhenConditionMatches()
        {
            var result = await _repository
                .FirstOrDefaultAsync(m => m.Type == InboxMessageType
                .DesignSent && !m.IsRead);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testMessageId1));
            Assert.That(result.IsRead, Is.False);
        }

        [Test]
        public async Task FirstOrDefaultAsync_ReturnsNull_WhenConditionDoesNotMatch()
        {
            var result = await _repository
                .FirstOrDefaultAsync(m => m.Type == InboxMessageType
                .DesignSent && m.Id == Guid.NewGuid());

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
            message.Notes = "Updated notes";

            var result = await _repository.UpdateAsync(message);

            Assert.That(result, Is.True);

            var updatedMessage = await _context.InboxMessages.FindAsync(_testMessageId1);

            Assert.That(updatedMessage, Is.Not.Null);
            Assert.That(updatedMessage.IsRead, Is.True);
            Assert.That(updatedMessage.Notes, Is.EqualTo("Updated notes"));
        }

        [Test]
        public async Task UpdateAsync_ReturnsFalse_WhenMessageNotFound()
        {
            var nonExistentMessage = new InboxMessage
            {
                Id = Guid.NewGuid(),
                DesignVariantId = _testDesignVariantId,
                ReceiverId = _testReceiverId,
                Type = InboxMessageType.DesignSent,
                Notes = "Non-existent"
            };

            var result = await _repository.UpdateAsync(nonExistentMessage);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateAsync_UpdatesReadStatusOnly()
        {
            var message = await _repository.GetByIdAsync(_testMessageId2);

            Assert.That(message, Is.Not.Null);

            var originalNotes = message.Notes;
            var originalType = message.Type;

            message.IsRead = false;

            var result = await _repository.UpdateAsync(message);

            Assert.That(result, Is.True);
            var updatedMessage = await _context.InboxMessages.FindAsync(_testMessageId2);

            Assert.That(updatedMessage, Is.Not.Null);
            Assert.That(updatedMessage.IsRead, Is.False);
            Assert.That(updatedMessage.Notes, Is.EqualTo(originalNotes));
            Assert.That(updatedMessage.Type, Is.EqualTo(originalType));
        }

        #endregion UpdateAsync Tests

        #region DeleteAsync Tests

        [Test]
        public void DeleteAsync_ThrowsInvalidOperationException_WhenEntityHasNoIsDeletedProperty()
        {
            var message = new InboxMessage { Id = _testMessageId1 };

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
            var deletedMessage = await _context.InboxMessages.FindAsync(_testMessageId1);
            Assert.That(deletedMessage, Is.Null);
        }

        [Test]
        public async Task HardDeleteAsync_RemovesOnlySpecifiedMessage()
        {
            var messageToDelete = await _repository.GetByIdAsync(_testMessageId1);
            Assert.That(messageToDelete, Is.Not.Null);

            var result = await _repository.HardDeleteAsync(messageToDelete);

            Assert.That(result, Is.True);

            var remainingMessages = await _context.InboxMessages.ToListAsync();

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
            _context.InboxMessages.RemoveRange(_context.InboxMessages);
            await _context.SaveChangesAsync();

            var result = await _repository.CountAsync();

            Assert.That(result, Is.EqualTo(0));
        }

        #endregion CountAsync Tests

        #region SaveChangesAsync Tests

        [Test]
        public async Task SaveChangesAsync_SavesPendingChanges()
        {
            var newMessage = new InboxMessage
            {
                Id = Guid.NewGuid(),
                DesignVariantId = _testDesignVariantId,
                ReceiverId = _testReceiverId,
                Type = InboxMessageType.DesignSent,
                CreatedOn = DateTime.UtcNow
            };
            await _context.InboxMessages.AddAsync(newMessage);

            await _repository.SaveChangesAsync();

            var savedMessage = await _context.InboxMessages.FindAsync(newMessage.Id);
            Assert.That(savedMessage, Is.Not.Null);
        }

        #endregion SaveChangesAsync Tests

        #region Where Clause Tests

        [Test]
        public async Task GetAllAttached_CanFilterByReadStatus()
        {
            var unreadMessages = _repository.GetAllAttached()
                .Where(m => !m.IsRead)
                .ToList();

            Assert.That(unreadMessages.Count, Is.EqualTo(2));
            Assert.That(unreadMessages.All(m => !m.IsRead), Is.True);
        }

        [Test]
        public async Task GetAllAttached_CanFilterByReceiver()
        {
            var receiverMessages = _repository.GetAllAttached()
                .Where(m => m.ReceiverId == _testReceiverId)
                .ToList();

            Assert.That(receiverMessages.Count, Is.EqualTo(4));
        }

        #endregion Where Clause Tests

        #region Edge Cases and Validation Tests

        [Test]
        public async Task AddAsync_AddsMessageWithNullNotes()
        {
            var newMessage = new InboxMessage
            {
                Id = Guid.NewGuid(),
                DesignVariantId = _testDesignVariantId,
                ReceiverId = _testReceiverId,
                Type = InboxMessageType.DesignSent,
                IsRead = false,
                CreatedOn = DateTime.UtcNow,
                Notes = null
            };

            await _repository.AddAsync(newMessage);

            var savedMessage = await _context.InboxMessages.FindAsync(newMessage.Id);

            Assert.That(savedMessage, Is.Not.Null);
            Assert.That(savedMessage.Notes, Is.Null);
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
            var updatedMessage = await _context.InboxMessages.FindAsync(_testMessageId1);

            Assert.That(updatedMessage, Is.Not.Null);
            Assert.That(updatedMessage.IsRead, Is.True);
        }

        #endregion Edge Cases and Validation Tests
    }
}