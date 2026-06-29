using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces.Account;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Services.Core.Implementations.Account;
using FurnitureGardenDesign.Services.Core.Interfaces.Message;
using FurnitureGardenDesign.Web.ViewModels.Messages;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;

namespace FurnitureGardenDesign.Unit.Tests.Services.User.Account
{
    [TestFixture]
    public class ProfileServiceTests
    {
        private Mock<IAppUserRepository> _userRepositoryMock;
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<IInboxMessageService> _inboxMessageServiceMock;
        private Mock<ISystemInboxMessageService> _systemInboxMessageServiceMock;
        private Mock<IContactMessageClientService> _contactMessageClientServiceMock;
        private Mock<IContactMessageService> _contactMessageServiceMock;
        private ProfileService _profileService;

        private AppUser _testUser;
        private string _testUserId;
        private List<ContactMessageDetailsViewModel> _testContactMessages;
        private List<InboxMessageViewModel> _testInboxMessages;
        private List<SystemInboxMessageViewModel> _testSystemMessages;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IAppUserRepository>(MockBehavior.Strict);

            var store = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _inboxMessageServiceMock = new Mock<IInboxMessageService>(MockBehavior.Strict);
            _systemInboxMessageServiceMock = new Mock<ISystemInboxMessageService>(MockBehavior.Strict);
            _contactMessageClientServiceMock = new Mock<IContactMessageClientService>(MockBehavior.Strict);
            _contactMessageServiceMock = new Mock<IContactMessageService>(MockBehavior.Strict);

            _profileService = new ProfileService(
                _userRepositoryMock.Object,
                _userManagerMock.Object,
                _inboxMessageServiceMock.Object,
                _systemInboxMessageServiceMock.Object,
                _contactMessageClientServiceMock.Object,
                _contactMessageServiceMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _testUserId = "test-user-123";

            _testUser = new AppUser
            {
                Id = _testUserId,
                Email = "test@example.com",
                UserName = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Test Street"
            };

            _testInboxMessages = new List<InboxMessageViewModel>
            {
                new InboxMessageViewModel
                {
                    Id = Guid.NewGuid(),
                    DesignVariantId = Guid.NewGuid(),
                    DesignImage2DUrl = "design1.jpg",
                    Notes = "Test note 1",
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow.AddDays(-1),
                    Type = Data.Common.Enums.InboxMessageType.DesignSent
                },
                new InboxMessageViewModel
                {
                    Id = Guid.NewGuid(),
                    DesignVariantId = Guid.NewGuid(),
                    DesignImage2DUrl = "design2.jpg",
                    Notes = "Test note 2",
                    IsRead = true,
                    CreatedOn = DateTime.UtcNow.AddDays(-2),
                    Type = Data.Common.Enums.InboxMessageType.DesignApproved
                }
            };

            _testSystemMessages = new List<SystemInboxMessageViewModel>
            {
                new SystemInboxMessageViewModel
                {
                    Id = Guid.NewGuid(),
                    Description = "System message 1",
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow.AddHours(-5),
                    Type = Data.Common.Enums.InboxMessageType.SystemMessage,
                    SenderId = "system"
                },
                new SystemInboxMessageViewModel
                {
                    Id = Guid.NewGuid(),
                    Description = "System message 2",
                    IsRead = true,
                    CreatedOn = DateTime.UtcNow.AddHours(-10),
                    Type = Data.Common.Enums.InboxMessageType.SystemMessage,
                    SenderId = "system"
                }
            };

            _testContactMessages = new List<ContactMessageDetailsViewModel>
            {
                new ContactMessageDetailsViewModel
                {
                    Id = Guid.NewGuid(),
                    Subject = "Contact Message 1",
                    Message = "Message content 1",
                    SenderName = "Sender 1",
                    SenderEmail = "sender1@example.com",
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow.AddDays(-1)
                },
                new ContactMessageDetailsViewModel
                {
                    Id = Guid.NewGuid(),
                    Subject = "Contact Message 2",
                    Message = "Message content 2",
                    SenderName = "Sender 2",
                    SenderEmail = "sender2@example.com",
                    IsRead = true,
                    CreatedOn = DateTime.UtcNow.AddDays(-2)
                }
            };
        }

        #region GetProfileAsync Tests - Regular User

