using FashionShopMVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Controllers
{
    public class VouchersController : Controller
    {
        private readonly IVoucherRepository _voucherRepositoty;

        public VouchersController(IVoucherRepository voucherRepository)
        {
            _voucherRepositoty = voucherRepository;
        }

        [HttpGet]
        public async Task<IActionResult> VoucherListView()
        {
            var ListVoucher = await _voucherRepositoty.GetAll();
            return View(ListVoucher);
        }

        //[HttpGet]
        //public async Task<IActionResult> Create(CreateVoucherDTO createVoucherDTO)
        //{

        //}


    }
}
