using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using FurnitureGardenDesign.Services.Core.Interfaces;





namespace FurnitureGardenDesign.Web.Controllers
{
   

    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: /Category
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllActiveCategoriesAsync();
            return View(categories);
        }

        // GET: /Category/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModelCreate model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.AddCategoryAsync(model);

            TempData["SuccessMessage"] = "Category created successfully!";
            return RedirectToAction("Index");
        }
    }
}