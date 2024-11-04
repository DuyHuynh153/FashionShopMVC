using FashionShop.Models.DTO.ProductDTO;
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.ProductDTO;
using FashionShopMVC.Repositories;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace FashionShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // GET: Admin/Product
        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? searchByName = null)
        {
            var productPaginationSet = await _productRepository.GetAll(page - 1, pageSize, null, searchByName);
            ViewData["searchByName"] = searchByName ?? "";
            return View(productPaginationSet);
        }

        // GET: Admin/Product/Create
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllCategoryAsync();
            // Kiểm tra dữ liệu trước khi trả về view
            ViewBag.Categories = categories;
            return View();
        }



        // POST: Admin/Product/Create
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateProductDTO createProductDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await _productRepository.Create(createProductDTO);
                if (result != null)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            // Nếu model không hợp lệ, lấy lại danh sách danh mục
            var categories = await _categoryRepository.GetAllCategoryAsync();
            ViewBag.Categories = categories; // Đặt lại danh sách vào ViewBag
            return View(createProductDTO); // Trả về model đã nhập
        }

        /*// GET: Admin/Product/Edit/{id}
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }*/

        // POST: Admin/Product/Edit/{id}
        [HttpPost]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(UpdateProductDTO updateProductDTO, int id)
        {
            if (ModelState.IsValid)
            {
                var result = await _productRepository.Update(updateProductDTO, id);
                if (result != null)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(updateProductDTO);
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _productRepository.Delete(id);
            if (result)
            {
                return Json(new { success = true, message = "Sản phẩm đã được xóa thành công." });
            }
            return Json(new { success = false, message = "Có lỗi xảy ra khi xóa sản phẩm." });
        }
    }
}
