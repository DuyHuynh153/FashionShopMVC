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

namespace FashionShopMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly INotyfService _notyfService;
        private readonly IEmailSender _emailSender;
        public AccountController(UserManager<User> userManager, IUserRepository userRepository, INotyfService notyfService, IEmailSender emailSender)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _notyfService = notyfService;
            _emailSender = emailSender;
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

                        return Redirect(returnUrl);
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
                    PhoneNumber = registerRequestDTO.PhoneNumber,
                    PasswordHash = registerRequestDTO.Password,

                    // Các thuộc tính khác, nếu có
                };

                // Tạo tài khoản người dùng với mật khẩu
                var result = await _userManager.CreateAsync(user, registerRequestDTO.Password);

                if (result.Succeeded)
                {
                    // Nếu tài khoản được tạo thành công, lưu thông tin vào session
                    string userJson = JsonConvert.SerializeObject(user);
                    HttpContext.Session.SetString(CommonConstants.SessionUser, userJson);

                    _notyfService.Success("Đăng ký thành công", 2);

                    return Redirect(returnUrl);
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
                await _emailSender.SendEmailAsync(user.Email, "Mã xác thực của bạn", $"Mã OTP của bạn là: {otp}");
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("OTP", otp);
                var savedEmail = HttpContext.Session.GetString("Email");
                var savedOtp = HttpContext.Session.GetString("OTP");
                return RedirectToAction("confirmOTP", "Account");
            }
            return View();
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
                    string userJson = JsonConvert.SerializeObject(user);
                    HttpContext.Session.SetString(CommonConstants.SessionUser, userJson);

                    _notyfService.Success("Sửa mật khẩu thành công", 2);

                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }
    }
}
