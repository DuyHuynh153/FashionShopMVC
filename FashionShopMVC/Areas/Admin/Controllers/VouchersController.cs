using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.VoucherDTO;
using FashionShopMVC.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class VouchersController : Controller
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly UserManager<User> _userManager;
        public VouchersController(IVoucherRepository voucherRepository, UserManager<User> userManager)
        {
            _voucherRepository = voucherRepository;
            _userManager = userManager;
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
            model.CreatedBy = User.Identity.Name;
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
                if (!ModelState.IsValid) 
                { 
                    return View(createVoucherDTO);
                }
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
        [AuthorizeRoles("Quản trị viên", "Nhân viên")]
        public async Task<ActionResult> Edit(int id)
        {
            // Giả sử bạn có phương thức để lấy voucher theo id
            var user = await _userManager.GetUserAsync(User);
            var voucher = await _voucherRepository.GetById(id);
            
            // Chuyển đổi thành UpdateVoucherDTO hoặc trực tiếp sử dụng
            var updateVoucherDTO = new UpdateVoucherDTO
            {
                Id = id,
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
                UpdatedBy = User.Identity.Name // Lưu thông tin người cập nhật
            };

            return View(updateVoucherDTO);
        }

        // PUT: VouchersController/Edit/5
        [HttpPost]
        [Route("Edit/{id}")]
        [AuthorizeRoles("Quản trị viên", "Nhân viên")]
        public async Task<IActionResult> Edit(UpdateVoucherDTO updateVoucherDTO, int id)
        {
            // Kiểm tra ModelState
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState
                   .Where(x => x.Value.Errors.Count > 0) // Chỉ lấy các trường có lỗi
                   .ToDictionary(
                       x => x.Key,
                       x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                   )
                });
            }
            if(!updateVoucherDTO.DiscountAmount && !updateVoucherDTO.DiscountPercentage)
            {
                return Json(new { success = false, message = "Vui lòng chọn 1 trong 2 kiểu giảm giá." });
            }    

            // Kiểm tra điều kiện với DiscountAmount và DiscountValue
            if (updateVoucherDTO.DiscountAmount && updateVoucherDTO.DiscountValue <= 0)
            {
                return Json(new { success = false, message = "Giá trị giảm phải lớn hơn 0." });
            }

            // Kiểm tra điều kiện với DiscountPercentage và DiscountValue
            if (updateVoucherDTO.DiscountPercentage && (updateVoucherDTO.DiscountValue <= 0 || updateVoucherDTO.DiscountValue > 100))
            {
                return Json(new { success = false, message = "Phần trăm giảm giá phải nằm trong khoảng 1 - 100." });
            }

            // Kiểm tra ngày kết thúc phải sau ngày bắt đầu
            if (updateVoucherDTO.EndDate <= updateVoucherDTO.StartDate)
            {
                return Json(new { success = false, message = "Ngày kết thúc phải sau ngày bắt đầu." });
            }

            try
            {
                // Cập nhật voucher
                var voucher = await _voucherRepository.Update(updateVoucherDTO, id);
                if (voucher != null)
                {
                    return Json(new { success = true, message = "Cập nhật Voucher : "+id+" thành công"});
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy ID của voucher." });
                }
            }
            catch
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi cập nhật voucher." });
            }
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        [AuthorizeRoles("Quản trị viên")]
        public async Task<IActionResult> Delele(int id)
        {
            try
            {
                var voucher = await _voucherRepository.Delete(id);
                if (voucher == true)
                {
                    return Ok("Xóa voucher thành công");
                }
                else
                {
                    return BadRequest("Không tìm thấy id của voucher");
                }
            }
            catch
            {
                return BadRequest("Xóa voucher không thành công");
            }
        }
    }
}
