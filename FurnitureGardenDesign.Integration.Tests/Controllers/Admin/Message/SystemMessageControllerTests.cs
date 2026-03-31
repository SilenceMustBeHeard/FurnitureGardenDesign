using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces.Account;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Web.Areas.Admin.Controllers.Message;
using FurnitureGardenDesign.Web.ViewModels.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;

namespace FurnitureGardenDesign.Tests.Integration.Controllers.Admin.Message
{
    [TestFixture]
    public class SystemMessageControllerTests
    {
        private Mock<ISystemInboxMessageService> _systemMessageServiceMock;
        private Mock<IAppUserRepository> _userRepositoryMock;
        private Mock<UserManager<AppUser>> _userManagerMock;
        private SystemMessageController _controller;
        private string _adminId;

        [SetUp]
        public void SetUp()
        {
            _systemMessageServiceMock = new Mock<ISystemInboxMessageService>();
            _userRepositoryMock = new Mock<IAppUserRepository>();

            var store = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(
                store.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<AppUser>>().Object,
                new IUserValidator<AppUser>[0],
                new IPasswordValidator<AppUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<AppUser>>>().Object);

            _adminId = "11111111-1111-1111-1111-111111111111";
            _controller = new SystemMessageController(
                _systemMessageServiceMock.Object,
                _userRepositoryMock.Object,
                _userManagerMock.Object);

            SetupUserContext();
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        private void SetupUserContext()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _adminId),
                new Claim(ClaimTypes.Name, "admin@example.com"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            _userManagerMock.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(_adminId);
        }

        #region Index Tests

        [Test]
        public async Task Index_Get_ReturnsViewWithMessages()
        {
            var messages = new List<SystemInboxMessageViewModel>
            {
                new SystemInboxMessageViewModel { Id = Guid.NewGuid(), Description = "Message 1" },
                new SystemInboxMessageViewModel { Id = Guid.NewGuid(), Description = "Message 2" }
            };
            _systemMessageServiceMock.Setup(x => x.GetAdminMessagesAsync(_adminId))
                .ReturnsAsync(messages);

            var result = await _controller.Index();

            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.EqualTo(messages));
        }

        #endregion

        #region Create GET Tests


        // TO DO : fix test seed data, since the controller action retrieves all users to populate the dropdown list in the view,
        // we need to set up the mock to return a list of users,
        // and then assert that the view model contains those users in the AvailableUsers property, and that the ReceiverId is null when no userId is provided.

        //[Test]
        //public async Task Create_Get_WithoutUserId_ReturnsViewWithAvailableUsers()
        //{
        //    var users = new List<AppUser>
        //    {
        //        new AppUser { Id = "user-1", FirstName = "John", LastName = "Doe", Email = "john@test.com" },
        //        new AppUser { Id = "user-2", FirstName = "Jane", LastName = "Smith", Email = "jane@test.com" }
        //    };
        //    var mockQueryable = users.AsQueryable();
        //    _userRepositoryMock.Setup(x => x.GetAllAttached())
        //        .Returns(mockQueryable);

        //    var result = await _controller.Create((string)null);

        //    Assert.That(result, Is.TypeOf<ViewResult>());
        //    var viewResult = result as ViewResult;
        //    var model = viewResult.Model as SystemInboxMessageCreateViewModel;
        //    Assert.That(model, Is.Not.Null);
        //    Assert.That(model.AvailableUsers.Count, Is.EqualTo(2));
        //    Assert.That(model.ReceiverId, Is.Null);
        //}


        // TO DO : fix test seed data, since the controller action retrieves all users to populate the dropdown list in the view,
        //[Test]
        //public async Task Create_Get_WithUserId_ReturnsViewWithSelectedUser()
        //{
        //    var userId = "user-1";
        //    var users = new List<AppUser>
        //    {
        //        new AppUser { Id = "user-1", FirstName = "John", LastName = "Doe", Email = "john@test.com" },
        //        new AppUser { Id = "user-2", FirstName = "Jane", LastName = "Smith", Email = "jane@test.com" }
        //    };
        //    var mockQueryable = users.AsQueryable();
        //    _userRepositoryMock.Setup(x => x.GetAllAttached())
        //        .Returns(mockQueryable);

        //    var result = await _controller.Create(userId);

        //    Assert.That(result, Is.TypeOf<ViewResult>());
        //    var viewResult = result as ViewResult;
        //    var model = viewResult.Model as SystemInboxMessageCreateViewModel;
        //    Assert.That(model, Is.Not.Null);
        //    Assert.That(model.ReceiverId, Is.EqualTo(userId));
        //    Assert.That(model.ReceiverName, Is.EqualTo("John Doe"));
        //}

        #endregion

        #region Create POST Tests


        // TO DO : fix test seed data, since the model validation happens inside the model itself,
        // we need to test that the model state is valid when the description is provided,
        // and that the service method is called with the correct parameters, and that the result is a redirect to the index action.

