using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Implementations;
using FurnitureGardenDesign.Web.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Services.Tests.Messages
{
    [TestFixture]
    public class ContactMessageClientServiceTests
    {
        private Mock<IContactMessageRepository> _messageRepositoryMock;
        private Mock<UserManager<AppUser>> _userManagerMock;
        private ContactMessageClientService _contactMessageClientService;

        private string _testUserId;
        private string _testAdminId;
        private string _testOtherUserId;
        private Guid _testMessageId;
        private Guid _testMessageId2;
        private ContactMessage _testMessage;
        private ContactMessage _testMessageWithResponse;
        private ContactMessage _testMessageRead;
        private ContactMessage _testMessageOtherUser;
        private List<ContactMessage> _testMessages;
        private List<AppUser> _testUsers;

        [SetUp]
        public void SetUp()
        {
            _messageRepositoryMock = new Mock<IContactMessageRepository>(MockBehavior.Strict);

            var store = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _contactMessageClientService = new ContactMessageClientService(
                _messageRepositoryMock.Object,
                _userManagerMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _testUserId = "test-user-123";
            _testAdminId = "admin-456";
            _testOtherUserId = "other-user-789";
            _testMessageId = Guid.NewGuid();
            _testMessageId2 = Guid.NewGuid();

            _testMessage = new ContactMessage
            {
                Id = _testMessageId,
                SenderId = _testUserId,
                ReceiverId = _testAdminId,
                Subject = "Test Subject",
                Message = "Test Message with sufficient length",
                Type = InboxMessageType.ContactMessage,
                CreatedOn = DateTime.UtcNow.AddDays(-2),
                IsRead = false,
                IsReadByAdmin = false,
                Response = null,
                RespondedAt = null
            };

            _testMessageWithResponse = new ContactMessage
            {
                Id = _testMessageId2,
                SenderId = _testUserId,
                ReceiverId = _testAdminId,
                Subject = "Response Subject",
                Message = "Original Message",
                Type = InboxMessageType.ContactMessage,
                CreatedOn = DateTime.UtcNow.AddDays(-1),
                IsRead = false,
                IsReadByAdmin = true,
                Response = "This is the admin response",
                RespondedAt = DateTime.UtcNow.AddDays(-1),
                RespondedById = _testAdminId
            };

            _testMessageRead = new ContactMessage
            {
                Id = Guid.NewGuid(),
                SenderId = _testUserId,
                ReceiverId = _testAdminId,
                Subject = "Read Subject",
                Message = "Read Message",
                Type = InboxMessageType.ContactMessage,
                CreatedOn = DateTime.UtcNow.AddHours(-1),
                IsRead = true,
                IsReadByAdmin = true,
                Response = "Response already read",
                RespondedAt = DateTime.UtcNow.AddHours(-1)
            };

            _testMessageOtherUser = new ContactMessage
            {
                Id = Guid.NewGuid(),
                SenderId = _testOtherUserId,
                ReceiverId = _testAdminId,
                Subject = "Other User Subject",
                Message = "Other User Message",
                Type = InboxMessageType.ContactMessage,
                CreatedOn = DateTime.UtcNow.AddDays(-3),
                IsRead = false,
                IsReadByAdmin = false,
                Response = null
            };

            _testMessages = new List<ContactMessage>
            {
                _testMessage,
                _testMessageWithResponse,
                _testMessageRead,
                _testMessageOtherUser
            };

            _testUsers = new List<AppUser>
            {
                new AppUser { Id = _testAdminId, UserName = "admin@test.com", FirstName = "Admin", LastName = "User", Email = "admin@test.com" },
                new AppUser { Id = _testUserId, UserName = "user@test.com", FirstName = "Test", LastName = "User", Email = "user@test.com" },
                new AppUser { Id = _testOtherUserId, UserName = "other@test.com", FirstName = "Other", LastName = "User", Email = "other@test.com" }
            };
        }

        private ClaimsPrincipal GetTestUserPrincipal(string userId)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims);
            return new ClaimsPrincipal(identity);
        }

        private void SetupUserManagerForSendMessage(string userId, bool hasAdmin = true)
        {
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(_testUsers.FirstOrDefault(u => u.Id == userId));

            var adminUsers = hasAdmin
                ? _testUsers.Where(u => u.Id == _testAdminId).ToList()
                : new List<AppUser>();

            _userManagerMock.Setup(x => x.GetUsersInRoleAsync("Admin"))
                .ReturnsAsync(adminUsers);
        }

        #region SendContactMessageAsync Tests

        [Test]
        public async Task SendContactMessageAsync_ValidMessage_CreatesContactMessage()
        {
            
            var model = new ContactMessageCreateViewModel
            {
                Subject = "New Test Subject",
                Message = "New Test Message with sufficient length"
            };
            var userPrincipal = GetTestUserPrincipal(_testUserId);

            SetupUserManagerForSendMessage(_testUserId);

            var emptyList = new List<ContactMessage>();
            var mockDbSet = emptyList.BuildMockDbSet();
            _messageRepositoryMock.Setup(x => x.GetAllAttached())
                .Returns(mockDbSet.Object);

            
            _messageRepositoryMock.Setup(x => x.AddAsync(It.IsAny<ContactMessage>()))
                .Returns(Task.CompletedTask);

            _messageRepositoryMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.FromResult(1));

         
            await _contactMessageClientService.SendContactMessageAsync(model, userPrincipal);

         
            _messageRepositoryMock.Verify(x => x.AddAsync(It.Is<ContactMessage>(cm =>
                cm.SenderId == _testUserId &&
                cm.ReceiverId == _testAdminId &&
                cm.Subject == model.Subject &&
                cm.Message == model.Message &&
                cm.Type == InboxMessageType.ContactMessage &&
                cm.IsRead == false &&
                cm.IsReadByAdmin == false)), Times.Once);

            _messageRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task SendContactMessageAsync_DuplicateMessage_DoesNotCreateDuplicate()
        {
           
            var model = new ContactMessageCreateViewModel
            {
                Subject = _testMessage.Subject,
                Message = _testMessage.Message
            };
            var userPrincipal = GetTestUserPrincipal(_testUserId);

            SetupUserManagerForSendMessage(_testUserId);

            var mockDbSet = _testMessages.BuildMockDbSet();
            _messageRepositoryMock.Setup(x => x.GetAllAttached())
                .Returns(mockDbSet.Object);

           
            await _contactMessageClientService.SendContactMessageAsync(model, userPrincipal);

            _messageRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ContactMessage>()), Times.Never);
            _messageRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public void SendContactMessageAsync_UserNotLoggedIn_ThrowsArgumentException()
        {
            
            var model = new ContactMessageCreateViewModel
            {
                Subject = "Test Subject",
                Message = "Test Message with sufficient length"
            };
            var userPrincipal = GetTestUserPrincipal(_testUserId);

            _userManagerMock.Setup(x => x.GetUserAsync(userPrincipal))
                .ReturnsAsync((AppUser)null);

            var ex = Assert.ThrowsAsync<ArgumentException>(
                () => _contactMessageClientService.SendContactMessageAsync(model, userPrincipal));
            Assert.That(ex.Message, Is.EqualTo("You must be logged in to send a contact message."));
        }

        [Test]
        public void SendContactMessageAsync_NoAdminFound_ThrowsInvalidOperationException()
        {
          
            var model = new ContactMessageCreateViewModel
            {
                Subject = "Test Subject",
                Message = "Test Message with sufficient length"
            };
            var userPrincipal = GetTestUserPrincipal(_testUserId);

            SetupUserManagerForSendMessage(_testUserId, hasAdmin: false);

           
            var ex = Assert.ThrowsAsync<InvalidOperationException>(
                () => _contactMessageClientService.SendContactMessageAsync(model, userPrincipal));
            Assert.That(ex.Message, Is.EqualTo("No admin user found in the system."));
        }

        #endregion

        #region GetUserMessagesAsync Tests



      
        [Test]
        public async Task GetUserMessagesAsync_ReturnsEmptyList_WhenUserHasNoMessages()
        {
           
            var emptyList = new List<ContactMessage>();
            var mockDbSet = emptyList.BuildMockDbSet();
            _messageRepositoryMock.Setup(x => x.GetAllAttached())
                .Returns(mockDbSet.Object);

          
            var result = await _contactMessageClientService.GetUserMessagesAsync(_testUserId);

          
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

     

        #endregion

        #region GetMessageDetailsAsync Tests

     
     

      

        [Test]
        public async Task GetMessageDetailsAsync_MessageNotFound_ReturnsNull()
        {
           
            var emptyList = new List<ContactMessage>();
            var mockDbSet = emptyList.BuildMockDbSet();
            _messageRepositoryMock.Setup(x => x.GetAllAttached())
                .Returns(mockDbSet.Object);

          
            var result = await _contactMessageClientService.GetMessageDetailsAsync(Guid.NewGuid(), _testUserId);

         
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetMessageDetailsAsync_UserNotSender_ReturnsNull()
        {
         
            var mockDbSet = _testMessages.BuildMockDbSet();
            _messageRepositoryMock.Setup(x => x.GetAllAttached())
                .Returns(mockDbSet.Object);

            var result = await _contactMessageClientService.GetMessageDetailsAsync(_testMessageWithResponse.Id, _testOtherUserId);

        
            Assert.That(result, Is.Null);
        }

        #endregion

        #region GetUserUnreadResponsesCountAsync Tests

        [Test]
        public async Task GetUserUnreadResponsesCountAsync_ReturnsCorrectCount()
        {
           
            var mockDbSet = _testMessages.BuildMockDbSet();
            _messageRepositoryMock.Setup(x => x.GetAllAttached())
                .Returns(mockDbSet.Object);

          
            var result = await _contactMessageClientService.GetUserUnreadResponsesCountAsync(_testUserId);

           
            Assert.That(result, Is.EqualTo(1)); // Only _testMessageWithResponse is unread with response
        }

        [Test]
        public async Task GetUserUnreadResponsesCountAsync_ReturnsZero_WhenNoUnreadResponses()
        {
           
            var messages = new List<ContactMessage>
            {
                _testMessageRead, 
                new ContactMessage
                {
                    Id = Guid.NewGuid(),
                    SenderId = _testUserId,
                    Response = null, 
                    IsRead = false
                }
            };
            var mockDbSet = messages.BuildMockDbSet();
            _messageRepositoryMock.Setup(x => x.GetAllAttached())
                .Returns(mockDbSet.Object);

          
            var result = await _contactMessageClientService.GetUserUnreadResponsesCountAsync(_testUserId);

          
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetUserUnreadResponsesCountAsync_ReturnsZero_WhenUserHasNoMessages()
        {
           
            var emptyList = new List<ContactMessage>();
            var mockDbSet = emptyList.BuildMockDbSet();
            _messageRepositoryMock.Setup(x => x.GetAllAttached())
                .Returns(mockDbSet.Object);

         
            var result = await _contactMessageClientService.GetUserUnreadResponsesCountAsync(_testUserId);

         
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetUserUnreadResponsesCountAsync_OnlyCountsCurrentUserMessages()
        {
          
            var mockDbSet = _testMessages.BuildMockDbSet();
            _messageRepositoryMock.Setup(x => x.GetAllAttached())
                .Returns(mockDbSet.Object);

          
            var result = await _contactMessageClientService.GetUserUnreadResponsesCountAsync(_testUserId);

           
            Assert.That(result, Is.EqualTo(1)); 
        }

        #endregion

        #region Constructor Tests

        [Test]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {
            
            var service = new ContactMessageClientService(
                _messageRepositoryMock.Object,
                _userManagerMock.Object);

           
            Assert.That(service, Is.Not.Null);
        }

        #endregion
    }
}