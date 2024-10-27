using FashionShopMVC.Helper;
using FashionShopMVC.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class ContactsController : Controller
    {
        private readonly IContactRepository _contactRepository;

        public ContactsController(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        // GET: /Contact/GetAll
        [HttpGet]
        [Route("")]
        public IActionResult index()
        {
            return View();
        }

        [HttpGet]
        [Route("loadContactsPartial")]
        public IActionResult loadContactsPartial(int page = 0, int pageSize = PaginationConfig.DefaultContactsPageSize, string? searchByPhoneNumber = null)
        {
            var allContacts = _contactRepository.GetAllContact(page, pageSize, searchByPhoneNumber);
            return PartialView("_ContactSearchResults",allContacts);
        }

        // PUT: /Contact/CofirmContact/5
        [HttpPost]
        [Route("confirmContact/{id}")]
        public IActionResult confirmContact(int id)
        {
            var check = _contactRepository.Confirm(id);
            if (check)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
