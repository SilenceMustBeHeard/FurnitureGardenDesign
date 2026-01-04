using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Admin")] // admin only
public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Category/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Category/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Category model)
    {
        if (!ModelState.IsValid)
            return View(model);

        model.Id = Guid.NewGuid(); // generate guid
        _context.Categories.Add(model);
        _context.SaveChanges();

        return RedirectToAction("Index", "CatalogDesigns"); 
    }

    // GET: Category/Index
    public IActionResult Index()
    {
        var categories = _context.Categories.ToList();
        return View(categories);
    }

    // GET: Category/Delete/{id}
    public IActionResult Delete(Guid id)
    {
        var category = _context.Categories.Find(id);
        if (category == null) return NotFound();

        _context.Categories.Remove(category);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
