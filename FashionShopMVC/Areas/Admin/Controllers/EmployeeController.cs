using FashionShopMVC.Areas.Admin.Models.UserDTO;
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.UserDTO;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace FashionShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Quản trị viên, Admin")]
    public class EmployeeController:Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;    
        private UserManager<User> _userManager;
        public EmployeeController (IUserRepository userRepository, IRoleRepository roleRepository, UserManager<User> userManager)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string searchQuery = "", int page = 1, int pageSize = 2)
        {
            string role = (await _roleRepository.GetByNameAsync("Nhân Viên")).ID.ToString();

            var listUserEmployee = await _userRepository.GetPagedUsersAdminAsync(searchQuery, role, page, pageSize);

            int totalUsers = (await _userRepository.GetAllUserAsync(searchQuery, role)).Count();

            int totalpages = (int)Math.Ceiling((decimal)totalUsers / pageSize);

            var model = new PageUserListDTO
            {
                Users = listUserEmployee,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalpages
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_IndexPartial", model);
            }
            return View(model);

            // return View(listUserEmployee);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return PartialView("_CreatePartial");
        }
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(RegisterRequestDTO registerRequestDTO)
        {
            if (ModelState.IsValid)
            {
                
                var checkEmail = await _userManager.FindByEmailAsync(registerRequestDTO.Email);
                if (checkEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return PartialView("_CreatePartial", registerRequestDTO);
                    // return View(registerRequestDTO);
                }
                var checkUserName = await _userManager.FindByNameAsync(registerRequestDTO.FullName);
                if (checkUserName != null)
                {
                    ModelState.AddModelError("UserName", "Tên đăng nhập đã tồn tại");
                    return PartialView("_CreatePartial", registerRequestDTO);
                    // return View(registerRequestDTO);
                }
                var result = await _userRepository.RegisterAccountEmployeeAsync(registerRequestDTO);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tạo Tài Khoản Nhân Viên thất bại");
                    return PartialView("_CreatePartial", registerRequestDTO);
                }
            }
            return PartialView("_CreatePartial", registerRequestDTO);
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return PartialView("_EditPartial", user);
        }
        [HttpPost]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(GetUserDTO updateUserDTO, string id)
        {
            if (ModelState.IsValid)
            {
                var result = await _userRepository.UpdateAsync(updateUserDTO, id);
                if (result != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Cập nhật Không thành công");
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
            return PartialView("_DeletePartial", user);
        }
        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _userRepository.Delete(id);
            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
            return PartialView("_DeletePartial");
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

    }
}
