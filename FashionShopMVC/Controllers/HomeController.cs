using AspNetCoreHero.ToastNotification.Abstractions;
using FashionShopMVC.Repositories;
using FashionShopMVC.Helper;
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.FavoriteProductDTO;
using FashionShopMVC.Models.DTO.ProductDTO;
using FashionShopMVC.Repositories;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FashionShopMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFavoriteProductRepository _favoriteProductRepository;
        private readonly INotyfService _notyfService;

        private const int pageSize = 7;
        public HomeController(IProductRepository productRepository, ICategoryRepository categoryRepository, IFavoriteProductRepository favoriteProductRepository, INotyfService notyfService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _favoriteProductRepository = favoriteProductRepository;
            _notyfService = notyfService;
        }
        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            List<GetProductDTO> products = _productRepository.GetAll();
            int totalProducts = products.Count;
            var paginationProduct = products.Skip((page - 1 ) * pageSize).Take(pageSize).ToList();
            ViewBag.CurrentPage = page;

            ViewBag.totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductListPartial", paginationProduct);
            }
            return View(paginationProduct);
        }

        public JsonResult GetListProductByName(string keyword)
        {
            var listProductByName = _productRepository.GetAllByName(keyword);

            return Json(new
            {
                data = listProductByName,
            });
        }

        [HttpPost]
        public IActionResult SearchProductByName(string keyword)
        {
            ViewBag.keyword = keyword;

            var listProductByName = _productRepository.GetAll(keyword);

            return View(listProductByName);
        }

        public async Task<JsonResult> AddFavoriteProduct(CreateFavoriteProductDTO createFavoriteProductDTO)
        {
            var userSession = HttpContext.Session.GetString(CommonConstants.SessionUser);

            if (userSession != null)
            {
                var user = JsonConvert.DeserializeObject<User>(userSession);

                createFavoriteProductDTO.UserID = user.Id;

                var favoriteProduct = await _favoriteProductRepository.Create(createFavoriteProductDTO);

                if (favoriteProduct != null)
                {
                    var product = await _productRepository.GetById(createFavoriteProductDTO.ProductID);

                    _notyfService.Custom("<img style='height: 40px; padding-right: 10px;' src='" + product.Image + "'/> Đã thêm vào yêu thích", 2, "white");

                    return Json(new
                    {
                        status = true
                    });
                }
            }

            _notyfService.Error("Vui lòng đăng nhập", 2);
            return Json(new
            {
                status = false
            });
        }
    }
}
