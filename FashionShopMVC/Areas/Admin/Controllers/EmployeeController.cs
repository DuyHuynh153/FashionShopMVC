using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.UserDTO;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
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
        public async Task<IActionResult> Index()
        {
            string userName = "";
            string role = (await _roleRepository.GetByNameAsync("Nhân Viên")).ID.ToString();
            var listUserEmployee = await _userRepository.GetAllUserAsync(userName, role);
            return View(listUserEmployee);
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
                
                var checkEmail = await _userManager.FindByEmailAsync(registerRequestDTO.Email);
                if (checkEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return View(registerRequestDTO);
                }
                var checkUserName = await _userManager.FindByNameAsync(registerRequestDTO.FullName);
                if (checkUserName != null)
                {
                    ModelState.AddModelError("UserName", "Tên đăng nhập đã tồn tại");
                    return View(registerRequestDTO);
                }
                var result = await _userRepository.RegisterAccountEmployeeAsync(registerRequestDTO);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(registerRequestDTO);
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
                    return RedirectToAction(nameof(Index));
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
                return RedirectToAction(nameof(Index));
            }
            return View();
        }


    }
}
