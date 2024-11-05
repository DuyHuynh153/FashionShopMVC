using FashionShopMVC.Models.DTO.VoucherDTO;
using FashionShopMVC.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class VouchersController : Controller
    {
        private readonly IVoucherRepository _voucherRepository;
        public VouchersController(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        // GET: VouchersController
        [HttpGet]
        [Route("")]
        public async Task<ActionResult> Index()
        {
            var ListVoucher = await  _voucherRepository.GetAll();
            return View(ListVoucher);
        }

        // GET: VouchersController/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var voucher = await _voucherRepository.GetById(id);
            return View(voucher);
        }

        // GET: VouchersController/Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            var model = new CreateVoucherDTO();
            return View(model);
        }

        // POST: VouchersController/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateVoucherDTO createVoucherDTO)
        {
            try
            {
                if (createVoucherDTO.DiscountAmount == true && createVoucherDTO.DiscountValue <= 0)
                {
                    return BadRequest("Số tiền giảm phải lớn hơn 0");
                }

                if (createVoucherDTO.DiscountPercentage == true && (createVoucherDTO.DiscountValue <= 0 || createVoucherDTO.DiscountValue > 100))
                {
                    return BadRequest("Phần trăm giảm phải nằm trong khoảng 1 đến 100");
                }

                if (createVoucherDTO.EndDate <= createVoucherDTO.StartDate)
                {
                    return BadRequest("Ngày bắt đầu và kết thúc không hợp lệ");
                }

                var voucher = await _voucherRepository.Create(createVoucherDTO);

                if (voucher != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return BadRequest("Tạo voucher không thành công");
                }

            }
            catch
            {
                return BadRequest("Mã giảm giá đã tồn tại");
            }
        }

        // GET: VouchersController/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<ActionResult> Edit(int id)
        {
            // Giả sử bạn có phương thức để lấy voucher theo id
            var voucher = await _voucherRepository.GetById(id);

            // Chuyển đổi thành UpdateVoucherDTO hoặc trực tiếp sử dụng
            var updateVoucherDTO = new UpdateVoucherDTO
            {
                DiscountCode = voucher.discountCode,
                DiscountAmount = voucher.discountAmount,
                DiscountPercentage = voucher.discountPercentage,
                DiscountValue = voucher.discountValue,
                MinimumValue = voucher.minimumValue,
                Quantity = voucher.quantity,
                StartDate = voucher.startDate,
                EndDate = voucher.endDate,
                Describe = voucher.describe,
                Status = voucher.status,
                //UpdatedBy = User.Identity.Name // Lưu thông tin người cập nhật
            };

            return View(updateVoucherDTO);
        }

        // POST: VouchersController/Edit/5
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("Edit/{id}")]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: VouchersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VouchersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id}")]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
