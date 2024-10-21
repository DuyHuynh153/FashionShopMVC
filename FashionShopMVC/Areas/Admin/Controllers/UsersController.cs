using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.UserDTO;
using FashionShopMVC.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public UsersController(UserManager<User> userManager, IUserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        //Get list user
        [HttpGet]
        //[AuthorizeRoles("Quản trị viên", "Nhân viên")]
        public async Task<IActionResult> GetListUsers(string? searchByName, string? filterRole)
        {
            try
            {
                var listUsers = await _userRepository.GetAllUser(searchByName, filterRole);

                return View(listUsers);
            }
            catch
            {
                ViewBag.ErrorMessage = "Lấy danh sách người dùng không thành công";
                return View();
            }
        }
        // get user by id
        [HttpGet]
        //[AuthorizeRoles("Quản trị viên", "Nhân viên")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _userRepository.GetUserById(id);

                return View(user);
            }
            catch
            {
                ViewBag.ErrorMessage = "Lấy danh sách người dùng không thành công";
                return View();
            }
        }

        [HttpPut]
        //[AuthorizeRoles("Quản trị viên")]
        public async Task<IActionResult> AccountLock(string idAccount)
        {
            try
            {
                var result = await _userRepository.AccountLock(idAccount);
                if (result == true)
                {
                    return Ok("Khóa tài khoản thành công");
                }
                else
                {
                    return BadRequest("Tài khoản này đã bị khóa");
                }
            }
            catch
            {
                return BadRequest("Thực hiện khóa không thành công");
            }
        }

        [HttpPut]
        //[AuthorizeRoles("Quản trị viên")]
        public async Task<IActionResult> AccountUnLock(string idAccount)
        {
            try
            {
                var result = await _userRepository.AccountUnlock(idAccount);
                if (result == true)
                {
                    return Ok("Mở khóa tài khoản thành công");
                }
                else
                {
                    return BadRequest("Tài khoản này không bị khóa");
                }
            }
            catch
            {
                return BadRequest("Thực hiện mở khóa không thành công");
            }
        }

        [HttpDelete("delete-user/{id}")]
        //[AuthorizeRoles("Quản trị viên")]
        public async Task<IActionResult> DeleleUser(string id)
        {
            try
            {
                var user = await _userRepository.Delete(id);
                if (user == true)
                {
                    return Ok("Xóa người dùng thành công");
                }
                else
                {
                    return BadRequest("Không tìm thấy id của người dùng");
                }
            }
            catch
            {
                return BadRequest("Xóa người dùng không thành công");
            }
        }
        [HttpPut]
        //[AuthorizeRoles("Quản trị viên")]
        public async Task<IActionResult> UpdateUser(UpdateUserDTO updateUserDTO, string id)
        {
            try
            {
                var userById = await _userManager.FindByIdAsync(id);
                var checkEmail = await _userManager.FindByEmailAsync(updateUserDTO.Email);

                if (checkEmail != null && checkEmail.Email != userById.Email)
                {
                    return BadRequest("Email đã tồn tại, vui lòng nhập email khác");
                }

                var updateUser = await _userRepository.Update(updateUserDTO, id);
                if (updateUser != null)
                {
                    return Ok("Chỉnh sửa tài khoản thành công");
                }
                else
                {
                    return BadRequest("Chỉnh sửa tài khoản không thành công");
                }
            }
            catch
            {
                return BadRequest("Chỉnh sửa tài khoản không thành công");

            }
        }
        [HttpGet]
        //[AuthorizeRoles("Quản trị viên", "Nhân viên")]
        public async Task<IActionResult> GetCountCustomer()
        {
            try
            {
                var count = await _userRepository.Count();

                return Ok(count);
            }
            catch
            {
                return BadRequest("Lỗi");
            }
        }


    }
}
