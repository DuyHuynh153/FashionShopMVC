using FashionShopMVC.Areas.Admin.Repo.UnitOfWork;
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.CategoriesDTO;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    //[Authorize]
    public class CategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var listCategory = categories.Select(c => new GetCategoryDTO { ID = c.ID, Name = c.Name }).ToList();
            if (listCategory == null)
            {
                return NotFound();
            }
            return View(listCategory);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateCategoryDTO addCategoryRequestDTO)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = (await _unitOfWork.Categories.GetAllAsync())
                    .FirstOrDefault(c => c.Name == addCategoryRequestDTO.Name);

                if (existingCategory != null)
                {
                    ModelState.AddModelError("", "Category already exists with the same name.");
                    return View(addCategoryRequestDTO);
                }

                var newCategory = new Category { Name = addCategoryRequestDTO.Name };
                await _unitOfWork.Categories.AddAsync(newCategory);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(addCategoryRequestDTO);
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var updateCategoryDTO = new UpdateCategoryDTO { Name = category.Name };
            return View(updateCategoryDTO);
        }

        [HttpPost]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, UpdateCategoryDTO updateCategoryDTO)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = await _unitOfWork.Categories.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    return NotFound();
                }

                existingCategory.Name = updateCategoryDTO.Name;
                _unitOfWork.Categories.Update(existingCategory);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(updateCategoryDTO);
        }

        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var getCategoryDTO = new GetCategoryDTO { ID = category.ID, Name = category.Name };
            return View(getCategoryDTO);
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
