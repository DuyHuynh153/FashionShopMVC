using FashionShopMVC.Models.DTO.UserDTO;
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace FashionShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]

    public class UserController : Controller
    {

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private UserManager<User> _userManager;

        public UserController(IUserRepository userRepository, UserManager<User> userManager, IRoleRepository roleRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        [HttpGet("")]
        public async Task< IActionResult> Index()
        {
            string userName = "";
            string role =  (await _roleRepository.GetByNameAsync("Quản Trị Viên")).ID.ToString();


            var listUserAdmin = await _userRepository.GetAllUserAsync(userName, role);


            return View(listUserAdmin);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
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
                    return View(registerRequestDTO);
                }

                var checkEmail = await _userManager.FindByEmailAsync(registerRequestDTO.Email);
                if (checkEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return View(registerRequestDTO);
                }

                var result = await _userRepository.RegisterAccountAdminAsync(registerRequestDTO);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tạo tài khoản không thành công");
                    return View(registerRequestDTO);
                }
            }
            return View(registerRequestDTO);
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
            return View(user);
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
            return View(user);
        }
        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _userRepository.Delete(id);
            if (result)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Xóa không thành công");
                return View();
            }
        }

        [HttpPost]
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
