
using FurnitureGardenDesign.Web.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using FurnitureGardenDesign.Services.Core.Interfaces;




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

        
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllActiveCategoriesAsync();
            return View(categories);
        }

       
  
        [HttpGet]
       
        public IActionResult Create()
        {
            return View();
        }

    
   
        [HttpPost]

        public async Task<IActionResult> Create(CategoryViewModelCreate model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.AddCategoryAsync(model);

            TempData["Success"] = "Category created successfully!";
            return RedirectToAction("Index");
        }


 
        [HttpGet]

        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _categoryService.GetCategoryForEditByIdAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            TempData["Success"] = "Category edited!";

            return View(model);
        }

  
        [HttpPost]


        public async Task<IActionResult> Edit(CategoryViewModelEdit model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.EditCategoryAsync(model.Id, model);

            TempData["Success"] = "Category edited successfully!";
            return RedirectToAction("Index");
        }


        [HttpGet]

      
    
        public async Task<IActionResult> EditList()
        {
            var categories = await _categoryService.GetAllActiveCategoriesAsync();
            return View("EditList", categories);

        }


    }
}