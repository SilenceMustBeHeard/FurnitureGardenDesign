using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Implementations.Message;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Tests.Integration.Repositories.Message
{
    [TestFixture]
    public class ContactMessageRepositoryTests
    {
        private ApplicationDbContext _context;
        private ContactMessageRepository _repository;
        private Guid _testMessageId1;
        private Guid _testMessageId2;
        private Guid _testMessageId3;
        private string _testSenderId;
        private string _testReceiverId;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new ContactMessageRepository(_context);
            _testMessageId1 = Guid.NewGuid();
            _testMessageId2 = Guid.NewGuid();
            _testMessageId3 = Guid.NewGuid();
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

            var messages = new[]
            {
                new ContactMessage
                {
                    Id = _testMessageId1,
                    Subject = "Order Question",
                    Message = "I have a question about my order #12345",
                    SenderId = _testSenderId,
                    ReceiverId = _testReceiverId,
                    IsRead = false,
                    IsReadByAdmin = false,
                    CreatedOn = DateTime.UtcNow,
                    Response = null,
                    RespondedAt = null,
                    RespondedById = null
                },
                new ContactMessage
                {
                    Id = _testMessageId2,
                    Subject = "Product Inquiry",
                    Message = "Is the gaming desk available in black?",
                    SenderId = _testSenderId,
                    ReceiverId = _testReceiverId,
                    IsRead = true,
                    IsReadByAdmin = false,
                    CreatedOn = DateTime.UtcNow.AddDays(-1),
                    Response = null,
                    RespondedAt = null,
                    RespondedById = null
                },
                new ContactMessage
                {
                    Id = _testMessageId3,
                    Subject = "Support Request",
                    Message = "Need help with my account",
                    SenderId = _testSenderId,
                    ReceiverId = _testReceiverId,
                    IsRead = false,
                    IsReadByAdmin = true,
                    CreatedOn = DateTime.UtcNow.AddDays(-2),
                    Response = "We'll help you with your account. Please provide more details.",
                    RespondedAt = DateTime.UtcNow.AddDays(-1),
                    RespondedById = _testReceiverId
                },
                new ContactMessage
                {
                    Id = Guid.NewGuid(),
                    Subject = "Feedback",
                    Message = "Great website!",
                    SenderId = _testSenderId,
                    ReceiverId = _testReceiverId,
                    IsRead = false,
                    IsReadByAdmin = false,
                    CreatedOn = DateTime.UtcNow.AddDays(-3),
                    Response = null,
                    RespondedAt = null,
                    RespondedById = null
                }
            };

            _context.Users.AddRange(sender, receiver);
            _context.ContactMessages.AddRange(messages);
            _context.SaveChanges();
        }

        #region AddAsync Tests

        [Test]
        public async Task AddAsync_AddsContactMessageSuccessfully()
        {
            var newMessage = new ContactMessage
            {
                Id = Guid.NewGuid(),
                Subject = "New Inquiry",
                Message = "This is a test message",
                SenderId = _testSenderId,
                ReceiverId = _testReceiverId,
                IsRead = false,
                IsReadByAdmin = false,
                CreatedOn = DateTime.UtcNow
            };

            await _repository.AddAsync(newMessage);

            var savedMessage = await _context.ContactMessages.FindAsync(newMessage.Id);

            Assert.That(savedMessage, Is.Not.Null);
            Assert.That(savedMessage.Subject, Is.EqualTo("New Inquiry"));
            Assert.That(savedMessage.Message, Is.EqualTo("This is a test message"));
        }

        #endregion AddAsync Tests

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_ReturnsMessage_WhenExists()
        {
            var result = await _repository.GetByIdAsync(_testMessageId1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testMessageId1));
            Assert.That(result.Subject, Is.EqualTo("Order Question"));
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

            // Assert
            Assert.That(result.Count, Is.EqualTo(4));
        }

        [Test]
        public async Task GetAllAttached_ReturnsEmptyList_WhenNoMessages()
        {
            _context.ContactMessages.RemoveRange(_context.ContactMessages);
            await _context.SaveChangesAsync();

            var result = _repository.GetAllAttached().ToList();

            Assert.That(result, Is.Empty);
        }

        #endregion GetAllAttached Tests

        #region UpdateAsync Tests

        [Test]
        public async Task UpdateAsync_UpdatesMessageSuccessfully()
        {
            var message = await _repository.GetByIdAsync(_testMessageId1);

            Assert.That(message, Is.Not.Null);

            message.Subject = "Updated Subject";
            message.Message = "Updated message content";
            message.IsRead = true;
            message.IsReadByAdmin = true;

            var result = await _repository.UpdateAsync(message);

            Assert.That(result, Is.True);
            var updatedMessage = await _context.ContactMessages.FindAsync(_testMessageId1);

            Assert.That(updatedMessage, Is.Not.Null);
            Assert.That(updatedMessage.Subject, Is.EqualTo("Updated Subject"));
            Assert.That(updatedMessage.Message, Is.EqualTo("Updated message content"));
            Assert.That(updatedMessage.IsRead, Is.True);
            Assert.That(updatedMessage.IsReadByAdmin, Is.True);
        }

        [Test]
        public async Task UpdateAsync_ReturnsFalse_WhenMessageNotFound()
        {
            var nonExistentMessage = new ContactMessage
            {
                Id = Guid.NewGuid(),
                Subject = "Non-existent",
                Message = "This message doesn't exist"
            };

            var result = await _repository.UpdateAsync(nonExistentMessage);

            Assert.That(result, Is.False);
        }

        #endregion UpdateAsync Tests

        #region DeleteAsync Tests

        [Test]
        public void DeleteAsync_ThrowsInvalidOperationException_WhenEntityHasNoIsDeletedProperty()
        {
            var message = new ContactMessage { Id = _testMessageId1 };

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

            var deletedMessage = await _context.ContactMessages.FindAsync(_testMessageId1);

            Assert.That(deletedMessage, Is.Null);
        }

        [Test]
        public async Task HardDeleteAsync_RemovesOnlySpecifiedMessage()
        {
            var messageToDelete = await _repository.GetByIdAsync(_testMessageId1);
            Assert.That(messageToDelete, Is.Not.Null);

            var result = await _repository.HardDeleteAsync(messageToDelete);

            Assert.That(result, Is.True);

            var remainingMessages = await _context.ContactMessages.ToListAsync();

            Assert.That(remainingMessages.Count, Is.EqualTo(3));
            Assert.That(remainingMessages.Any(m => m.Id == _testMessageId1), Is.False);
            Assert.That(remainingMessages.Any(m => m.Id == _testMessageId2), Is.True);
            Assert.That(remainingMessages.Any(m => m.Id == _testMessageId3), Is.True);
        }

        #endregion HardDeleteAsync Tests

        #region FirstOrDefaultAsync Tests

        [Test]
        public async Task FirstOrDefaultAsync_ReturnsMessage_WhenConditionMatches()
        {
            var result = await _repository.FirstOrDefaultAsync(m => m.Subject == "Order Question");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testMessageId1));
        }

        [Test]
        public async Task FirstOrDefaultAsync_ReturnsNull_WhenConditionDoesNotMatch()
        {
            var result = await _repository.FirstOrDefaultAsync(m => m.Subject == "Non-existent Subject");

            Assert.That(result, Is.Null);
        }

        #endregion FirstOrDefaultAsync Tests

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
            _context.ContactMessages.RemoveRange(_context.ContactMessages);
            await _context.SaveChangesAsync();

            var result = await _repository.CountAsync();

            Assert.That(result, Is.EqualTo(0));
        }

        #endregion CountAsync Tests

        #region SaveChangesAsync Tests

        [Test]
        public async Task SaveChangesAsync_SavesPendingChanges()
        {
            var newMessage = new ContactMessage
            {
                Id = Guid.NewGuid(),
                Subject = "New Message",
                Message = "Content",
                SenderId = _testSenderId,
                ReceiverId = _testReceiverId,
                CreatedOn = DateTime.UtcNow
            };
            await _context.ContactMessages.AddAsync(newMessage);

            await _repository.SaveChangesAsync();

            var savedMessage = await _context.ContactMessages.FindAsync(newMessage.Id);
            Assert.That(savedMessage, Is.Not.Null);
        }

        #endregion SaveChangesAsync Tests
    }
}