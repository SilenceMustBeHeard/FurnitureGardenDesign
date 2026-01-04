using Furniture_GardenDesign.Data.Enums;
using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize] // само логнати потребители
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly ICategoryRepository _categoryRepository;

    public OrdersController(
        ApplicationDbContext context,
        UserManager<AppUser> userManager,
        ICategoryRepository categoryRepository)
    {
        _context = context;
        _userManager = userManager;
        _categoryRepository = categoryRepository;
    }

    // =========================
    // GET: Orders/Create
    // =========================
    public async Task<IActionResult> Create()
    {
        await LoadCategoriesAsync();

        return View(new OrderFormViewModel());
    }

    // =========================
    // POST: Orders/Create
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId!,
            FurnitureType = model.FurnitureType,
            Dimensions = model.Dimensions,
            Description = model.Description,
            ReferenceImageUrl = model.ReferenceImageUrl,
            CategoryId = model.CategoryId,
            Status = OrderStatus.Pending,
            CreatedOn = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        TempData["Message"] = "Your order has been submitted!";
        return RedirectToAction("Index", "CatalogDesigns");
    }

    // =========================
    // GET: Orders/Manage
    // =========================
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Manage()
    {
        var orders = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Category)
            .Where(o => o.Status == OrderStatus.Pending)
            .ToListAsync();

        return View(orders);
    }

    // =========================
    // Helper method
    // =========================
    private async Task LoadCategoriesAsync()
    {
        var categories = await _categoryRepository
            .GetAll()
            .Where(c => c.IsActive)
            .ToListAsync();

        ViewBag.Categories = categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        });
    }
}
