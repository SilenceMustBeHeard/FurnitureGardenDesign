using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FurnitureGardenDesign.Services.Core.Interfaces;

namespace FurnitureGardenDesign.Web.Areas.Admin.Controllers
{

[Authorize(Roles = "Admin")] // admin only
[Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid) 
                return View(model);

            await _categoryService.CreateCategoryAsync(model);
            TempData["Success"] = "Category created successfully!";
            return RedirectToAction(nameof(Index));

        }
    }


}