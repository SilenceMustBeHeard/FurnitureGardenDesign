using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Implementations.Account;
using FurnitureGardenDesign.Services.Core.Interfaces.Account;
using FurnitureGardenDesign.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FurnitureGardenDesign.Unit.Tests.Services.User.Account
{
    [TestFixture]
    public class AccountServiceTests
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<SignInManager<AppUser>> _signInManagerMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IViewRenderService> _viewRenderServiceMock;
        private Mock<ILogger<AccountService>> _loggerMock;
        private AccountService _accountService;

        private RegisterViewModel _validRegisterModel;
        private LoginViewModel _validLoginModel;
        private AppUser _testUser;

        [SetUp]
        public void SetUp()
        {
            var userStoreMock = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(
                userStoreMock.Object,
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<AppUser>>(),
                new List<IUserValidator<AppUser>>(),
                new List<IPasswordValidator<AppUser>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<AppUser>>>());

            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<AppUser>>();

            _signInManagerMock = new Mock<SignInManager<AppUser>>(
                _userManagerMock.Object,
                contextAccessorMock.Object,
                userPrincipalFactoryMock.Object,
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<ILogger<SignInManager<AppUser>>>(),
                Mock.Of<IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<AppUser>>());

            _emailServiceMock = new Mock<IEmailService>();
            _viewRenderServiceMock = new Mock<IViewRenderService>();
            _loggerMock = new Mock<ILogger<AccountService>>();

            _accountService = new AccountService(
                _userManagerMock.Object, _signInManagerMock.Object, _emailServiceMock.Object, _viewRenderServiceMock.Object, _loggerMock.Object);
            SeedTestData();
        }

        private void SeedTestData()
        {
            _validRegisterModel = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };

            _validLoginModel = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                RememberMe = true
            };

            _testUser = new AppUser
            {
                Id = "test-user-id",
                UserName = "test@example.com",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe"
            };
        }

        #region RegisterAsync Tests

        [Test]
        public async Task RegisterAsync_WithValidModel_CreatesUser_AssignsUserRole_SignsIn_ReturnsSuccess()
        {
            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<AppUser>(), _validRegisterModel.Password))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<AppUser, string>((user, password) =>
                {
                    Assert.That(user.Email, Is.EqualTo(_validRegisterModel.Email));
                    Assert.That(user.UserName, Is.EqualTo(_validRegisterModel.Email));
                    Assert.That(user.FirstName, Is.EqualTo(_validRegisterModel.FirstName));
                    Assert.That(user.LastName, Is.EqualTo(_validRegisterModel.LastName));
                });

            _userManagerMock
                .Setup(um => um.AddToRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            _signInManagerMock
                .Setup(sm => sm.SignInAsync(It.IsAny<AppUser>(), false, null))
                .Returns(Task.CompletedTask);

            var result = await _accountService.RegisterAsync(_validRegisterModel);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Errors, Is.Empty);

            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<AppUser>(), _validRegisterModel.Password), Times.Once);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<AppUser>(), "User"), Times.Once);
            _signInManagerMock.Verify(sm => sm.SignInAsync(It.IsAny<AppUser>(), false, null), Times.Once);
        }

        [Test]
        public async Task RegisterAsync_WhenUserCreationFails_ReturnsErrors_AndDoesNotSignIn()
        {
            var identityErrors = new List<IdentityError>
            {
                new IdentityError { Code = "PasswordTooShort", Description = "Password is too short" },
                new IdentityError { Code = "InvalidEmail", Description = "Email is invalid" }
            };

            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<AppUser>(), _validRegisterModel.Password))
                .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            var result = await _accountService.RegisterAsync(_validRegisterModel);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors.Length, Is.EqualTo(2));
            Assert.That(result.Errors, Does.Contain("Password is too short"));
            Assert.That(result.Errors, Does.Contain("Email is invalid"));

            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<AppUser>(), _validRegisterModel.Password), Times.Once);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<AppUser>(), "User"), Times.Never);
            _signInManagerMock.Verify(sm => sm.SignInAsync(It.IsAny<AppUser>(), false, null), Times.Never);
        }

        [Test]
        public async Task RegisterAsync_CreatesUserWithCorrectProperties()
        {
            AppUser capturedUser = null;

            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<AppUser>(), _validRegisterModel.Password))
                .Callback<AppUser, string>((user, password) => capturedUser = user)
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock
                .Setup(um => um.AddToRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            _signInManagerMock
                .Setup(sm => sm.SignInAsync(It.IsAny<AppUser>(), false, null))
                .Returns(Task.CompletedTask);

            await _accountService.RegisterAsync(_validRegisterModel);

            Assert.That(capturedUser, Is.Not.Null);
            Assert.That(capturedUser.Email, Is.EqualTo("test@example.com"));
            Assert.That(capturedUser.UserName, Is.EqualTo("test@example.com"));
            Assert.That(capturedUser.FirstName, Is.EqualTo("John"));
            Assert.That(capturedUser.LastName, Is.EqualTo("Doe"));
        }

        [Test]
        public async Task RegisterAsync_WithNullFirstAndLastName_CreatesUserWithNullProperties()
        {
            var modelWithNullNames = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = null,
                LastName = null
            };

            AppUser capturedUser = null;

            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<AppUser>(), modelWithNullNames.Password))
                .Callback<AppUser, string>((user, password) => capturedUser = user)
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock
                .Setup(um => um.AddToRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            _signInManagerMock
                .Setup(sm => sm.SignInAsync(It.IsAny<AppUser>(), false, null))
                .Returns(Task.CompletedTask);

            await _accountService.RegisterAsync(modelWithNullNames);

            Assert.That(capturedUser, Is.Not.Null);
            Assert.That(capturedUser.FirstName, Is.Null);
            Assert.That(capturedUser.LastName, Is.Null);
        }

        #endregion RegisterAsync Tests

        #region LoginAsync Tests

        [Test]
        public async Task LoginAsync_WithValidCredentials_ReturnsTrue()
        {
            _signInManagerMock
                .Setup(sm => sm.PasswordSignInAsync(
                    _validLoginModel.Email,
                    _validLoginModel.Password,
                    _validLoginModel.RememberMe,
                    false))
                .ReturnsAsync(SignInResult.Success);

            var result = await _accountService.LoginAsync(_validLoginModel);

            Assert.That(result, Is.True);

            _signInManagerMock.Verify(sm => sm.PasswordSignInAsync(
                _validLoginModel.Email,
                _validLoginModel.Password,
                _validLoginModel.RememberMe,
                false), Times.Once);
        }

        [Test]
        public async Task LoginAsync_WithInvalidPassword_ReturnsFalse()
        {
            _signInManagerMock
                .Setup(sm => sm.PasswordSignInAsync(
                    _validLoginModel.Email,
                    _validLoginModel.Password,
                    _validLoginModel.RememberMe,
                    false))
                .ReturnsAsync(SignInResult.Failed);

            var result = await _accountService.LoginAsync(_validLoginModel);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task LoginAsync_WithLockedOutAccount_ReturnsFalse()
        {
            _signInManagerMock
                .Setup(sm => sm.PasswordSignInAsync(
                    _validLoginModel.Email,
                    _validLoginModel.Password,
                    _validLoginModel.RememberMe,
                    false))
                .ReturnsAsync(SignInResult.LockedOut);

            var result = await _accountService.LoginAsync(_validLoginModel);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task LoginAsync_WithNotAllowedAccount_ReturnsFalse()
        {
            _signInManagerMock
                .Setup(sm => sm.PasswordSignInAsync(
                    _validLoginModel.Email,
                    _validLoginModel.Password,
                    _validLoginModel.RememberMe,
                    false))
                .ReturnsAsync(SignInResult.NotAllowed);

            var result = await _accountService.LoginAsync(_validLoginModel);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task LoginAsync_WithRememberMeFalse_PassesCorrectParameter()
        {
            var loginModel = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                RememberMe = false
            };

            _signInManagerMock
                .Setup(sm => sm.PasswordSignInAsync(
                    loginModel.Email,
                    loginModel.Password,
                    loginModel.RememberMe,
                    false))
                .ReturnsAsync(SignInResult.Success);

            var result = await _accountService.LoginAsync(loginModel);

            Assert.That(result, Is.True);
            _signInManagerMock.Verify(sm => sm.PasswordSignInAsync(
                loginModel.Email,
                loginModel.Password,
                false,
                false), Times.Once);
        }

        #endregion LoginAsync Tests

        #region LogoutAsync Tests

        [Test]
        public async Task LogoutAsync_CallsSignOutAsync()
        {
            _signInManagerMock
                .Setup(sm => sm.SignOutAsync())
                .Returns(Task.CompletedTask);

            await _accountService.LogoutAsync();

            _signInManagerMock.Verify(sm => sm.SignOutAsync(), Times.Once);
        }

        [Test]
        public async Task LogoutAsync_CompletesSuccessfully_EvenWhenCalledMultipleTimes()
        {
            _signInManagerMock
                .Setup(sm => sm.SignOutAsync())
                .Returns(Task.CompletedTask);

            await _accountService.LogoutAsync();
            await _accountService.LogoutAsync();

            _signInManagerMock.Verify(sm => sm.SignOutAsync(), Times.Exactly(2));
        }

        #endregion LogoutAsync Tests

        #region ForgotPasswordAsync Tests

        [Test]
        public async Task ForgotPasswordAsync_WithValidEmail_SendsEmail_ReturnsTrue()
        {
            var email = "test@example.com";
            var resetLink = "https://example.com/reset-password";
            var resetToken = "generated-reset-token";
            var fullResetLink = $"{resetLink}?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(email)}";

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync(_testUser);

            _userManagerMock
                .Setup(um => um.GeneratePasswordResetTokenAsync(_testUser))
                .ReturnsAsync(resetToken);

            _viewRenderServiceMock
                .Setup(vrs => vrs.RenderToStringAsync("Emails/PasswordReset", _testUser, It.IsAny<ViewDataDictionary>()))
                .ReturnsAsync("Email body content");

            _emailServiceMock
                .Setup(es => es.SendEmailAsync(email, "Password Reset Request", "Email body content"))
                .ReturnsAsync(true);

            var result = await _accountService.ForgotPasswordAsync(email, resetLink);

            Assert.That(result, Is.True);

            _userManagerMock.Verify(um => um.FindByEmailAsync(email), Times.Once);
            _userManagerMock.Verify(um => um.GeneratePasswordResetTokenAsync(_testUser), Times.Once);
            _viewRenderServiceMock.Verify(vrs => vrs.RenderToStringAsync("Emails/PasswordReset", _testUser, It.IsAny<ViewDataDictionary>()), Times.Once);
            _emailServiceMock.Verify(es => es.SendEmailAsync(email, "Password Reset Request", "Email body content"), Times.Once);
        }

        [Test]
        public async Task ForgotPasswordAsync_WithNonExistentEmail_ReturnsFalse()
        {
            var email = "nonexistent@example.com";
            var resetLink = "https://example.com/reset-password";

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync((AppUser)null);

            var result = await _accountService.ForgotPasswordAsync(email, resetLink);

            Assert.That(result, Is.False);

            _userManagerMock.Verify(um => um.FindByEmailAsync(email), Times.Once);
            _userManagerMock.Verify(um => um.GeneratePasswordResetTokenAsync(It.IsAny<AppUser>()), Times.Never);
            _viewRenderServiceMock.Verify(vrs => vrs.RenderToStringAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<ViewDataDictionary>()), Times.Never);
            _emailServiceMock.Verify(es => es.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ForgotPasswordAsync_WhenEmailServiceFails_ReturnsFalse()
        {
            var email = "test@example.com";
            var resetLink = "https://example.com/reset-password";
            var resetToken = "generated-reset-token";

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync(_testUser);

            _userManagerMock
                .Setup(um => um.GeneratePasswordResetTokenAsync(_testUser))
                .ReturnsAsync(resetToken);

            _viewRenderServiceMock
                .Setup(vrs => vrs.RenderToStringAsync("Emails/PasswordReset", _testUser, It.IsAny<ViewDataDictionary>()))
                .ReturnsAsync("Email body content");

            _emailServiceMock
                .Setup(es => es.SendEmailAsync(email, "Password Reset Request", "Email body content"))
                .ReturnsAsync(false);

            var result = await _accountService.ForgotPasswordAsync(email, resetLink);

            Assert.That(result, Is.False);
        }

        #endregion ForgotPasswordAsync Tests

        #region ResetPasswordAsync Tests

        [Test]
        public async Task ResetPasswordAsync_WithValidModel_ResetsPassword_ReturnsSuccess()
        {
            var resetModel = new ResetPasswordViewModel
            {
                Email = "test@example.com",
                Token = "valid-token",
                Password = "NewPassword123!",
                ConfirmPassword = "NewPassword123!"
            };

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(resetModel.Email))
                .ReturnsAsync(_testUser);

            _userManagerMock
                .Setup(um => um.ResetPasswordAsync(_testUser, resetModel.Token, resetModel.Password))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _accountService.ResetPasswordAsync(resetModel);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Errors, Is.Empty);

            _userManagerMock.Verify(um => um.FindByEmailAsync(resetModel.Email), Times.Once);
            _userManagerMock.Verify(um => um.ResetPasswordAsync(_testUser, resetModel.Token, resetModel.Password), Times.Once);
        }

        [Test]
        public async Task ResetPasswordAsync_WithNonExistentEmail_ReturnsError()
        {
            var resetModel = new ResetPasswordViewModel
            {
                Email = "nonexistent@example.com",
                Token = "valid-token",
                Password = "NewPassword123!",
                ConfirmPassword = "NewPassword123!"
            };

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(resetModel.Email))
                .ReturnsAsync((AppUser)null);

            var result = await _accountService.ResetPasswordAsync(resetModel);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors[0], Is.EqualTo("User not found."));

            _userManagerMock.Verify(um => um.FindByEmailAsync(resetModel.Email), Times.Once);
            _userManagerMock.Verify(um => um.ResetPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ResetPasswordAsync_WithInvalidToken_ReturnsErrors()
        {
            var resetModel = new ResetPasswordViewModel
            {
                Email = "test@example.com",
                Token = "invalid-token",
                Password = "NewPassword123!",
                ConfirmPassword = "NewPassword123!"
            };

            var identityErrors = new List<IdentityError>
            {
                new IdentityError { Code = "InvalidToken", Description = "Invalid password reset token" }
            };

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(resetModel.Email))
                .ReturnsAsync(_testUser);

            _userManagerMock
                .Setup(um => um.ResetPasswordAsync(_testUser, resetModel.Token, resetModel.Password))
                .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            var result = await _accountService.ResetPasswordAsync(resetModel);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors[0], Is.EqualTo("Invalid password reset token"));
        }

        [Test]
        public async Task ResetPasswordAsync_WithWeakPassword_ReturnsErrors()
        {
            var resetModel = new ResetPasswordViewModel
            {
                Email = "test@example.com",
                Token = "valid-token",
                Password = "weak",
                ConfirmPassword = "weak"
            };

            var identityErrors = new List<IdentityError>
            {
                new IdentityError { Code = "PasswordTooWeak", Description = "Password must be at least 6 characters" }
            };

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(resetModel.Email))
                .ReturnsAsync(_testUser);

            _userManagerMock
                .Setup(um => um.ResetPasswordAsync(_testUser, resetModel.Token, resetModel.Password))
                .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            var result = await _accountService.ResetPasswordAsync(resetModel);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors[0], Is.EqualTo("Password must be at least 6 characters"));
        }

        #endregion ResetPasswordAsync Tests
    }
}