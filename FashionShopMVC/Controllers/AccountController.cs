using FashionShopMVC.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

//using Blogger_Common;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using FashionShopMVC.Helper;
using FashionShopMVC.Models.ViewModel;
//using FashionShopMVC.Models.DTO.FavoriteProductDTO;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using AspNetCoreHero.ToastNotification.Abstractions;
using FashionShopMVC.Models.DTO.UserDTO;
using FashionShopMVC.Repositories.@interface;
using FashionShopMVC.Service.Model;
using FashionShopMVC.Repositories;
using FashionShopMVC.Models.DTO.FavoriteProductDTO;
using FashionShopMVC.Service.Service;

namespace FashionShopMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly INotyfService _notyfService;
        private readonly IProductRepository _productRepository;
        private readonly IFavoriteProductRepository _favoriteProductRepository;
        private readonly IOrderRepository _orderRepository;


        // import emailService from class lib fashioShop.Service

        private readonly IEmailAuthService _emailAuthService;
        public AccountController(UserManager<User> userManager, IUserRepository userRepository, INotyfService notyfService,  IEmailAuthService emailAuthService, IProductRepository productRepository, IFavoriteProductRepository favoriteProductRepository, IOrderRepository orderRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _notyfService = notyfService;
            _emailAuthService = emailAuthService;
            _favoriteProductRepository = favoriteProductRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }
        public IActionResult Login()
        {
            // Lấy đường dẫn của trang trước đó (referrer)
            string referrerUrl = HttpContext.Request.Headers["Referer"].ToString();

            ViewBag.ReturnUrl = referrerUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginRequestDTO.Email);

                var checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

                if (user != null && checkPasswordResult == true)
                {
                    if (user.LockoutEnabled == true)
                    {
                        ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa.");
                    }
                    else
                    {
                        // Serialize đối tượng User thành JSON và lưu vào session
                        string userJson = JsonConvert.SerializeObject(user);
                        HttpContext.Session.SetString(CommonConstants.SessionUser, userJson);

                        _notyfService.Success("Đăng nhập thành công", 2);

                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove(CommonConstants.SessionUser);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Register()
        {
            string referrerUrl = HttpContext.Request.Headers["Referer"].ToString();

            ViewBag.ReturnUrl = referrerUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDTO registerRequestDTO, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Tạo đối tượng User mới từ thông tin trong RegisterRequestDTO
                var user = new User
                {
                    FullName = registerRequestDTO.FullName,
                    UserName = registerRequestDTO.Email,
                    Email = registerRequestDTO.Email,
                    PhoneNumber = registerRequestDTO.PhoneNumber
                   

                    // Các thuộc tính khác, nếu có
                };

                // Tạo tài khoản người dùng với mật khẩu
                var result = await _userManager.CreateAsync(user, registerRequestDTO.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Khách Hàng");
                    // generate email confirmation token (string type)
                    

                    // send email confirmation token to user email

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new {  token = token, email = user.Email }, Request.Scheme);

                    var message = new Message(new string[] { user.Email }, "Xác thực tài khoản",  confirmationLink);
                    _emailAuthService.SendAuthEmail(message);

                    _notyfService.Success($"Đăng ký tài khoản: {user.Email}, Vui lòng xác thực tài khoản qua email",5);

                    return RedirectToAction("Index", "Home");


                    // Nếu tài khoản được tạo thành công, lưu thông tin vào session
                    /* string userJson = JsonConvert.SerializeObject(user);
                     HttpContext.Session.SetString(CommonConstants.SessionUser, userJson);

                     _notyfService.Success("Đăng ký thành công", 2);

                     return Redirect(returnUrl);*/
                }
                else
                {
                    // Nếu đăng ký không thành công, hiển thị lỗi
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                _notyfService.Error("Invalid token or email.");
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                // unlock the user
                user.LockoutEnabled = false;
                await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _notyfService.Success("Xác thực email thành công", 5);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _notyfService.Error(error.Description, 5);
                    }
                }
            }
            else
            {
                _notyfService.Error("Xác thực email không thành công", 5);
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Identify()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Identify(IdentifyRequestDTO indentifyRequestDTO)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(indentifyRequestDTO.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Email này không tồn tại trong hệ thống.");
                    return View();
                }
                var otp = GenerateOtp();
                var message = new Message(new string[] {user.Email} , "Mã xác thực", $"Mã OTP của bạn là: {otp}");

                _emailAuthService.SendAuthEmail(message);

                _notyfService.Success("Mã OTP đã được gửi đến email của bạn", 5);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("OTP", otp);

                return RedirectToAction("confirmOTP", "Account");
            }
            return View();
            /*if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(indentifyRequestDTO.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Email này không tồn tại trong hệ thống.");
                    return View();
                }

                var otp = GenerateOtp();
                await _emailSender.SendEmailAsync(user.Email, "Mã xác thực của bạn", $"Mã OTP của bạn là: {otp}");
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("OTP", otp);
                //var savedEmail = HttpContext.Session.GetString("Email");
                //var savedOtp = HttpContext.Session.GetString("OTP");
                return RedirectToAction("confirmOTP", "Account");
            }
            return View();*/
        }
        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // Tạo OTP 6 chữ số
        }
        public IActionResult confirmOTP()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmOTP(OTPRequestDTO otpRequestDTO)
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.Session.GetString("OTP");

                if (token == null)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra, vui lòng thử lại.");
                    return View();
                }

                if (otpRequestDTO.OTP != token)
                {
                    ModelState.AddModelError("", "Mã OTP không chính xác.");
                    return View();
                }

                _notyfService.Success("Mã OTP chính xác", 5);
                return RedirectToAction("Resetpassword", "Account");
            }
            return View();
        }

        public IActionResult Resetpassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Resetpassword(ResetPasswordRequestDTO resetPasswordRequestDTO)
        {

            if (ModelState.IsValid)
            {
                var email = HttpContext.Session.GetString("Email");
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra, vui lòng thử lại.");
                    return View();
                }
                if (resetPasswordRequestDTO.Password != resetPasswordRequestDTO.RePassword)
                {
                    ModelState.AddModelError("", "Vui lòng nhập lại mật khẩu phải chính xác với mật khẩu mới!");
                    return View();
                }
                var result = await _userManager.RemovePasswordAsync(user); // Bỏ mật khẩu cũ (nếu cần)
                result = await _userManager.AddPasswordAsync(user, resetPasswordRequestDTO.Password);
                if (result.Succeeded)
                {
                    // Nếu tài khoản được tạo thành công, lưu thông tin vào session
                   /* string userJson = JsonConvert.SerializeObject(user);
                    HttpContext.Session.SetString(CommonConstants.SessionUser, userJson);*/


                    _notyfService.Success("Sửa mật khẩu thành công, Vui lòng đăng nhập lại", 2);

                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }
        public IActionResult Information()
        {
            return View();
        }
        public IActionResult Updatepassword()
        {
            var userJson = HttpContext.Session.GetString(CommonConstants.SessionUser);
            User user = null;

            if (userJson != null)
            {
                user = JsonConvert.DeserializeObject<User>(userJson);
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Updatepassword(UpdatepasswordDTO updatepasswordDTO)
        {
            var userJson = HttpContext.Session.GetString(CommonConstants.SessionUser);
            User user = null;

            if (userJson != null)
            {
                user = JsonConvert.DeserializeObject<User>(userJson);
            }
            // Kiểm tra dữ liệu mật khẩu
            if (ModelState.IsValid)
            {
                if (updatepasswordDTO.Password != updatepasswordDTO.RePassword)
                {
                    ModelState.AddModelError("NewPass", "Mật khẩu mới và nhập lại mật khẩu không trùng khớp.");
                    return View(user);  // Trả về view với lỗi
                }
                // làm vậy vì nó đang theo dõi hai thể hiện của đối tượng User cùng một lúc.
                var dbUser = await _userManager.FindByIdAsync(user.Id.ToString());
                // Cập nhật mật khẩu cho người dùng
                if (dbUser != null)
                {
                    // Cập nhật mật khẩu cho người dùng
                    var updateResult = await _userManager.ChangePasswordAsync(dbUser, updatepasswordDTO.OldPass, updatepasswordDTO.Password);

                    if (updateResult.Succeeded)
                    {
                        // Mật khẩu được cập nhật thành công
                        TempData["SuccessMessage"] = "Mật khẩu đã được cập nhật thành công.";
                        return RedirectToAction("Information", "Account");
                    }
                    else
                    {
                        // Nếu có lỗi trong quá trình thay đổi mật khẩu
                        foreach (var error in updateResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(user);  // Trả về view với lỗi
                    }
                }
            }

            return View(user);  // Trả về view nếu model không hợp lệ
        }

        public async Task<IActionResult> FavoriteList()
        {
            var userSession = HttpContext.Session.GetString(CommonConstants.SessionUser);
            var user = JsonConvert.DeserializeObject<User>(userSession);

            var listFavoriteProduct = await _favoriteProductRepository.GetByUserID(user.Id);

            return View(listFavoriteProduct);
        }

        [HttpPost]
        public async Task<JsonResult> AddFavoriteProduct(CreateFavoriteProductDTO createFavoriteProductDTO)
        {
            // Get user session
            var userSession = HttpContext.Session.GetString(CommonConstants.SessionUser);

            // Check if user is logged in (session contains user information)
            if (userSession != null)
            {
                var user = JsonConvert.DeserializeObject<User>(userSession);

                // Assign the logged-in user's ID to the DTO
                createFavoriteProductDTO.UserID = user.Id;

                // Check if the product is already in the user's favorites
                var existingFavoriteProduct = await _favoriteProductRepository.GetByUserID(user.Id);


                if (existingFavoriteProduct != null)
                {
                    foreach (var p in existingFavoriteProduct)
                    {
                        if (createFavoriteProductDTO.ProductID == p.ProductDTO.ID)
                        {
                            return Json(new
                            {
                                status = false,
                                message = "Sản phẩm đã có trong yêu thích"  // Product already in favorites
                            });
                        }
                    }
                    // Product already exists in favorites, return a message
                    
                }

                // Call repository method to create the favorite product record
                var favoriteProduct = await _favoriteProductRepository.Create(createFavoriteProductDTO);

                // Check if product is successfully added to the favorites
                if (favoriteProduct != null)
                {
                    // Retrieve the product's details (e.g., image URL) from the product repository
                    var product = await _productRepository.GetById(createFavoriteProductDTO.ProductID);

                    if (product != null)
                    {
                        // Notify the user about the successful addition to favorites
                        // _notyfService.Custom("<img style='height: 40px; padding-right: 10px;' src='" + product.Image + "'/> Đã thêm vào yêu thích", 2, "white");

                        // Return a successful JSON response
                        return Json(new
                        {
                            status = true,
                            message = "Sản phẩm đã được thêm vào yêu thích"  // Optional success message
                        });
                    }
                    else
                    {
                        // Handle the case where product is not found
                        return Json(new
                        {
                            status = false,
                            message = "Sản phẩm không tồn tại",
                            imageUrl = product?.Image // Send image URL to use in the client-side notification
                        });
                    }
                }
                else
                {
                    // Handle failure when the favorite product creation fails
                    return Json(new
                    {
                        status = false,
                        message = "Không thể thêm vào yêu thích, vui lòng thử lại"
                    });
                }
            }
            else
            {
                // If the user is not logged in, return an error message
                // _notyfService.Error("Vui lòng đăng nhập", 2);

                return Json(new
                {
                    status = false,
                    message = "Vui lòng đăng nhập để thêm sản phẩm vào yêu thích"
                });
            }
        }


        [HttpPost]
        public async Task<JsonResult> DeleteFavoriteProduct(int productID)
        {
            var userSession = HttpContext.Session.GetString(CommonConstants.SessionUser);

            if (userSession != null)
            {
                var user = JsonConvert.DeserializeObject<User>(userSession);

                var favoriteProduct = await _favoriteProductRepository.Delete(productID, user.Id);

                if (favoriteProduct != null)
                {
                    _notyfService.Success("Đã xóa khỏi yêu thích", 2);
                    return Json(new
                    {
                        status = true
                    });
                }
            }

            return Json(new
            {
                status = false
            });
        }

        public async Task<IActionResult> OrderList()
        {
            var userSession = HttpContext.Session.GetString(CommonConstants.SessionUser);
            var user = JsonConvert.DeserializeObject<User>(userSession);

            var order = await _orderRepository.GetByUserID(user.Id);

            return View(order);
        }

        public async Task<IActionResult> OrderDetail(int id)
        {
            var orderDetail = await _orderRepository.GetById(id);

            return View(orderDetail);
        }

        public async Task<JsonResult> OrderCancel(int id)
        {
            var orderCancel = await _orderRepository.Cancel(id);

            if (orderCancel != null)
            {
                // Tăng số lượng sản phẩm khi hủy hàng
                var increaseQuantityProduct = await _productRepository.IncreaseQuantityOrder(orderCancel);

                if (increaseQuantityProduct == true)
                {
                    _notyfService.Success("Đã hủy đơn hàng", 2);
                    return Json(new
                    {
                        status = true
                    });
                }
            }

            return Json(new
            {
                status = false
            });
        }
    }
}
