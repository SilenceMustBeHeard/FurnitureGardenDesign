using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Web.Areas.Admin.Controllers.Account;
using FurnitureGardenDesign.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace FurnitureGardenDesign.Tests.Integration.Controllers.Admin.Account
{
    [TestFixture]
    public class AdminAccountControllerTests
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<SignInManager<AppUser>> _signInManagerMock;
        private AccountController _controller;

        [SetUp]
        public void SetUp()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(
                new Mock<IUserStore<AppUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<AppUser>>().Object,
                new IUserValidator<AppUser>[0],
                new IPasswordValidator<AppUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<AppUser>>>().Object
            );

            _signInManagerMock = new Mock<SignInManager<AppUser>>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<AppUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<AppUser>>().Object
            );

            _controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object);

            SetupAdminUserContext();
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        private void SetupAdminUserContext()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "admin-123"),
                new Claim(ClaimTypes.Name, "admin@example.com"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        #region Register Tests

        [Test]
        public void Register_Get_ReturnsViewResult()
        {
            var result = _controller.Register();

            Assert.That(result, Is.TypeOf<ViewResult>());
        }

        [Test]
        public async Task Register_Post_WithValidModel_CreatesUserAndRedirects()
        {
            var model = new RegisterViewModel
            {
                Email = "newuser@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.IsInRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(false);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);
            _signInManagerMock.Setup(x => x.SignInAsync(It.IsAny<AppUser>(), false, null))
                .Returns(Task.CompletedTask);

            var result = await _controller.Register(model);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());

            var redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Home"));
        }

        [Test]
        public async Task Register_Post_WithInvalidModel_ReturnsSameView()
        {
            var model = new RegisterViewModel
            {
                Email = "invalid",
                Password = "123",
                ConfirmPassword = "456"
            };
            _controller.ModelState.AddModelError("Email", "Invalid email");

            var result = await _controller.Register(model);

            Assert.That(result, Is.TypeOf<ViewResult>());

            var viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task Register_Post_WithExistingEmail_ReturnsViewWithError()
        {
            var model = new RegisterViewModel
            {
                Email = "existing@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Email already taken" }));

            var result = await _controller.Register(model);

            Assert.That(result, Is.TypeOf<ViewResult>());
            Assert.That(_controller.ModelState.IsValid, Is.False);
        }

        #endregion Register Tests

        #region Login Tests

        [Test]
        public void Login_Get_ReturnsViewResult()
        {
            var result = _controller.Login();

            Assert.That(result, Is.TypeOf<ViewResult>());
        }

        [Test]
        public async Task Login_Post_WithValidAdminCredentials_RedirectsToAdminArea()
        {
            var model = new LoginViewModel
            {
                Email = "admin@example.com",
                Password = "Admin123!",
                RememberMe = false
            };

            var adminUser = new AppUser { Id = "admin-123", Email = "admin@example.com" };

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                .ReturnsAsync(SignInResult.Success);
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email))
                .ReturnsAsync(adminUser);
            _userManagerMock.Setup(x => x.IsInRoleAsync(adminUser, "Admin"))
                .ReturnsAsync(true);

            var result = await _controller.Login(model, null);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Home"));
            Assert.That(redirect.RouteValues, Is.Not.Null);
            Assert.That(redirect.RouteValues["area"], Is.EqualTo("Admin"));
        }

        [Test]
        public async Task Login_Post_WithValidUserCredentials_RedirectsToHome()
        {
            var model = new LoginViewModel
            {
                Email = "user@example.com",
                Password = "User123!",
                RememberMe = false
            };

            var regularUser = new AppUser { Id = "user-123", Email = "user@example.com" };

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                .ReturnsAsync(SignInResult.Success);
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email))
                .ReturnsAsync(regularUser);
            _userManagerMock.Setup(x => x.IsInRoleAsync(regularUser, "Admin"))
                .ReturnsAsync(false);

            var result = await _controller.Login(model, null);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());

            var redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Home"));
            Assert.That(redirect.RouteValues, Is.Not.Null);
            Assert.That(redirect.RouteValues["area"], Is.EqualTo(""));
        }

        [Test]
        public async Task Login_Post_WithInvalidCredentials_ReturnsViewWithError()
        {
            var model = new LoginViewModel
            {
                Email = "wrong@example.com",
                Password = "WrongPassword!",
                RememberMe = false
            };

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                .ReturnsAsync(SignInResult.Failed);

            var result = await _controller.Login(model, null);

            Assert.That(result, Is.TypeOf<ViewResult>());

            var viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.EqualTo(model));
            Assert.That(_controller.ModelState.IsValid, Is.False);
            Assert.That(_controller.ModelState, Is.Not.Null);
            Assert.That(_controller.ModelState[""].Errors[0].ErrorMessage, Is.EqualTo("Invalid login attempt."));
        }

        [Test]
        public async Task Login_Post_WithInvalidModel_ReturnsViewWithErrors()
        {
            var model = new LoginViewModel
            {
                Email = "",
                Password = "",
                RememberMe = false
            };
            _controller.ModelState.AddModelError("Email", "Email is required");

            var result = await _controller.Login(model, null);

            Assert.That(result, Is.TypeOf<ViewResult>());

            var viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.EqualTo(model));
            Assert.That(_controller.ModelState.IsValid, Is.False);
        }

        #endregion Login Tests

        #region Logout Tests

        [Test]
        public async Task Logout_Post_SignsOutAndRedirectsToHome()
        {
            _signInManagerMock.Setup(x => x.SignOutAsync())
                .Returns(Task.CompletedTask);

            var result = await _controller.Logout();

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());

            var redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Home"));
            Assert.That(redirect.RouteValues, Is.Not.Null);
            Assert.That(redirect.RouteValues["area"], Is.EqualTo(""));
            _signInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);
        }

        #endregion Logout Tests

        #region Authorization Tests

        [Test]
        public void Controller_HasAuthorizeAttribute()
        {
            var controllerType = typeof(AccountController);
            var authorizeAttribute = Attribute.GetCustomAttribute(controllerType, typeof(AuthorizeAttribute)) as AuthorizeAttribute;

            Assert.That(authorizeAttribute, Is.Not.Null);
            Assert.That(authorizeAttribute.Roles, Is.EqualTo("Admin"));
        }

        [Test]
        public void Controller_HasAreaAttribute()
        {
            var controllerType = typeof(AccountController);
            var areaAttribute = Attribute.GetCustomAttribute(controllerType, typeof(AreaAttribute)) as AreaAttribute;

            Assert.That(areaAttribute, Is.Not.Null);
            Assert.That(areaAttribute.RouteValue, Is.EqualTo("Admin"));
        }

        #endregion Authorization Tests
    }
}