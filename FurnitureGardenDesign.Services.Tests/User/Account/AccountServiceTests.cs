using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Implementations;
using FurnitureGardenDesign.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FurnitureGardenDesign.Services.Tests.User.Profile
{
    [TestFixture]
    public class AccountServiceTests
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<SignInManager<AppUser>> _signInManagerMock;
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

            _accountService = new AccountService(
                _userManagerMock.Object,
                _signInManagerMock.Object);

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
                LastName = "Doe",
                Address = "123 Test Street"
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
                LastName = "Doe",
                Address = "123 Test Street"
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
                    Assert.That(user.Address, Is.EqualTo(_validRegisterModel.Address));
                });

            _userManagerMock
                .Setup(um => um.IsInRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(false);

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
            _userManagerMock.Verify(um => um.IsInRoleAsync(It.IsAny<AppUser>(), "User"), Times.Once);
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
            Assert.That(result.Errors.Count(), Is.EqualTo(2));
            Assert.That(result.Errors, Contains.Item("Password is too short"));
            Assert.That(result.Errors, Contains.Item("Email is invalid"));

            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<AppUser>(), _validRegisterModel.Password), Times.Once);
            _userManagerMock.Verify(um => um.IsInRoleAsync(It.IsAny<AppUser>(), "User"), Times.Never);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<AppUser>(), "User"), Times.Never);
            _signInManagerMock.Verify(sm => sm.SignInAsync(It.IsAny<AppUser>(), false, null), Times.Never);
        }

        [Test]
        public async Task RegisterAsync_WhenUserAlreadyInUserRole_DoesNotAddRoleAgain()
        {
           
            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<AppUser>(), _validRegisterModel.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock
                .Setup(um => um.IsInRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(true); // User already in role

            _signInManagerMock
                .Setup(sm => sm.SignInAsync(It.IsAny<AppUser>(), false, null))
                .Returns(Task.CompletedTask);

            var result = await _accountService.RegisterAsync(_validRegisterModel);

          
            Assert.That(result.Success, Is.True);
            Assert.That(result.Errors, Is.Empty);

            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<AppUser>(), _validRegisterModel.Password), Times.Once);
            _userManagerMock.Verify(um => um.IsInRoleAsync(It.IsAny<AppUser>(), "User"), Times.Once);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<AppUser>(), "User"), Times.Never);
            _signInManagerMock.Verify(sm => sm.SignInAsync(It.IsAny<AppUser>(), false, null), Times.Once);
        }

        [Test]
        public async Task RegisterAsync_WhenAddToRoleFails_StillReturnsSuccess_AndSignsIn()
        {
        
            var roleErrors = new List<IdentityError>
            {
                new IdentityError { Code = "RoleFailure", Description = "Failed to add role" }
            };

            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<AppUser>(), _validRegisterModel.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock
                .Setup(um => um.IsInRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(false);

            _userManagerMock
                .Setup(um => um.AddToRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(IdentityResult.Failed(roleErrors.ToArray()));

            _signInManagerMock
                .Setup(sm => sm.SignInAsync(It.IsAny<AppUser>(), false, null))
                .Returns(Task.CompletedTask);

            
            var result = await _accountService.RegisterAsync(_validRegisterModel);

        
            Assert.That(result.Success, Is.True); 
            Assert.That(result.Errors, Is.Empty);

            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<AppUser>(), _validRegisterModel.Password), Times.Once);
            _userManagerMock.Verify(um => um.IsInRoleAsync(It.IsAny<AppUser>(), "User"), Times.Once);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<AppUser>(), "User"), Times.Once);
            _signInManagerMock.Verify(sm => sm.SignInAsync(It.IsAny<AppUser>(), false, null), Times.Once);
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
                .Setup(um => um.IsInRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(true);

            _signInManagerMock
                .Setup(sm => sm.SignInAsync(It.IsAny<AppUser>(), false, null))
                .Returns(Task.CompletedTask);

         
            await _accountService.RegisterAsync(_validRegisterModel);

         
            Assert.That(capturedUser, Is.Not.Null);
            Assert.That(capturedUser.Email, Is.EqualTo("test@example.com"));
            Assert.That(capturedUser.UserName, Is.EqualTo("test@example.com"));
            Assert.That(capturedUser.FirstName, Is.EqualTo("John"));
            Assert.That(capturedUser.LastName, Is.EqualTo("Doe"));
            Assert.That(capturedUser.Address, Is.EqualTo("123 Test Street"));
        }

        #endregion

        #region LoginAsync Tests

        [Test]
        public async Task LoginAsync_WithValidCredentials_ReturnsTrue()
        {
          
            _userManagerMock
                .Setup(um => um.FindByEmailAsync(_validLoginModel.Email))
                .ReturnsAsync(_testUser);

            _signInManagerMock
                .Setup(sm => sm.PasswordSignInAsync(_testUser, _validLoginModel.Password, _validLoginModel.RememberMe, false))
                .ReturnsAsync(SignInResult.Success);

          
            var result = await _accountService.LoginAsync(_validLoginModel);

            
            Assert.That(result, Is.True);

            _userManagerMock.Verify(um => um.FindByEmailAsync(_validLoginModel.Email), Times.Once);
            _signInManagerMock.Verify(sm => sm.PasswordSignInAsync(_testUser, _validLoginModel.Password, _validLoginModel.RememberMe, false), Times.Once);
        }

        [Test]
        public async Task LoginAsync_WithInvalidPassword_ReturnsFalse()
        {
           
            _userManagerMock
                .Setup(um => um.FindByEmailAsync(_validLoginModel.Email))
                .ReturnsAsync(_testUser);

            _signInManagerMock
                .Setup(sm => sm.PasswordSignInAsync(_testUser, _validLoginModel.Password, _validLoginModel.RememberMe, false))
                .ReturnsAsync(SignInResult.Failed);

           
            var result = await _accountService.LoginAsync(_validLoginModel);

           
            Assert.That(result, Is.False);

            _userManagerMock.Verify(um => um.FindByEmailAsync(_validLoginModel.Email), Times.Once);
            _signInManagerMock.Verify(sm => sm.PasswordSignInAsync(_testUser, _validLoginModel.Password, _validLoginModel.RememberMe, false), Times.Once);
        }

      

        [Test]
        public async Task LoginAsync_WithLockedOutAccount_ReturnsFalse()
        {
          
            _userManagerMock
                .Setup(um => um.FindByEmailAsync(_validLoginModel.Email))
                .ReturnsAsync(_testUser);

            _signInManagerMock
                .Setup(sm => sm.PasswordSignInAsync(_testUser, _validLoginModel.Password, _validLoginModel.RememberMe, false))
                .ReturnsAsync(SignInResult.LockedOut);

            var result = await _accountService.LoginAsync(_validLoginModel);

            Assert.That(result, Is.False);

            _userManagerMock.Verify(um => um.FindByEmailAsync(_validLoginModel.Email), Times.Once);
            _signInManagerMock.Verify(sm => sm.PasswordSignInAsync(_testUser, _validLoginModel.Password, _validLoginModel.RememberMe, false), Times.Once);
        }

        [Test]
        public async Task LoginAsync_WithNotAllowedAccount_ReturnsFalse()
        {
          
            _userManagerMock
                .Setup(um => um.FindByEmailAsync(_validLoginModel.Email))
                .ReturnsAsync(_testUser);

            _signInManagerMock
                .Setup(sm => sm.PasswordSignInAsync(_testUser, _validLoginModel.Password, _validLoginModel.RememberMe, false))
                .ReturnsAsync(SignInResult.NotAllowed);

           
            var result = await _accountService.LoginAsync(_validLoginModel);

           
            Assert.That(result, Is.False);

            _userManagerMock.Verify(um => um.FindByEmailAsync(_validLoginModel.Email), Times.Once);
            _signInManagerMock.Verify(sm => sm.PasswordSignInAsync(_testUser, _validLoginModel.Password, _validLoginModel.RememberMe, false), Times.Once);
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

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(loginModel.Email))
                .ReturnsAsync(_testUser);

            _signInManagerMock
                .Setup(sm => sm.PasswordSignInAsync(_testUser, loginModel.Password, loginModel.RememberMe, false))
                .ReturnsAsync(SignInResult.Success);

         
            var result = await _accountService.LoginAsync(loginModel);

         
            Assert.That(result, Is.True);
            _signInManagerMock.Verify(sm => sm.PasswordSignInAsync(_testUser, loginModel.Password, false, false), Times.Once);
        }

        #endregion

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

        #endregion

        #region Edge Cases and Validation Tests

      

        [Test]
        public async Task RegisterAsync_WithEmptyFirstName_CreatesUserWithNullFirstName()
        {
           
            var modelWithNullFirstName = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = null,
                LastName = "Doe",
                Address = "123 Test St"
            };

            AppUser capturedUser = null;

            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<AppUser>(), modelWithNullFirstName.Password))
                .Callback<AppUser, string>((user, password) => capturedUser = user)
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock
                .Setup(um => um.IsInRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(true);

            _signInManagerMock
                .Setup(sm => sm.SignInAsync(It.IsAny<AppUser>(), false, null))
                .Returns(Task.CompletedTask);

            await _accountService.RegisterAsync(modelWithNullFirstName);

     
            Assert.That(capturedUser, Is.Not.Null);
            Assert.That(capturedUser.FirstName, Is.Null);
            Assert.That(capturedUser.LastName, Is.EqualTo("Doe"));
        }

        [Test]
        public async Task RegisterAsync_WithEmptyAddress_CreatesUserWithNullAddress()
        {
           
            var modelWithNullAddress = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe",
                Address = null
            };

            AppUser capturedUser = null;

            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<AppUser>(), modelWithNullAddress.Password))
                .Callback<AppUser, string>((user, password) => capturedUser = user)
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock
                .Setup(um => um.IsInRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(true);

            _signInManagerMock
                .Setup(sm => sm.SignInAsync(It.IsAny<AppUser>(), false, null))
                .Returns(Task.CompletedTask);


            await _accountService.RegisterAsync(modelWithNullAddress);

   
            Assert.That(capturedUser, Is.Not.Null);
            Assert.That(capturedUser.FirstName, Is.EqualTo("John"));
            Assert.That(capturedUser.Address, Is.Null);
        }

        #endregion


      



    }
}