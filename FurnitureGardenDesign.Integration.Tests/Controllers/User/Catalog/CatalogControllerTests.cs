using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Implementations.Catalog;
using FurnitureGardenDesign.Data.Repository.Implementations.Interactions;
using FurnitureGardenDesign.Data.Repository.Interfaces.Catalog;
using FurnitureGardenDesign.Data.Repository.Interfaces.Interactions;
using FurnitureGardenDesign.Services.Core.Implementations.Catalog;
using FurnitureGardenDesign.Services.Core.Implementations.Interactions;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces.Catalog;
using FurnitureGardenDesign.Web.Controllers.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Tests.Integration.Controllers.Catalog
{
    [TestFixture]
    public class CatalogControllerIntegrationTests
    {
        private ServiceProvider _serviceProvider;
        private ApplicationDbContext _context;
        private UserManager<AppUser> _userManager;
        private ICatalogService _catalogService;
        private IFavoriteService _favoriteService;
        private CatalogController _controller;
        private AppUser _testUser;
        private CatalogDesign _testDesign;
        private Category _testCategory;

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

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<ICatalogRepository, CatalogRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();

            services.AddScoped<ICatalogService, CatalogService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<IReviewService, ReviewService>();

            _serviceProvider = services.BuildServiceProvider();

            _userManager = _serviceProvider.GetRequiredService<UserManager<AppUser>>();
            _catalogService = _serviceProvider.GetRequiredService<ICatalogService>();
            _favoriteService = _serviceProvider.GetRequiredService<IFavoriteService>();

            var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole("User"));

            await SeedTestData();

            _controller = new CatalogController(_catalogService, _favoriteService);
        }

        [TearDown]
        public async Task TearDown()
        {
            _controller?.Dispose();

            if (_userManager != null)
            {
                _userManager.Dispose();
            }

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
            _testCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Test Category",
                Description = "Test Description",
                IsDeleted = false
            };
            _context.Categories.Add(_testCategory);

            for (int i = 1; i <= 5; i++)
            {
                var design = new CatalogDesign
                {
                    Id = Guid.NewGuid(),
                    Title = $"Test Design {i}",
                    Description = $"Description for design {i}",
                    Image2DUrl = $"/images/design{i}.jpg",
                    Model3DUrl = i % 2 == 0 ? $"/models/design{i}.glb" : null,
                    Materials = i == 1 ? "Wood, Metal" : null,
                    Price = 99.99m * i,
                    CategoryId = _testCategory.Id,
                    IsDeleted = false,
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow
                };
                _context.CatalogDesigns.Add(design);

                if (i == 1)
                {
                    _testDesign = design;
                }
            }

            _testUser = new AppUser
            {
                Id = "user-123",
                UserName = "testuser@example.com",
                Email = "testuser@example.com",
                FirstName = "Test",
                LastName = "User"
            };
            await _userManager.CreateAsync(_testUser, "User123!");
            await _userManager.AddToRoleAsync(_testUser, "User");

            await _context.SaveChangesAsync();
        }

        private void SetUserContext(string userId, bool isAuthenticated = true)
        {
            if (!isAuthenticated)
            {
                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = null }
                };
                return;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "testuser@example.com"),
                new Claim(ClaimTypes.Email, "testuser@example.com")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        #region CatalogIndex Tests

        [Test]
        public async Task CatalogIndex_Get_AsGuest_ReturnsOnly3Designs()
        {
            SetUserContext(null, false);

            var result = await _controller.CatalogIndex(1, 9);

            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult.Model as IEnumerable<CatalogDesignViewModel>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count(), Is.EqualTo(3));
            Assert.That(viewResult.ViewData["IsGuest"], Is.EqualTo(true));
            Assert.That(viewResult.ViewData["PageSize"], Is.EqualTo(3));
        }

        [Test]
        public async Task CatalogIndex_Get_AsAuthenticatedUser_ReturnsPaginatedDesigns()
        {
            SetUserContext(_testUser.Id, true);

            var result = await _controller.CatalogIndex(1, 3);

            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult.Model as IEnumerable<CatalogDesignViewModel>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count(), Is.EqualTo(3));
            Assert.That(viewResult.ViewData["IsGuest"], Is.EqualTo(false));
            Assert.That(viewResult.ViewData["PageSize"], Is.EqualTo(3));
        }

        [Test]
        public async Task CatalogIndex_Get_AsAuthenticatedUser_SecondPage_ReturnsRemainingDesigns()
        {
            SetUserContext(_testUser.Id, true);

            var result = await _controller.CatalogIndex(2, 3);

            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult.Model as IEnumerable<CatalogDesignViewModel>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count(), Is.EqualTo(2));
        }

        #endregion

        #region Details Tests

        [Test]
        public async Task Details_Get_WhenDesignExists_ReturnsView()
        {
            SetUserContext(_testUser.Id, true);

            var result = await _controller.Details(_testDesign.Id);

            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult.Model as CatalogDesignViewModel;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(_testDesign.Id));
            Assert.That(model.Title, Is.EqualTo("Test Design 1"));
            Assert.That(model.Materials, Is.EqualTo("Wood, Metal"));
        }

        // TO DO : fix the test : implement proper set up

        //[Test]
        //public async Task Details_Get_WhenDesignNotFound_ReturnsNotFound()
        //{
        //    SetUserContext(_testUser.Id, true);

        //    var result = await _controller.Details(Guid.NewGuid());

        //    Assert.That(result, Is.TypeOf<NotFoundResult>());
        //}

        [Test]
        public async Task Details_Get_AsGuest_ReturnsView()
        {
            SetUserContext(null, false);

            var result = await _controller.Details(_testDesign.Id);

            Assert.That(result, Is.TypeOf<ViewResult>());
        }

        #endregion

        #region ToggleFavorite Tests


        // TO DO : fix the test : implement proper set up
        //[Test]
        //public async Task ToggleFavorite_Post_WhenAuthenticated_AddsFavorite()
        //{
        //    SetUserContext(_testUser.Id, true);
        //    var returnUrl = "/Catalog/Index";

        //    var result = await _controller.ToggleFavorite(_testDesign.Id, returnUrl);

        //    Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        //    var redirect = result as RedirectToActionResult;
        //    Assert.That(redirect.ActionName, Is.EqualTo("CatalogIndex"));
        //    Assert.That(_controller.TempData["Success"], Is.EqualTo("You added this design to favorites!"));

        //    var isFavorite = await _favoriteService.IsFavoriteAsync(_testUser.Id, _testDesign.Id);
        //    Assert.That(isFavorite, Is.True, "Favorite should be added");
        //}


        // TO DO : fix the test : implement proper set up
        //[Test]
        //public async Task ToggleFavorite_Post_WhenAlreadyFavorited_RemovesFavorite()
        //{
        //    SetUserContext(_testUser.Id, true);

        //    await _favoriteService.ToggleFavoriteAsync(_testUser.Id, _testDesign.Id);

        //    var wasAdded = await _favoriteService.IsFavoriteAsync(_testUser.Id, _testDesign.Id);
        //    Assert.That(wasAdded, Is.True, "Favorite should exist before removal");

        //    var result = await _controller.ToggleFavorite(_testDesign.Id, "/Catalog/Index");

        //    Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        //    Assert.That(_controller.TempData["Success"], Is.EqualTo("You removed this design from favorites."));

        //    var isFavorite = await _favoriteService.IsFavoriteAsync(_testUser.Id, _testDesign.Id);
        //    Assert.That(isFavorite, Is.False, "Favorite should be removed");
        //}


        // TO DO : fix the test : implement proper set up
        //[Test]
        //public async Task ToggleFavorite_Post_WhenUnauthenticated_RedirectsWithError()
        //{
        //    SetUserContext(null, false);

        //    var result = await _controller.ToggleFavorite(_testDesign.Id, "/Catalog/Index");

        //    Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        //    var redirect = result as RedirectToActionResult;
        //    Assert.That(redirect.ActionName, Is.EqualTo("CatalogIndex"));
        //    Assert.That(_controller.TempData["Error"], Is.EqualTo("You must be logged in to manage favorites."));

        //    var isFavorite = await _favoriteService.IsFavoriteAsync(_testUser.Id, _testDesign.Id);
        //    Assert.That(isFavorite, Is.False, "No favorite should be added for unauthenticated user");
        //}

        #endregion

        #region AddReview Tests
        // TO DO : fix the test : implement proper set up
        //[Test]
        //public async Task AddReview_Post_WithValidRating_AddsReview()
        //{
        //    SetUserContext(_testUser.Id, true);
        //    int rating = 4;
        //    string comment = "Excellent design!";

        //    var result = await _controller.AddReview(_testDesign.Id, rating, comment);

        //    Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        //    var redirect = result as RedirectToActionResult;
        //    Assert.That(redirect.ActionName, Is.EqualTo("CatalogIndex"));
        //    Assert.That(_controller.TempData["Success"], Is.EqualTo("You added a review!"));

        //    var updatedDesign = await _catalogService.GetDetailsAsync(_testDesign.Id, _testUser.Id);
        //    Assert.That(updatedDesign.ReviewCount, Is.EqualTo(1));
        //}
        // TO DO : fix the test : 

        //[Test]
        //public async Task AddReview_Post_WithRatingTooLow_ReturnsError()
        //{
        //    SetUserContext(_testUser.Id, true);
        //    int rating = 0;
        //    string comment = "Bad";

        //    var result = await _controller.AddReview(_testDesign.Id, rating, comment);

        //    Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        //    var redirect = result as RedirectToActionResult;
        //    Assert.That(redirect.ActionName, Is.EqualTo("Details"));
        //    Assert.That(redirect.RouteValues["id"], Is.EqualTo(_testDesign.Id));
        //    Assert.That(_controller.TempData["Error"], Is.EqualTo("Rating must be between 1 and 5."));
        //}

        // TO DO fix the test : implement proper set up

        //[Test]
        //public async Task AddReview_Post_WithRatingTooHigh_ReturnsError()
        //{
        //    SetUserContext(_testUser.Id, true);
        //    int rating = 6;
        //    string comment = "Perfect!";

        //    var result = await _controller.AddReview(_testDesign.Id, rating, comment);

        //    Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        //    var redirect = result as RedirectToActionResult;
        //    Assert.That(redirect.ActionName, Is.EqualTo("Details"));
        //    Assert.That(redirect.RouteValues["id"], Is.EqualTo(_testDesign.Id));
        //    Assert.That(_controller.TempData["Error"], Is.EqualTo("Rating must be between 1 and 5."));
        //}

        #endregion

        #region Authorization Tests

        [Test]
        public void CatalogIndex_HasAllowAnonymousAttribute()
        {
            var method = typeof(CatalogController).GetMethod(nameof(CatalogController.CatalogIndex));
            var attribute = method?.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).FirstOrDefault();
            Assert.That(attribute, Is.Not.Null);
        }

        [Test]
        public void Details_HasAllowAnonymousAttribute()
        {
            var method = typeof(CatalogController).GetMethod(nameof(CatalogController.Details));
            var attribute = method?.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).FirstOrDefault();
            Assert.That(attribute, Is.Not.Null);
        }

        [Test]
        public void ToggleFavorite_HasAuthorizeAttribute()
        {
            var method = typeof(CatalogController).GetMethod(nameof(CatalogController.ToggleFavorite));
            var attribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), true).FirstOrDefault();
            Assert.That(attribute, Is.Not.Null);
        }

        [Test]
        public void AddReview_HasAuthorizeAttribute()
        {
            var method = typeof(CatalogController).GetMethod(nameof(CatalogController.AddReview));
            var attribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), true).FirstOrDefault();
            Assert.That(attribute, Is.Not.Null);
        }

        #endregion
    }
}