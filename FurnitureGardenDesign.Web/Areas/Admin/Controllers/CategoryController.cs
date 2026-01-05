using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using FurnitureGardenDesign.Services.Core.Interfaces;  





namespace FurnitureGardenDesign.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // only admin
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: /Admin/Category
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllActiveCategoriesAsync();
            return View(categories);
        }

        // GET: /Admin/Category/Add
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // POST: /Admin/Category/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CategoryViewModelCreate model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.AddCategoryAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Category/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _categoryService.GetCategoryForEditByIdAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        // POST: /Admin/Category/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModelEdit model)
        {
            if (!ModelState.IsValid) return View(model);

            await _categoryService.EditCategoryAsync(model.Id, model);
            return RedirectToAction(nameof(Index));
        }


        // GET: /Admin/Category/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _categoryService.GetCategoryForEditByIdAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        // POST: /Admin/Category/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _categoryService.SoftDeleteCategoryAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
