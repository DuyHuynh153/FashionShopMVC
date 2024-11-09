using FashionShopMVC.Models.DTO.ProductDTO;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

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





        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateProductDTO model)
        {
            var categories = await _categoryRepository.GetAllCategoryAsync();
            ViewBag.Categories = categories;
            //var userName = HttpContext.Session.GetString("UserName");
            //string userName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            //return Json(new { success = true, message = userName });
            // Kiểm tra tính hợp lệ của Model
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    // Ghi lỗi để kiểm tra chi tiết
                    Debug.WriteLine(error.ErrorMessage);
                }
                TempData["ErrorMessage"] = "Dữ liệu nhập vào không hợp lệ.";
                return View(model);
            }
            try
            {
                // Xử lý ảnh chính
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(model.ImageFile.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    var newFileName = Guid.NewGuid().ToString() + fileExtension;

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadFiles/Images", newFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    model.ImagePath = "UploadFiles/Images/" + newFileName;  // Đảm bảo rằng đường dẫn được gán
                }

                // Xử lý danh sách ảnh phụ
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

                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadFiles/Images", newFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            imagePaths.Add("UploadFiles/Images/" + newFileName);
                        }
                    }

                    model.ListImagePaths = JsonConvert.SerializeObject(imagePaths);

                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình tạo sản phẩm: " + ex.Message;
            }

            // Gọi hàm Create trong Repository để lưu sản phẩm

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
            // Chuyển đổi từ GetProductByIdDTO sang UpdateProductDTO
            var updateProductDTO = new UpdateProductDTO
            {
                Name = product.Name,
                CategoryID = product.CategoryID,
                Quantity = product.Quantity,
                Describe = product.Describe,
                Image = product.Image,
                ListImages = product.ListImages,
                Price = product.Price,
                PurchasePrice = product.PurchasePrice,
                Discount = product.Discount,
                Status = product.Status,
            };
            return View(updateProductDTO);
        }

        // POST: Admin/Product/Edit/{id}
        [HttpPost]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(UpdateProductDTO updateProductDTO, int id)
        {
            var categories = await _categoryRepository.GetAllCategoryAsync();
            ViewBag.Categories = categories;
            // Kiểm tra tính hợp lệ của Model
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    // Ghi lỗi để kiểm tra chi tiết
                    Debug.WriteLine(error.ErrorMessage);
                }
                TempData["ErrorMessage"] = "Dữ liệu nhập vào không hợp lệ.";
                return View(updateProductDTO);
            }

            try
            {
                // Xử lý ảnh chính
                if (updateProductDTO.ImageFile != null && updateProductDTO.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(updateProductDTO.ImageFile.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    var newFileName = Guid.NewGuid().ToString() + fileExtension;

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadFiles/Images", newFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await updateProductDTO.ImageFile.CopyToAsync(stream);
                    }

                    updateProductDTO.Image = "UploadFiles/Images/" + newFileName;  // Đảm bảo rằng đường dẫn được gán
                }

                // Xử lý danh sách ảnh phụ
                if (updateProductDTO.ListImageFiles != null && updateProductDTO.ListImageFiles.Any())
                {
                    var imagePaths = new List<string>();

                    foreach (var file in updateProductDTO.ListImageFiles)
                    {
                        if (file != null && file.Length > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var fileExtension = Path.GetExtension(fileName);
                            var newFileName = Guid.NewGuid().ToString() + fileExtension;

                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadFiles/Images", newFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            imagePaths.Add("UploadFiles/Images/" + newFileName);
                        }
                    }

                    updateProductDTO.ListImages = JsonConvert.SerializeObject(imagePaths);

                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình tạo sản phẩm: " + ex.Message;
            }

            var result = await _productRepository.Update(updateProductDTO, id);
            if (result != null)
            {
                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình cập nhật sản phẩm.";
            }


            return View(updateProductDTO);
        }
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _productRepository.GetById(id);

            if (result != null)
            {
                var imageList = JsonConvert.DeserializeObject<List<string>>(result.ListImages);
                ViewBag.ListImages = imageList;

                return View(result);

            }
            return NotFound();
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
