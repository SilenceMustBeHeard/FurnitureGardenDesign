using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Web.Areas.Admin.Controllers.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;

namespace FurnitureGardenDesign.Tests.Integration.Controllers.Admin.Account
{
    [TestFixture]
    public class BaseAdminControllerTests
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private TestAdminController _controller;
        private string _testUserId;

        private class TestAdminController : BaseAdminController
        {
            public TestAdminController(UserManager<AppUser> userManager) : base(userManager)
            {
            }

            public bool PublicIsUserAdmin() => IsUserAdmin();

            public bool PublicIsUserAuthenticated() => IsUserAuthenticated();

            public Guid PublicGetUserId() => GetUserId();

            public Task<AppUser?> PublicGetCurrentUserAsync() => GetCurrentUserAsync();
        }

        [SetUp]
        public void SetUp()
        {
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

            _testUserId = "11111111-1111-1111-1111-111111111111";
            _controller = new TestAdminController(_userManagerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        private void SetUserContext(bool isAuthenticated, bool isAdmin, string userId)
        {
            var claims = new List<Claim>();
            if (isAuthenticated)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
                claims.Add(new Claim(ClaimTypes.Name, "test@example.com"));
            }
            if (isAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var identity = new ClaimsIdentity(claims, isAuthenticated ? "TestAuth" : null);
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        #region IsUserAdmin Tests

        [Test]
        public void IsUserAdmin_ReturnsTrue_WhenUserHasAdminRole()
        {
            SetUserContext(true, true, _testUserId);

            var result = _controller.PublicIsUserAdmin();

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsUserAdmin_ReturnsFalse_WhenUserDoesNotHaveAdminRole()
        {
            SetUserContext(true, false, _testUserId);

            var result = _controller.PublicIsUserAdmin();

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsUserAdmin_ReturnsFalse_WhenUserNotAuthenticated()
        {
            SetUserContext(false, false, _testUserId);

            var result = _controller.PublicIsUserAdmin();

            Assert.That(result, Is.False);
        }

        #endregion IsUserAdmin Tests

        #region IsUserAuthenticated Tests

        [Test]
        public void IsUserAuthenticated_ReturnsTrue_WhenUserIsAuthenticated()
        {
            SetUserContext(true, false, _testUserId);

            var result = _controller.PublicIsUserAuthenticated();

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsUserAuthenticated_ReturnsFalse_WhenUserIsNotAuthenticated()
        {
            SetUserContext(false, false, _testUserId);

            var result = _controller.PublicIsUserAuthenticated();

            Assert.That(result, Is.False);
        }

        #endregion IsUserAuthenticated Tests

        #region GetUserId Tests

        [Test]
        public void GetUserId_ReturnsGuid_WhenUserIsAuthenticated()
        {
            SetUserContext(true, true, _testUserId);
            _userManagerMock.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(_testUserId);

            var result = _controller.PublicGetUserId();

            Assert.That(result, Is.EqualTo(Guid.Parse(_testUserId)));
        }

        #endregion GetUserId Tests

        #region GetCurrentUserAsync Tests

        [Test]
        public async Task GetCurrentUserAsync_ReturnsUser_WhenUserExists()
        {
            var user = new AppUser { Id = _testUserId, Email = "test@example.com" };
            SetUserContext(true, true, _testUserId);
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var result = await _controller.PublicGetCurrentUserAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testUserId));
        }

        [Test]
        public async Task GetCurrentUserAsync_ReturnsNull_WhenUserNotFound()
        {
            SetUserContext(true, true, _testUserId);
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((AppUser)null);

            var result = await _controller.PublicGetCurrentUserAsync();

            Assert.That(result, Is.Null);
        }

        #endregion GetCurrentUserAsync Tests
    }
}