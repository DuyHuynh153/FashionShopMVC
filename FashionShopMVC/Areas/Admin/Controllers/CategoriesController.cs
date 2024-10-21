

using FashionShopMVC.Models.DTO.CategoriesDTO;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
            
        }

        [HttpGet]
        [Route("")]
        public async Task< IActionResult> Index()
        {
            var listCategory = await _categoryRepository.GetAllCategoryAsync();
            if (listCategory == null)
            {
                return NotFound();
            }
            return View(listCategory);
        }

        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [Route("Create")]
        public async Task< IActionResult> Create(CreateCategoryDTO addCategoryRequestDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryRepository.AddCategoryAsync(addCategoryRequestDTO);
                if (result != null)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(addCategoryRequestDTO);
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task< IActionResult> Edit(int id) {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, UpdateCategoryDTO updateCategoryDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryRepository.UpdateByIdAsync(id, updateCategoryDTO);
                if (result != null)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(updateCategoryDTO);
        }

        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id) {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _categoryRepository.DeleteByIdAsync(id);
            if (result != null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }


    }
}