        [Test]
        public async Task GetProfileAsync_ReturnsProfileViewModel_WhenUserExistsAndIsRegularUser()
        {
            var users = new List<AppUser> { _testUser };

            var mockQueryable = users.BuildMockDbSet();

            _userRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            _userManagerMock.Setup(u => u.GetRolesAsync(_testUser))
                .ReturnsAsync(new List<string> { "User" });

            _inboxMessageServiceMock
                .Setup(s => s.GetUserMessagesAsync(_testUserId))
                .ReturnsAsync(_testInboxMessages);

            _systemInboxMessageServiceMock
                .Setup(s => s.GetUserMessagesAsync(_testUserId))
                .ReturnsAsync(_testSystemMessages);

            _contactMessageClientServiceMock
                .Setup(s => s.GetUserMessagesAsync(_testUserId))
                .ReturnsAsync(_testContactMessages);

            var result = await _profileService.GetProfileAsync(_testUserId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testUser.Id));
            Assert.That(result.Email, Is.EqualTo(_testUser.Email));
            Assert.That(result.FirstName, Is.EqualTo(_testUser.FirstName));
            Assert.That(result.LastName, Is.EqualTo(_testUser.LastName));
            Assert.That(result.Address, Is.EqualTo(_testUser.Address));
            Assert.That(result.Inbox.Count(), Is.EqualTo(2));
            Assert.That(result.SystemInbox.Count(), Is.EqualTo(2));
            Assert.That(result.ContactMessages.Count(), Is.EqualTo(2));

