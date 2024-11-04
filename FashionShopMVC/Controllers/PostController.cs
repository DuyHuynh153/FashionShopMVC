using FashionShopMVC.Data;
<<<<<<< HEAD
using FashionShopMVC.Models.DTO;
=======
using FashionShopMVC.Models.DTO.PostDTO;
>>>>>>> huyhuynh Post
using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Controllers
{
    public class PostController : Controller
    {
        private readonly FashionShopDBContext _dbContext;
        public PostController(FashionShopDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var allPostDTO = _dbContext.Posts
            .Select(Post => new PostDTO()
            {
                ID = Post.ID,
                Title = Post.Title,
                Image = Post.Image,
                Content = Post.Content,
                Status = Post.Status,
            })
            .ToList();

            return View(allPostDTO);
        }
        public IActionResult Detail(int id)
        {
            // Lấy thông tin bài đăng dựa vào id từ cơ sở dữ liệu
            var post = _dbContext.Posts.FirstOrDefault(p => p.ID == id);

            if (post == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy bài đăng
            }

            var postDTO = new PostDTO()
            {
                ID = post.ID,
                Title = post.Title,
                Image = post.Image,
                Content = post.Content,
                Status = post.Status,
            };

            return View(postDTO); // Truyền dữ liệu vào view chi tiết
        }
    }
}
