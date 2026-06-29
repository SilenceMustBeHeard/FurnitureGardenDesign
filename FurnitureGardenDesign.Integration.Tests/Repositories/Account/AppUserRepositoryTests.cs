using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Implementations.Account;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Tests.Integration.Repositories.Account
{
    [TestFixture]
    public class AppUserRepositoryTests
    {
        private ApplicationDbContext _context;
        private AppUserRepository _repository;
        private string _testUserId;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new AppUserRepository(_context);
            _testUserId = "11111111-1111-1111-1111-111111111111";
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region AddAsync Tests

        [Test]
        public async Task AddAsync_AddsUserSuccessfully()
        {
            // Arrange
            var user = new AppUser
            {
                Id = _testUserId,
                Email = "test@example.com",
                UserName = "test@example.com",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            await _repository.AddAsync(user);

            // Assert
            var savedUser = await _context.Users.FindAsync(_testUserId);
            Assert.That(savedUser, Is.Not.Null);
            Assert.That(savedUser.Email, Is.EqualTo("test@example.com"));
            Assert.That(savedUser.FullName, Is.EqualTo("John Doe"));
        }

        [Test]
        public async Task AddAsync_ThrowsException_WhenUserWithSameIdExists()
        {
            // Arrange
            var user1 = new AppUser { Id = _testUserId, Email = "test1@example.com" };
            var user2 = new AppUser { Id = _testUserId, Email = "test2@example.com" };
            await _repository.AddAsync(user1);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _repository.AddAsync(user2));
        }

        #endregion AddAsync Tests

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_ReturnsUser_WhenExists()
        {
            // Arrange
            var user = new AppUser { Id = _testUserId, Email = "test@example.com" };
            await _repository.AddAsync(user);

            // Act
            var result = await _repository.GetByIdAsync(_testUserId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testUserId));
            Assert.That(result.Email, Is.EqualTo("test@example.com"));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Act
            var result = await _repository.GetByIdAsync("non-existent-id");

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion GetByIdAsync Tests

        #region GetAllAttached Tests

        [Test]
        public async Task GetAllAttached_ReturnsAllUsers()
        {
            // Arrange
            var users = new[]
            {
                new AppUser { Id = "user1", Email = "user1@example.com" },
                new AppUser { Id = "user2", Email = "user2@example.com" },
                new AppUser { Id = "user3", Email = "user3@example.com" }
            };

            foreach (var user in users)
            {
                await _repository.AddAsync(user);
            }

            // Act
            var result = _repository.GetAllAttached().ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result.Select(u => u.Email), Contains.Item("user1@example.com"));
        }

        [Test]
        public async Task GetAllAttached_ReturnsEmptyList_WhenNoUsers()
        {
            // Act
            var result = _repository.GetAllAttached().ToList();

            // Assert
            Assert.That(result, Is.Empty);
        }

        #endregion GetAllAttached Tests

        #region FirstOrDefaultAsync Tests

        [Test]
        public async Task FirstOrDefaultAsync_ReturnsUser_WhenConditionMatches()
        {
            // Arrange
            var user = new AppUser { Id = _testUserId, Email = "test@example.com" };
            await _repository.AddAsync(user);

            // Act
            var result = await _repository.FirstOrDefaultAsync(u => u.Email == "test@example.com");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(_testUserId));
        }

        [Test]
        public async Task FirstOrDefaultAsync_ReturnsNull_WhenConditionDoesNotMatch()
        {
            // Arrange
            var user = new AppUser { Id = _testUserId, Email = "test@example.com" };
            await _repository.AddAsync(user);

            // Act
            var result = await _repository.FirstOrDefaultAsync(u => u.Email == "nonexistent@example.com");

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion FirstOrDefaultAsync Tests

        #region CountAsync Tests

        [Test]
        public async Task CountAsync_ReturnsCorrectCount()
        {
            // Arrange
            await _repository.AddAsync(new AppUser { Id = "user1", Email = "user1@example.com" });
            await _repository.AddAsync(new AppUser { Id = "user2", Email = "user2@example.com" });

            // Act
            var result = await _repository.CountAsync();

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public async Task CountAsync_ReturnsZero_WhenNoUsers()
        {
            // Act
            var result = await _repository.CountAsync();

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        #endregion CountAsync Tests

        #region UpdateAsync Tests

        [Test]
        public async Task UpdateAsync_UpdatesUserSuccessfully()
        {
            // Arrange
            var user = new AppUser
            {
                Id = _testUserId,
                Email = "old@example.com",
                FirstName = "Old",
                LastName = "Name"
            };
            await _repository.AddAsync(user);

            user.Email = "new@example.com";
            user.FirstName = "New";
            user.LastName = "Name";

            // Act
            var result = await _repository.UpdateAsync(user);

            // Assert
            Assert.That(result, Is.True);
            var updatedUser = await _context.Users.FindAsync(_testUserId);
            Assert.That(updatedUser.Email, Is.EqualTo("new@example.com"));
            Assert.That(updatedUser.FullName, Is.EqualTo("New Name"));
        }

        [Test]
        public async Task UpdateAsync_ReturnsFalse_WhenUserNotTracked()
        {
            // Arrange
            var user = new AppUser
            {
                Id = _testUserId,
                Email = "test@example.com"
            };
            await _repository.AddAsync(user);

            // Detach the user
            _context.Entry(user).State = EntityState.Detached;

            var updatedUser = new AppUser
            {
                Id = _testUserId,
                Email = "updated@example.com"
            };

            // Act
            var result = await _repository.UpdateAsync(updatedUser);

            // Assert
            Assert.That(result, Is.False);
            var dbUser = await _context.Users.FindAsync(_testUserId);
            Assert.That(dbUser.Email, Is.EqualTo("updated@example.com"));
        }

        #endregion UpdateAsync Tests

        #region HardDeleteAsync Tests

        [Test]
        public async Task HardDeleteAsync_RemovesUserPermanently()
        {
            // Arrange
            var user = new AppUser { Id = _testUserId, Email = "test@example.com" };
            await _repository.AddAsync(user);

            // Act
            var result = await _repository.HardDeleteAsync(user);

            // Assert
            Assert.That(result, Is.True);
            var deletedUser = await _context.Users.FindAsync(_testUserId);
            Assert.That(deletedUser, Is.Null);
        }

        #endregion HardDeleteAsync Tests

        #region SaveChangesAsync Tests

        [Test]
        public async Task SaveChangesAsync_SavesPendingChanges()
        {
            // Arrange
            var user = new AppUser { Id = _testUserId, Email = "test@example.com" };
            await _context.Users.AddAsync(user);

            // Act
            await _repository.SaveChangesAsync();

            // Assert
            var savedUser = await _context.Users.FindAsync(_testUserId);
            Assert.That(savedUser, Is.Not.Null);
        }

        #endregion SaveChangesAsync Tests

        #region SingleOrDefaultAsync Tests

        [Test]
        public async Task SingleOrDefaultAsync_ReturnsUser_WhenOneMatches()
        {
            // Arrange
            await _repository.AddAsync(new AppUser { Id = "user1", Email = "unique@example.com" });
            await _repository.AddAsync(new AppUser { Id = "user2", Email = "other@example.com" });

            // Act
            var result = await _repository.SingleOrDefaultAsync(u => u.Email == "unique@example.com");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo("user1"));
        }

        [Test]
        public async Task SingleOrDefaultAsync_ReturnsNull_WhenNoneMatch()
        {
            // Arrange
            await _repository.AddAsync(new AppUser { Id = "user1", Email = "test@example.com" });

            // Act
            var result = await _repository.SingleOrDefaultAsync(u => u.Email == "nonexistent@example.com");

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion SingleOrDefaultAsync Tests

        #region AddRangeAsync Tests

        [Test]
        public async Task AddRangeAsync_AddsMultipleUsersSuccessfully()
        {
            // Arrange
            var users = new[]
            {
                new AppUser { Id = "user1", Email = "user1@example.com" },
                new AppUser { Id = "user2", Email = "user2@example.com" },
                new AppUser { Id = "user3", Email = "user3@example.com" }
            };

            // Act
            await _repository.AddRangeAsync(users);

            // Assert
            var allUsers = _repository.GetAllAttached().ToList();
            Assert.That(allUsers.Count, Is.EqualTo(3));
        }

        #endregion AddRangeAsync Tests
    }
}