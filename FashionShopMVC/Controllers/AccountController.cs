﻿using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.UserDTO;
using FashionShopMVC.Repositories;
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
using Service;

namespace FashionShopMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly INotyfService _notyfService;

        public AccountController(UserManager<User> userManager, IUserRepository userRepository, INotyfService notyfService)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _notyfService = notyfService;
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

    }
}