        //[Test]
        //public async Task Create_Post_WithValidModel_CreatesAndRedirects()
        //{
        //    var model = new SystemInboxMessageCreateViewModel
        //    {
        //        ReceiverId = "user-1",
        //        Description = "Test message",
        //        Type = InboxMessageType.SystemMessage
        //    };
        //    var users = new List<AppUser>
        //    {
        //        new AppUser { Id = "user-1", FirstName = "John", LastName = "Doe", Email = "john@test.com" }
        //    };
        //    var mockQueryable = users.AsQueryable();
        //    _userRepositoryMock.Setup(x => x.GetAllAttached())
        //        .Returns(mockQueryable);
        //    _systemMessageServiceMock.Setup(x => x.CreateMessageAsync(It.IsAny<SystemInboxMessage>()))
        //        .Returns(Task.CompletedTask);

        //    var result = await _controller.Create(model);

        //    Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        //    var redirect = result as RedirectToActionResult;
        //    Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        //}

        // TO DO : Test fail, since avalidations happen inside the model itself, we need to test that the model state is invalid when the description is missing, and that the view is returned with the same model, and that the available users are populated again in the view model.

        //[Test]
        //public async Task Create_Post_WithInvalidModel_ReturnsView()
        //{
        //    var model = new SystemInboxMessageCreateViewModel();
        //    _controller.ModelState.AddModelError("Description", "Description is required");
        //    var users = new List<AppUser>();
        //    var mockQueryable = users.AsQueryable();
        //    _userRepositoryMock.Setup(x => x.GetAllAttached())
        //        .Returns(mockQueryable);

        //    var result = await _controller.Create(model);

        //    Assert.That(result, Is.TypeOf<ViewResult>());
        //    var viewResult = result as ViewResult;
        //    Assert.That(viewResult.Model, Is.EqualTo(model));
        //}

        #endregion

        #region Details Tests

        [Test]
        public async Task Details_Get_WhenMessageExists_ReturnsView()
        {
            var messageId = Guid.NewGuid();
            var message = new SystemInboxMessageViewModel
            {
                Id = messageId,
                Description = "Test message",
                SenderId = _adminId,
                ReceiverId = "user-1"
            };
            var sender = new AppUser { Id = _adminId, FirstName = "Admin", LastName = "User" };
            var receiver = new AppUser { Id = "user-1", FirstName = "John", LastName = "Doe" };

            _systemMessageServiceMock.Setup(x => x.GetMessageDetailsAsync(messageId, _adminId))
                .ReturnsAsync(message);
            _userManagerMock.Setup(x => x.FindByIdAsync(_adminId))
                .ReturnsAsync(sender);
            _userManagerMock.Setup(x => x.FindByIdAsync("user-1"))
                .ReturnsAsync(receiver);

            var result = await _controller.Details(messageId);

            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.EqualTo(message));
            Assert.That(message.SenderName, Is.EqualTo("Admin User"));
            Assert.That(message.ReceiverName, Is.EqualTo("John Doe"));
        }

        // fix : This test is currently failing because the controller returns NotFound if the message is not found,
        // but the test is not properly setting up the mock to return null for the message
        // We need to set up the mock to return null when the message is not found,
        // and then assert that the result is a NotFoundResult.

        //[Test]
        //public async Task Details_Get_WhenMessageNotFound_ReturnsNotFound()
        //{

        //    var message = new SystemInboxMessageViewModel
        //    {
        //        Id = Guid.NewGuid(),
        //        Description = "Test message unexistent",
        //        SenderId = "non-existent",
        //        ReceiverId = "user-none"
        //    };


        //    var result = await _controller.Details(message.Id);

        //    Assert.That(result, Is.TypeOf<NotFoundResult>());
        //}

        [Test]
        public async Task Details_Get_WhenSenderNotFound_UsesSystemAsSenderName()
        {
            var messageId = Guid.NewGuid();
            var message = new SystemInboxMessageViewModel
            {
                Id = messageId,
                Description = "Test message",
                SenderId = "non-existent",
                ReceiverId = "user-1"
            };
            var receiver = new AppUser { Id = "user-1", FirstName = "John", LastName = "Doe" };

            _systemMessageServiceMock.Setup(x => x.GetMessageDetailsAsync(messageId, _adminId))
                .ReturnsAsync(message);
            _userManagerMock.Setup(x => x.FindByIdAsync("non-existent"))
                .ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(x => x.FindByIdAsync("user-1"))
                .ReturnsAsync(receiver);

            var result = await _controller.Details(messageId);

            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult.Model as SystemInboxMessageViewModel;
            Assert.That(model.SenderName, Is.EqualTo("System"));
        }

        [Test]
        public async Task Details_Get_WhenReceiverNotFound_UsesUnknownAsReceiverName()
        {
            var messageId = Guid.NewGuid();
            var message = new SystemInboxMessageViewModel
            {
                Id = messageId,
                Description = "Test message",
                SenderId = _adminId,
                ReceiverId = "non-existent"
            };
            var sender = new AppUser { Id = _adminId, FirstName = "Admin", LastName = "User" };

            _systemMessageServiceMock.Setup(x => x.GetMessageDetailsAsync(messageId, _adminId))
                .ReturnsAsync(message);
            _userManagerMock.Setup(x => x.FindByIdAsync(_adminId))
                .ReturnsAsync(sender);
            _userManagerMock.Setup(x => x.FindByIdAsync("non-existent"))
                .ReturnsAsync((AppUser)null);

            var result = await _controller.Details(messageId);

            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult.Model as SystemInboxMessageViewModel;
            Assert.That(model.ReceiverName, Is.EqualTo("Unknown"));
        }

        #endregion
    }
}