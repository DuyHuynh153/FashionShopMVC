﻿using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.UserDTO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FashionShopMVC.Repositories.@interface;

namespace FashionShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class AuthenController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IRoleRepository _roleRepository;

        public AuthenController(UserManager<User> userManager, IRoleRepository roleRepository)
        {
            _userManager = userManager;
            _roleRepository = roleRepository;
        }

        [HttpGet]
        [Route("Login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Statistics", new { area = "Admin" });

            // ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [Route("Login")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            if (ModelState.IsValid)
            {
                var checkUser = await _userManager.FindByEmailAsync(loginRequestDTO.Email);

                // Check if the user exists and is not locked
                if (checkUser != null && !checkUser.LockoutEnabled)
                {
                    var checkPassword = await _userManager.CheckPasswordAsync(checkUser, loginRequestDTO.Password);

                    if (checkPassword)
                    {
                        var roles = await _userManager.GetRolesAsync(checkUser);

                        // Allowed roles
                        var allowedRoles = new List<string> { "Admin", "Quản trị viên" };

                        // Check if the user has at least one allowed role
                        if (roles.Any(role => allowedRoles.Contains(role)))
                        {
                            // Create claims
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, checkUser.FullName ?? checkUser.UserName),
                                new Claim(ClaimTypes.Email, checkUser.Email),
                            };

                            foreach (var role in roles)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, role));
                            }

                            // Create the identity and sign in
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                            };

                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                            return RedirectToAction("Index", "Statistics");
                        }
                    }

                }
                


                // If login fails, show an error
                ModelState.AddModelError(string.Empty, "Tên đăng nhập không đúng hoặc bạn không có quyền truy cập.");
            }

            return View(loginRequestDTO);
        }


        [HttpGet]
        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