            _userRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
            _userManagerMock.Verify(u => u.GetRolesAsync(_testUser), Times.Once);
            _inboxMessageServiceMock.Verify(s => s.GetUserMessagesAsync(_testUserId), Times.Once);
            _systemInboxMessageServiceMock.Verify(s => s.GetUserMessagesAsync(_testUserId), Times.Once);
            _contactMessageClientServiceMock.Verify(s => s.GetUserMessagesAsync(_testUserId), Times.Once);
            _contactMessageServiceMock.Verify(s => s.GetAdminMessagesAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion GetProfileAsync Tests - Regular User

        #region GetProfileAsync Tests - Admin User

        [Test]
        public async Task GetProfileAsync_ReturnsProfileViewModel_WhenUserIsAdmin()
        {
            var users = new List<AppUser> { _testUser };
            var mockQueryable = users.BuildMockDbSet();
            _userRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            _userManagerMock.Setup(u => u.GetRolesAsync(_testUser))
                .ReturnsAsync(new List<string> { "Admin" });

            _inboxMessageServiceMock
                .Setup(s => s.GetUserMessagesAsync(_testUserId))
                .ReturnsAsync(_testInboxMessages);

            _systemInboxMessageServiceMock
                .Setup(s => s.GetUserMessagesAsync(_testUserId))
                .ReturnsAsync(_testSystemMessages);

            _contactMessageServiceMock
                .Setup(s => s.GetAdminMessagesAsync(_testUserId))
                .ReturnsAsync(_testContactMessages);

            var result = await _profileService.GetProfileAsync(_testUserId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testUser.Id));
            Assert.That(result.ContactMessages.Count(), Is.EqualTo(2));

            _userRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
            _userManagerMock.Verify(u => u.GetRolesAsync(_testUser), Times.Once);
            _inboxMessageServiceMock.Verify(s => s.GetUserMessagesAsync(_testUserId), Times.Once);
            _systemInboxMessageServiceMock.Verify(s => s.GetUserMessagesAsync(_testUserId), Times.Once);
            _contactMessageServiceMock.Verify(s => s.GetAdminMessagesAsync(_testUserId), Times.Once);
            _contactMessageClientServiceMock.Verify(s => s.GetUserMessagesAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion GetProfileAsync Tests - Admin User

        #region GetProfileAsync Tests - Manager User

        [Test]
        public async Task GetProfileAsync_ReturnsProfileViewModel_WhenUserIsManager()
        {
            var users = new List<AppUser> { _testUser };
            var mockQueryable = users.BuildMockDbSet();
            _userRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            _userManagerMock.Setup(u => u.GetRolesAsync(_testUser))
                .ReturnsAsync(new List<string> { "Manager" });

            _inboxMessageServiceMock
                .Setup(s => s.GetUserMessagesAsync(_testUserId))
                .ReturnsAsync(_testInboxMessages);

            _systemInboxMessageServiceMock
                .Setup(s => s.GetUserMessagesAsync(_testUserId))
                .ReturnsAsync(_testSystemMessages);

            var result = await _profileService.GetProfileAsync(_testUserId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testUser.Id));
            Assert.That(result.ContactMessages, Is.Empty);

            _userRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
            _userManagerMock.Verify(u => u.GetRolesAsync(_testUser), Times.Once);
            _inboxMessageServiceMock.Verify(s => s.GetUserMessagesAsync(_testUserId), Times.Once);
            _systemInboxMessageServiceMock.Verify(s => s.GetUserMessagesAsync(_testUserId), Times.Once);
            _contactMessageClientServiceMock.Verify(s => s.GetUserMessagesAsync(It.IsAny<string>()), Times.Never);
            _contactMessageServiceMock.Verify(s => s.GetAdminMessagesAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion GetProfileAsync Tests - Manager User

        #region GetProfileAsync Tests - User Not Found

        [Test]
        public async Task GetProfileAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            var users = new List<AppUser>();
            var mockQueryable = users.BuildMockDbSet();
            _userRepositoryMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable.Object);

            var result = await _profileService.GetProfileAsync("non-existent-user");

            Assert.That(result, Is.Null);

            _userRepositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
            _userManagerMock.Verify(u => u.GetRolesAsync(It.IsAny<AppUser>()), Times.Never);
            _inboxMessageServiceMock.Verify(s => s.GetUserMessagesAsync(It.IsAny<string>()), Times.Never);
            _systemInboxMessageServiceMock.Verify(s => s.GetUserMessagesAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion GetProfileAsync Tests - User Not Found

        #region GetProfileAsync Tests - Empty Messages

        [Test]
        public async Task GetProfileAsync_ReturnsEmptyCollections_WhenNoMessagesExist()
        {
            var users = new List<AppUser> { _testUser };
            var mockQueryable = users.BuildMockDbSet();

            _userRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            _userManagerMock.Setup(u => u.GetRolesAsync(_testUser))
                .ReturnsAsync(new List<string> { "User" });

            _inboxMessageServiceMock
                .Setup(s => s.GetUserMessagesAsync(_testUserId))
                .ReturnsAsync(new List<InboxMessageViewModel>());

            _systemInboxMessageServiceMock
                .Setup(s => s.GetUserMessagesAsync(_testUserId))
                .ReturnsAsync(new List<SystemInboxMessageViewModel>());

            _contactMessageClientServiceMock
                .Setup(s => s.GetUserMessagesAsync(_testUserId))
                .ReturnsAsync(new List<ContactMessageDetailsViewModel>());

            var result = await _profileService.GetProfileAsync(_testUserId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Inbox, Is.Empty);
            Assert.That(result.SystemInbox, Is.Empty);
            Assert.That(result.ContactMessages, Is.Empty);
        }

        #endregion GetProfileAsync Tests - Empty Messages

        #region GetProfileAsync Tests - Null Returns from Services

        [Test]
        public async Task GetProfileAsync_HandlesNullMessagesFromServices()
        {
            var users = new List<AppUser> { _testUser };

            var mockQueryable = users.BuildMockDbSet();

            _userRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            _userManagerMock.Setup(u => u.GetRolesAsync(_testUser))
                .ReturnsAsync(new List<string> { "User" });

            _inboxMessageServiceMock
                .Setup(s => s.GetUserMessagesAsync(_testUserId))
                .ReturnsAsync((List<InboxMessageViewModel>)null!);

            _systemInboxMessageServiceMock
                .Setup(s => s.GetUserMessagesAsync(_testUserId))
                .ReturnsAsync((List<SystemInboxMessageViewModel>)null!);

            _contactMessageClientServiceMock
                .Setup(s => s.GetUserMessagesAsync(_testUserId))
                .ReturnsAsync((List<ContactMessageDetailsViewModel>)null!);

            var result = await _profileService.GetProfileAsync(_testUserId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Inbox, Is.Empty);
            Assert.That(result.SystemInbox, Is.Empty);
            Assert.That(result.ContactMessages, Is.Empty);
        }

        #endregion GetProfileAsync Tests - Null Returns from Services

        #region GetProfileAsync Tests - Edge Cases

        [Test]
        public async Task GetProfileAsync_ReturnsUserWithNoAddress_WhenAddressIsNull()
        {
            var userWithNullAddress = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test2@example.com",
                FirstName = "Jane",
                LastName = "Smith",
                Address = null
            };

            var users = new List<AppUser> { userWithNullAddress };

            var mockQueryable = users.BuildMockDbSet();

            _userRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            _userManagerMock.Setup(u => u.GetRolesAsync(userWithNullAddress))
                .ReturnsAsync(new List<string> { "User" });

            _inboxMessageServiceMock
                .Setup(s => s.GetUserMessagesAsync(userWithNullAddress.Id))
                .ReturnsAsync(new List<InboxMessageViewModel>());

            _systemInboxMessageServiceMock
                .Setup(s => s.GetUserMessagesAsync(userWithNullAddress.Id))
                .ReturnsAsync(new List<SystemInboxMessageViewModel>());

            _contactMessageClientServiceMock
                .Setup(s => s.GetUserMessagesAsync(userWithNullAddress.Id))
                .ReturnsAsync(new List<ContactMessageDetailsViewModel>());

            var result = await _profileService.GetProfileAsync(userWithNullAddress.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Address, Is.Null);
        }

        [Test]
        public async Task GetProfileAsync_ReturnsEmptyEmail_WhenEmailIsNull()
        {
            var userWithNullEmail = new AppUser
            {
                Id = "user3",
                Email = null,
                FirstName = "Bob",
                LastName = "Johnson",
                Address = "456 Test Ave"
            };

            var users = new List<AppUser> { userWithNullEmail };

            var mockQueryable = users.BuildMockDbSet();

            _userRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            _userManagerMock.Setup(u => u.GetRolesAsync(userWithNullEmail))
                .ReturnsAsync(new List<string> { "User" });

            _inboxMessageServiceMock
                .Setup(s => s.GetUserMessagesAsync("user3"))
                .ReturnsAsync(new List<InboxMessageViewModel>());

            _systemInboxMessageServiceMock
                .Setup(s => s.GetUserMessagesAsync("user3"))
                .ReturnsAsync(new List<SystemInboxMessageViewModel>());

            _contactMessageClientServiceMock
                .Setup(s => s.GetUserMessagesAsync("user3"))
                .ReturnsAsync(new List<ContactMessageDetailsViewModel>());

            var result = await _profileService.GetProfileAsync("user3");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task GetProfileAsync_WithEmptyUserId_ReturnsNull()
        {
            var users = new List<AppUser> { _testUser };

            var mockQueryable = users.BuildMockDbSet();

            _userRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            var result = await _profileService.GetProfileAsync(string.Empty);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetProfileAsync_WithNullUserId_ReturnsNull()
        {
            var users = new List<AppUser> { _testUser };

            var mockQueryable = users.BuildMockDbSet();

            _userRepositoryMock.Setup(r => r.GetAllAttached()).Returns(mockQueryable.Object);

            var result = await _profileService.GetProfileAsync(null);

            Assert.That(result, Is.Null);
        }

        #endregion GetProfileAsync Tests - Edge Cases

        #region GetProfileAsync Tests - Multiple Users

        [Test]
        public async Task GetProfileAsync_ReturnsCorrectUser_WhenMultipleUsersExist()
        {
            // Arrange
            var user2 = new AppUser
            {
                Id = "user2-id",
                Email = "user2@example.com",
                FirstName = "User",
                LastName = "Two",
                Address = "456 Another St"
            };

            var users = new List<AppUser> { _testUser, user2 };

            var mockQueryable = users.BuildMockDbSet();

            _userRepositoryMock.Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);

            _userManagerMock.Setup(u => u.GetRolesAsync(user2))
                .ReturnsAsync(new List<string> { "User" });

            _inboxMessageServiceMock
                .Setup(s => s.GetUserMessagesAsync("user2-id"))
                .ReturnsAsync(new List<InboxMessageViewModel>());

            _systemInboxMessageServiceMock
                .Setup(s => s.GetUserMessagesAsync("user2-id"))
                .ReturnsAsync(new List<SystemInboxMessageViewModel>());

            _contactMessageClientServiceMock
                .Setup(s => s.GetUserMessagesAsync("user2-id"))
                .ReturnsAsync(new List<ContactMessageDetailsViewModel>());

            var result = await _profileService.GetProfileAsync("user2-id");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo("user2-id"));
            Assert.That(result.Email, Is.EqualTo("user2@example.com"));
            Assert.That(result.FirstName, Is.EqualTo("User"));
            Assert.That(result.LastName, Is.EqualTo("Two"));
        }

        #endregion GetProfileAsync Tests - Multiple Users
    }
}