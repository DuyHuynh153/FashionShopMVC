
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.ProductDTO;
using FashionShopMVC.Repositories;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing;
using System.Security.Claims;

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
            ViewBag.CreatedBy = User.Identity.Name;
            return View();
        }



        

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateProductDTO model)
        {
            var categories = await _categoryRepository.GetAllCategoryAsync();
            ViewBag.Categories = categories;
            // model.CreatedBy = "Duy";

            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    // Log errors for debugging
                    Console.WriteLine(error.ErrorMessage);
                }
                TempData["ErrorMessage"] = "Dữ liệu nhập vào không hợp lệ.";
                return View(model);
            }

            // Check if the CategoryID exists
            var category = await _categoryRepository.GetByIdAsync(model.CategoryID);
            if (category == null)
            {
                ModelState.AddModelError("CategoryID", "Danh mục không hợp lệ.");
                TempData["ErrorMessage"] = "Danh mục không hợp lệ.";
                return View(model);
            }

            try
            {
                // Handle main image
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(model.ImageFile.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    var newFileName = Guid.NewGuid().ToString() + fileExtension;

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products", newFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    model.ImagePath = "/img/products/" + newFileName;
                }

                // Handle additional images
                if (model.ListImageFiles != null && model.ListImageFiles.Any())
                {
                    var imagePaths = new List<string>();

                    foreach (var file in model.ListImageFiles)
                    {
                        if (file != null && file.Length > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var fileExtension = Path.GetExtension(fileName);
                            var newFileName = Guid.NewGuid().ToString() + fileExtension;

                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products", newFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            imagePaths.Add("/img/products/" + newFileName);
                        }
                    }

                    model.ListImagePaths = JsonConvert.SerializeObject(imagePaths);
                }

                // Call the Create method in the repository to save the product
                var result = await _productRepository.Create(model);

                if (result != null)
                {
                    TempData["SuccessMessage"] = "Tạo sản phẩm thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình tạo sản phẩm.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình tạo sản phẩm: " + ex.Message;
            }

            return View(model);
        }



        // GET: Admin/Product/Edit/{id}
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var categories = await _categoryRepository.GetAllCategoryAsync();
            ViewBag.Categories = categories;
            var product = await _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Admin/Product/Edit/{id}
        [HttpPost]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(UpdateProductDTO updateProductDTO, int id)
        {
            var categories = await _categoryRepository.GetAllCategoryAsync();
            ViewBag.Categories = categories;
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
