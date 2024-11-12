using FashionShopMVC.Models.DTO.PostDTO;
using FashionShopMVC.Models.DTO.ProductDTO;
using FashionShopMVC.Repositories;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FashionShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        public PostController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        // GET: Admin/Post
        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? searchByName = null)
        {
            var postPaginationSet = await _postRepository.GetAll(page - 1, pageSize, searchByName);
            return View(postPaginationSet);
        }
        // GET: Admin/Post/Create
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreatePostDTO model)
        {
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

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadFiles/Posts", newFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    model.Image = "UploadFiles/Posts/" + newFileName;  // Đảm bảo rằng đường dẫn được gán
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình tạo sản phẩm: " + ex.Message;
            }

            // Gọi hàm Create trong Repository để lưu sản phẩm

            var result = await _postRepository.Create(model);

            if (result != null)
            {
                TempData["SuccessMessage"] = "Tạo bài viết thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình tạo bài viết.";
            }

            return View(model);
        }
        // GET: Admin/Post/Edit/{id}
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _postRepository.GetById(id);

            if (post == null)
            {
                return NotFound();
            }
            // Chuyển đổi từ PostDTO sang UpdatePostDTO
            var updatePostDTO = new UpdatePostDTO
            {
                Title = post.Title,
                Image = post.Image,
                Content = post.Content,
                Status = post.Status,
            };
            return View(updatePostDTO);
        }
        // POST: Admin/Post/Edit/{id}
        [HttpPost]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(UpdatePostDTO updatePostDTO, int id)
        {
            // Kiểm tra tính hợp lệ của Model
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    // Ghi lỗi để kiểm tra chi tiết
                    Debug.WriteLine(error.ErrorMessage);
                }
                TempData["ErrorMessage"] = "Dữ liệu nhập vào không hợp lệ.";
                return View(updatePostDTO);
            }

            try
            {
                // Xử lý ảnh chính
                if (updatePostDTO.ImageFile != null && updatePostDTO.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(updatePostDTO.ImageFile.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    var newFileName = Guid.NewGuid().ToString() + fileExtension;

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadFiles/Posts", newFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await updatePostDTO.ImageFile.CopyToAsync(stream);
                    }

                    updatePostDTO.Image = "UploadFiles/Posts/" + newFileName;  // Đảm bảo rằng đường dẫn được gán
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình tạo bài viết: " + ex.Message;
            }

            var result = await _postRepository.Update(updatePostDTO, id);
            if (result != null)
            {
                TempData["SuccessMessage"] = "Cập nhật bài viết thành công!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình cập nhật bài viết.";
            }


            return View(updatePostDTO);
        }
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _postRepository.Delete(id);
            if (result)
            {
                return Json(new { success = true, message = "Sản phẩm đã được xóa thành công." });
            }
            return Json(new { success = false, message = "Có lỗi xảy ra khi xóa sản phẩm." });
        }
    }
}
