using FashionShop.Repositories;
using FashionShopMVC.Areas.Admin.Models.RolesDTO;
using FashionShopMVC.Data;
using FashionShopMVC.Repositories.@interface;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class RoleController : Controller
    {
        private IRoleRepository _roleService;
        public RoleController(IRoleRepository roleService)
        {
            _roleService = roleService;          
        }

        [HttpGet]
        [Route("")]
        public async Task< IActionResult> Index()
        {

            IEnumerable<GetRoleDTO> listRoles = await _roleService.GetAllAsync();
            if (listRoles == null)
            {
                return NotFound();
            }
            return View(listRoles);

        }


        // GET: Admin/Role/Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();

        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateRoleDTO createRoleDTO)
        {
            if (ModelState.IsValid)
            {
                await _roleService.CreateAsync(createRoleDTO);
                return RedirectToAction(nameof(Index));
            }
            return View(createRoleDTO);
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPut]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id, EditRoleDTO role)
        {
            if (ModelState.IsValid)
            {
                await _roleService.UpdateAsync(role, id);
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _roleService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


    }
}
