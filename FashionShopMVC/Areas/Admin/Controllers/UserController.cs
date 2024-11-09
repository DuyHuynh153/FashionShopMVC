using FashionShopMVC.Models.DTO.UserDTO;
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FashionShopMVC.Models;
using System.Diagnostics;
using FashionShopMVC.Areas.Admin.Models.UserDTO;
using AspNetCoreHero.ToastNotification.Abstractions;


namespace FashionShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class UserController : Controller
    {

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private UserManager<User> _userManager;

        private readonly ITokenRepository _tokenRepository;

        private readonly INotyfService _notyfService;

        public UserController(IUserRepository userRepository, UserManager<User> userManager, IRoleRepository roleRepository, ITokenRepository tokenRepository, INotyfService notyfService)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _tokenRepository = tokenRepository;
            _notyfService = notyfService;
        }


        [HttpGet("")]
        public async Task< IActionResult> Index(string searchQuery="", int page = 1, int pageSize = 2)
        {
            // string userName = "";
            string role =  (await _roleRepository.GetByNameAsync("Quản Trị Viên")).ID.ToString();

            //  Fetch paginated and filtered list of users

            var listUserAdmin = await _userRepository.GetPagedUsersAdminAsync(searchQuery, role, page, pageSize);
            // var listUserAdmin = await _userRepository.GetAllUserAsync(searchQuery, role);

            int totalUsers = (await _userRepository.GetAllUserAsync(searchQuery, role)).Count();

            int totalpages = (int)Math.Ceiling((decimal)totalUsers / pageSize);

            var model = new PageUserListDTO
            {
                Users = listUserAdmin,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalpages
            };
            
            // var listUserAdmin = await _userRepository.GetAllUserAsync(userName, role);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_UserListPartial", model);
            }
            return View(model);

            // using ajax

            // return PartialView("_UserListPartial", listUserAdmin);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return PartialView("_CreateUserPartial");
        }

        [HttpPost]
        [Route("Create")]

        public async Task<IActionResult> Create(RegisterRequestDTO registerRequestDTO)
        {
            if (ModelState.IsValid)
            {
                if (registerRequestDTO.Password != registerRequestDTO.RePassword)
                {
                    ModelState.AddModelError("RePassword", "Mật khẩu không khớp");
                    return PartialView("_CreateUserPartial",registerRequestDTO );
                    // return View(registerRequestDTO);
                }

                var checkEmail = await _userManager.FindByEmailAsync(registerRequestDTO.Email);
                if (checkEmail != null)
                {
                    // ModelState.AddModelError("Email", "Email đã tồn tại");
                    _notyfService.Error("Email đã tồn tại", 5);

                    return PartialView("_CreateUserPartial", registerRequestDTO);
                    // return View(registerRequestDTO);
                }

                var result = await _userRepository.RegisterAccountAdminAsync(registerRequestDTO);
                if (result)
                {

                    _notyfService.Success("Tạo tài khoản thành công", 5);
                    return RedirectToAction(nameof(Index));
                    /*string role = (await _roleRepository.GetByNameAsync("Quản Trị Viên")).ID.ToString();

                    var listUserAdmin = await _userRepository.GetPagedUsersAdminAsync("", role, 1, 2); // Reset to first page
                    var totalUsers = (await _userRepository.GetAllUserAsync("", role)).Count();
                    var totalpages = (int)Math.Ceiling((decimal)totalUsers / 2);
                    var model = new PageUserListDTO
                    {
                        Users = listUserAdmin,
                        CurrentPage = 1,
                        PageSize = 2,
                        TotalPages = totalpages
                    };

                    return PartialView("_UserListPartial", model);*/
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tạo tài khoản không thành công");
                    return PartialView("_CreateUserPartial", registerRequestDTO);
                    // return View(registerRequestDTO);
                }
            }
            return PartialView("_CreateUserPartial", registerRequestDTO);
            //  return View(registerRequestDTO);
        }


        [HttpGet]
        [Route("Edit/{id}")]

        public async Task< IActionResult> Edit(string id)
        {

            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return PartialView("_EditUserPartial", user);

            // return View(user);
        }

        [HttpPost]
        [Route("Edit/{id}")]

        public async Task<IActionResult> Edit(UpdateUserDTO updateUserDTO, string id)
        {
            if (ModelState.IsValid)
            {
                var result = await _userRepository.UpdateAsync(updateUserDTO, id);
                if (result != null)
                {
                    _notyfService.Success("Chỉnh sửa thành công", 5);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Cập nhật không thành công");
                    return View(updateUserDTO);
                }
            }
            return View(updateUserDTO);
        }

        [HttpGet]
        [Route("Delete/{id}")]

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return PartialView("_DeleteUserPartial", user);
        }
        [HttpPost]
        [Route("Delete/{id}")]

        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _userRepository.Delete(id);


            if (result)
            {
                _notyfService.Success("Xóa thành công", 5);
                return RedirectToAction("Index");
                
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Xóa không thành công");
                return PartialView("_DeleteUserPartial");
            }
        }

        [HttpPost]
        [Route("ToggleLockAccount/{id}")]
        public async Task<IActionResult> ToggleLockingAccount(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var result = await _userRepository.AccountLock(id); 
            if (result)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Khóa tài khoản không thành công");
                return RedirectToAction("Index");
            }
        }

        // CUSTOMERS

        [HttpGet]
        [Route("loadCustomersPartial")]
        //[AuthorizeRoles("Quản trị viên", "Nhân viên")]
        public async Task<IActionResult> loadCustomersPartial(int page, int pageSize, string? searchByName, string? phoneNumber, string? email)
        {
            try
            {
                string role = (await _roleRepository.GetByNameAsync("Khách Hàng")).ID.ToString();

                var listCustomers = await _userRepository.GetAllCustomerAsync(page, pageSize, searchByName, role, phoneNumber, email);

                return PartialView("_CustomerSearchResults", listCustomers);
            }
            catch
            {
                return PartialView("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        [Route("GetListCustomers")]
        public IActionResult GetListCustomers()
        {
            return View();
        }

        [HttpPost]
        [Route("LockCustomer")]
        public async Task<IActionResult> LockCustomer(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var result = await _userRepository.AccountLock(id);
            if (result)
            {
                return RedirectToAction("GetListCustomers");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Khóa tài khoản không thành công");
                return RedirectToAction("GetListCustomers");
            }
        }
    }
}
