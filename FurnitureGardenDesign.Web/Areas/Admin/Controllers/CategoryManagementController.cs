using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryManagementController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryManagementController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        // GET: Admin/CategoryManagement/EditList
        [HttpGet]
        public async Task<IActionResult> EditList()
        {
            var categories = await _categoryService.GetAllCategoriesForAdminAsync();
            return View("EditList", categories);
        }

        // POST: Admin/CategoryManagement/ToggleActive/{id}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            await _categoryService.ToggleCategoryAsync(id);
            TempData["Success"] = "Category status toggled!";
            return RedirectToAction(nameof(EditList));
        }


        // GET: Admin/CategoryManagement/Create
        [HttpGet]
        public IActionResult Create() => View();


        // POST: Admin/CategoryManagement/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModelCreate model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.AddCategoryAsync(model);
            TempData["Success"] = "Category created successfully!";
            return RedirectToAction(nameof(EditList));
        }

        // GET: Admin/CategoryManagement/Edit/{id}

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _categoryService.GetCategoryForEditByIdAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }


        // POST: Admin/CategoryManagement/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModelEdit model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.EditCategoryAsync(model.Id, model);
            TempData["Success"] = "Category edited successfully!";
            return RedirectToAction(nameof(EditList));
        }
    }
}
