using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace FurnitureGardenDesign.Tests.Unit.Services.Core
{
    [TestFixture]
    public class ManagerServiceTests
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private ManagerService _service;
        private string _testUserId;

        [SetUp]
        public void SetUp()
        {
            var store = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _service = new ManagerService(_userManagerMock.Object);
            _testUserId = "test-user-123";
        }

        #region IsUserManagerAsync Tests

        [Test]
        public async Task IsUserManagerAsync_ReturnsTrue_WhenUserExistsAndIsManager()
        {
            var user = new AppUser { Id = _testUserId };
            _userManagerMock.Setup(x => x.FindByIdAsync(_testUserId))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.IsInRoleAsync(user, "Manager"))
                .ReturnsAsync(true);

            var result = await _service.IsUserManagerAsync(_testUserId);

            Assert.That(result, Is.True);
            _userManagerMock.Verify(x => x.FindByIdAsync(_testUserId), Times.Once);
            _userManagerMock.Verify(x => x.IsInRoleAsync(user, "Manager"), Times.Once);
        }

        [Test]
        public async Task IsUserManagerAsync_ReturnsFalse_WhenUserExistsButIsNotManager()
        {
            var user = new AppUser { Id = _testUserId };
            _userManagerMock.Setup(x => x.FindByIdAsync(_testUserId))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.IsInRoleAsync(user, "Manager"))
                .ReturnsAsync(false);

            var result = await _service.IsUserManagerAsync(_testUserId);

            Assert.That(result, Is.False);
            _userManagerMock.Verify(x => x.FindByIdAsync(_testUserId), Times.Once);
            _userManagerMock.Verify(x => x.IsInRoleAsync(user, "Manager"), Times.Once);
        }

        [Test]
        public async Task IsUserManagerAsync_ReturnsFalse_WhenUserDoesNotExist()
        {
            _userManagerMock.Setup(x => x.FindByIdAsync(_testUserId))
                .ReturnsAsync((AppUser)null);

            var result = await _service.IsUserManagerAsync(_testUserId);

            Assert.That(result, Is.False);
            _userManagerMock.Verify(x => x.FindByIdAsync(_testUserId), Times.Once);
            _userManagerMock.Verify(x => x.IsInRoleAsync(It.IsAny<AppUser>(), "Manager"), Times.Never);
        }

        [Test]
        public async Task IsUserManagerAsync_ReturnsFalse_WhenUserIdIsEmpty()
        {
            var result = await _service.IsUserManagerAsync(string.Empty);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task IsUserManagerAsync_ReturnsFalse_WhenUserIdIsNull()
        {
            var result = await _service.IsUserManagerAsync(null);

            Assert.That(result, Is.False);

        #endregion IsUserManagerAsync Tests
        }
    }
}