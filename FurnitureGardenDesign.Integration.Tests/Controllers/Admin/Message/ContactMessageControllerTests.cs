using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Implementations.Message;
using FurnitureGardenDesign.Data.Repository.Interfaces.Message;
using FurnitureGardenDesign.Services.Core.Admin.Implementations.Message;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Web.Areas.Admin.Controllers.Message;
using FurnitureGardenDesign.Web.ViewModels.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace FurnitureGardenDesign.Tests.Integration.Controllers.Admin.Message
{
    [TestFixture]
    public class ContactMessageControllerIntegrationTests
    {
        private ServiceProvider _serviceProvider;
        private ApplicationDbContext _context;
        private UserManager<AppUser> _userManager;
        private ContactMessageController _controller;
        private AppUser _testAdmin;
        private AppUser _testUser;
        private ContactMessage _testMessage;
        private ContactMessage _respondedMessage;

        [SetUp]
        public async Task SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            var services = new ServiceCollection();
            services.AddSingleton(_context);
            services.AddLogging();
            services.AddOptions();
            services.AddHttpContextAccessor();

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IContactMessageRepository, ContactMessageRepository>();
            services.AddScoped<IContactMessageService, ContactMessageService>();

            _serviceProvider = services.BuildServiceProvider();

            _userManager = _serviceProvider.GetRequiredService<UserManager<AppUser>>();

            var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("User"));

            await SeedTestData();

            var contactMessageService = _serviceProvider.GetRequiredService<IContactMessageService>();

            _controller = new ContactMessageController(
                contactMessageService,
                _userManager);
        }

        [TearDown]
        public async Task TearDown()
        {
            _controller?.Dispose();
            _userManager?.Dispose();

            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            if (_context != null)
            {
                await _context.Database.EnsureDeletedAsync();
                await _context.DisposeAsync();
            }
        }

        private async Task SeedTestData()
        {
            _testAdmin = new AppUser
            {
                Id = "admin-123",
                UserName = "admin@example.com",
                Email = "admin@example.com",
                FirstName = "Admin",
                LastName = "User"
            };
            await _userManager.CreateAsync(_testAdmin, "Admin123!");
            await _userManager.AddToRoleAsync(_testAdmin, "Admin");

            _testUser = new AppUser
            {
                Id = "user-456",
                UserName = "user@example.com",
                Email = "user@example.com",
                FirstName = "Regular",
                LastName = "User"
            };
            await _userManager.CreateAsync(_testUser, "User123!");
            await _userManager.AddToRoleAsync(_testUser, "User");

            _testMessage = new ContactMessage
            {
                Id = Guid.NewGuid(),
                SenderId = _testUser.Id,
                ReceiverId = _testAdmin.Id,
                Subject = "Question about order",
                Message = "When will my order be shipped?",
                Type = InboxMessageType.ContactMessage,
                IsRead = false,
                IsReadByAdmin = false,
                CreatedOn = DateTime.UtcNow,
                Response = null
            };

            _respondedMessage = new ContactMessage
            {
                Id = Guid.NewGuid(),
                SenderId = _testUser.Id,
                ReceiverId = _testAdmin.Id,
                Subject = "Design modification",
                Message = "Can you modify the design?",
                Type = InboxMessageType.ContactMessage,
                IsRead = true,
                IsReadByAdmin = true,
                CreatedOn = DateTime.UtcNow.AddDays(-1),
                Response = "Yes, I can modify it.",
                RespondedAt = DateTime.UtcNow.AddDays(-1),
                RespondedById = _testAdmin.Id
            };

            _context.ContactMessages.AddRange(_testMessage, _respondedMessage);
            await _context.SaveChangesAsync();
        }

        private void SetUserContext(string userId, string role = "Admin")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, $"{userId}@example.com"),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        #region Index Tests

        // TO DO : fix the test with proper setup of the message and user context,

        //[Test]
        //public async Task Index_Get_ReturnsViewWithMessages()
        //{
        //    SetUserContext(_testAdmin.Id, "Admin");

        //    var result = await _controller.Index();

        //    Assert.That(result, Is.TypeOf<ViewResult>());
        //}

        #endregion

        #region Details Tests


        // TO DO : fix the test with proper setup of the message and user context,
        //[Test]
        //public async Task Details_Get_WithEmptyGuid_ReturnsBadRequest()
        //{
        //    SetUserContext(_testAdmin.Id, "Admin");

        //    var result = await _controller.Details(Guid.Empty);

        //    Assert.That(result, Is.TypeOf<BadRequestResult>());
        //}

        [Test]
        public async Task Details_Get_WhenMessageExists_ReturnsView()
        {
            SetUserContext(_testAdmin.Id, "Admin");

            var result = await _controller.Details(_testMessage.Id);

            Assert.That(result, Is.TypeOf<ViewResult>());
        }  
        
        
        // TO DO : fix the test with proper setup of the message and user context,

        //[Test]
        //public async Task Details_Get_WhenMessageNotFound_ReturnsNotFound()
        //{
        //    SetUserContext(_testAdmin.Id, "Admin");

        //    var result = await _controller.Details(Guid.NewGuid());

        //    Assert.That(result, Is.TypeOf<NotFoundResult>());
        //}

        #endregion

        #region Respond GET Tests

        [Test]
        public async Task Respond_Get_WhenMessageExistsAndNotResponded_ReturnsView()
        {
            SetUserContext(_testAdmin.Id, "Admin");

            var result = await _controller.Respond(_testMessage.Id);

            Assert.That(result, Is.TypeOf<ViewResult>());
        }


        // TO DO : fix the test with proper setup of the message and user context,

        //[Test]
        //public async Task Respond_Get_WhenMessageNotFound_ReturnsNotFound()
        //{
        //    SetUserContext(_testAdmin.Id, "Admin");

        //    var result = await _controller.Respond(Guid.NewGuid());

        //    Assert.That(result, Is.TypeOf<NotFoundResult>());
        //}



        //   TO DO : fix the test with proper setup of the message and user context,
        //[Test]
        //public async Task Respond_Get_WhenAlreadyResponded_RedirectsToDetails()
        //{
        //    SetUserContext(_testAdmin.Id, "Admin");

        //    var result = await _controller.Respond(_respondedMessage.Id);

        //    Assert.That(result, Is.TypeOf<ContactMessageDetailsViewModel>());
        //}

        #endregion

        #region Respond POST Tests

        // TO DO : fix the test with proper setup of the message and user context,

        //[Test]
        //public async Task Respond_Post_WithValidModel_RedirectsToDetails()
        //{
        //    SetUserContext(_testAdmin.Id, "Admin");
        //    var model = new ContactMessageResponseViewModel
        //    {
        //        Id = _testMessage.Id,
        //        Response = "Thank you for your message. We'll ship it soon."
        //    };

        //    var result = await _controller.Respond(model);

        //    Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        //}

        [Test]
        public async Task Respond_Post_WithInvalidModel_ReturnsView()
        {
            SetUserContext(_testAdmin.Id, "Admin");
            var model = new ContactMessageResponseViewModel { Id = _testMessage.Id };

            _controller.ModelState.AddModelError("Response", "Response is required");

            var result = await _controller.Respond(model);

            Assert.That(result, Is.TypeOf<ViewResult>());
        }


        // TO DO : fix the test with proper setup of the message and user context,
        // currently it fails because the message is already responded in the seed data,
        // need to create a new message for this test or adjust the seed data accordingly

        //[Test]
        //public async Task Respond_Post_WhenAlreadyResponded_RedirectsToDetailsWithError()
        //{
        //    SetUserContext(_testAdmin.Id, "Admin");
        //    var model = new ContactMessageResponseViewModel
        //    {
        //        Id = _respondedMessage.Id,
        //        Response = "Second response"
        //    };

        //    var result = await _controller.Respond(model);

        //    Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        //}

        #endregion

        #region Authorization Tests

        [Test]
        public void Controller_HasAuthorizeAttribute()
        {
            var controllerType = typeof(ContactMessageController);
            var authorizeAttribute = Attribute.GetCustomAttribute(controllerType, typeof(AuthorizeAttribute)) as AuthorizeAttribute;

            Assert.That(authorizeAttribute, Is.Not.Null);
            Assert.That(authorizeAttribute.Roles, Is.EqualTo("Admin"));
        }

        [Test]
        public void Controller_HasAreaAttribute()
        {
            var controllerType = typeof(ContactMessageController);
            var areaAttribute = Attribute.GetCustomAttribute(controllerType, typeof(AreaAttribute)) as AreaAttribute;

            Assert.That(areaAttribute, Is.Not.Null);
            Assert.That(areaAttribute.RouteValue, Is.EqualTo("Admin"));
        }

        #endregion
    }
}