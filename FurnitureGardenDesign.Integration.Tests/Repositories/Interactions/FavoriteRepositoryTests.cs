using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Implementations.Interactions;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Tests.Integration.Repositories.Interactions
{
    [TestFixture]
    public class FavoriteRepositoryTests
    {
        private ApplicationDbContext _context;
        private FavoriteRepository _repository;

        private string _userId1;
        private string _userId2;
        private Guid _designId1;
        private Guid _designId2;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new FavoriteRepository(_context);

            _userId1 = "user-123";
            _userId2 = "user-456";
            _designId1 = Guid.NewGuid();
            _designId2 = Guid.NewGuid();

            SeedData();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void SeedData()
        {
            var designs = new[]
 {
    new CatalogDesign
    {
        Id = _designId1,
        Title = "Chair",
        Description = "Test design 1",
        Image2DUrl = "/images/test1.jpg"
    },
    new CatalogDesign
    {
        Id = _designId2,
        Title = "Table",
        Description = "Test design 2",
        Image2DUrl = "/images/test2.jpg",
    }
};

            var favorites = new[]
            {
                new Favorite
                {
                    Id = Guid.NewGuid(),
                    UserId = _userId1,
                    CatalogDesignId = _designId1,
                    IsDeleted = false
                },
                new Favorite
                {
                    Id = Guid.NewGuid(),
                    UserId = _userId1,
                    CatalogDesignId = _designId2,
                    IsDeleted = true
                },
                new Favorite
                {
                    Id = Guid.NewGuid(),
                    UserId = _userId2,
                    CatalogDesignId = _designId1,
                    IsDeleted = false
                }
            };

            _context.CatalogDesigns.AddRange(designs);
            _context.Favorites.AddRange(favorites);
            _context.SaveChanges();
        }

        #region GetByCompositeKeyAsync

        [Test]
        public async Task GetByCompositeKeyAsync_ReturnsFavorite_WhenExists()
        {
            var result = await _repository.GetByCompositeKeyAsync(_userId1, _designId1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserId, Is.EqualTo(_userId1));
            Assert.That(result.CatalogDesignId, Is.EqualTo(_designId1));
        }

        [Test]
        public async Task GetByCompositeKeyAsync_ReturnsSoftDeletedFavorite()
        {
            var result = await _repository.GetByCompositeKeyAsync(_userId1, _designId2);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsDeleted, Is.True);
        }

        [Test]
        public async Task GetByCompositeKeyAsync_ReturnsNull_WhenNotExists()
        {
            var result = await _repository.GetByCompositeKeyAsync("missing-user", Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        #endregion GetByCompositeKeyAsync

        #region ExistsAsync

        [Test]
        public async Task ExistsAsync_ReturnsTrue_WhenFavoriteExists()
        {
            var result = await _repository.ExistsAsync(_userId1, _designId1);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ExistsAsync_ReturnsFalse_WhenFavoriteIsSoftDeleted()
        {
            var result = await _repository.ExistsAsync(_userId1, _designId2);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ExistsAsync_ReturnsFalse_WhenFavoriteDoesNotExist()
        {
            var result = await _repository.ExistsAsync("missing-user", Guid.NewGuid());

            Assert.That(result, Is.False);
        }

        #endregion ExistsAsync

        #region BaseRepository Coverage

        [Test]
        public async Task AddAsync_AddsFavoriteSuccessfully()
        {
            var newFavorite = new Favorite
            {
                Id = Guid.NewGuid(),
                UserId = _userId1,
                CatalogDesignId = Guid.NewGuid()
            };

            await _repository.AddAsync(newFavorite);

            var result = await _context.Favorites.FindAsync(newFavorite.Id);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsFavorite()
        {
            var favorite = await _context.Favorites.FirstOrDefaultAsync();

            var result = await _repository.GetByIdAsync(favorite.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(favorite.Id));
        }

        [Test]
        public async Task HardDeleteAsync_RemovesFavorite()
        {
            var favorite = await _context.Favorites.FirstOrDefaultAsync();

            var result = await _repository.HardDeleteAsync(favorite);

            Assert.That(result, Is.True);

            var deleted = await _context.Favorites.FindAsync(favorite.Id);

            Assert.That(deleted, Is.Null);
        }

        #endregion BaseRepository Coverage
    }
}