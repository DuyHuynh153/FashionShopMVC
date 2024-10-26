using FashionShopMVC.Models.DTO.VoucherDTO;
using FashionShopMVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Areas.Admin.Controllers
{
    public class Vouchers11Controller1 : Controller
    {
        private readonly IVoucherRepository _voucherRepositoty;


        public Vouchers11Controller1(IVoucherRepository voucherRepository)
        {
            _voucherRepositoty = voucherRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult VoucherAddView()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateVoucher(CreateVoucherDTO createVoucherDTO)
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

                var voucher = await _voucherRepositoty.Create(createVoucherDTO);

                if (voucher != null)
                {
                    return Ok(voucher);
                }
                else
                {
                    return BadRequest("Tạo voucher không thành công");
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi trong quá trình xử lý: {ex.Message}");
            }
        }

        [HttpGet]
        //[AuthorizeRoles("Quản trị viên", "Nhân viên")]
        public async Task<IActionResult> VoucherListView()
        {
            var ListVoucher = await _voucherRepositoty.GetAll();
            return View(ListVoucher);
        }

        [HttpGet]
        public async Task<IActionResult> GetVoucherById(int id)
        {
            try
            {
                var voucher = await _voucherRepositoty.GetById(id);
                if (voucher != null)
                {
                    return Ok(voucher);
                }
                else
                {
                    return BadRequest("Không tìm thấy id của voucher");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Lấy id voucher không thành công: {ex.Message}");
            }
        }
        [HttpPut]
        public async Task<IActionResult> updateVoucher(UpdateVoucherDTO updateVoucherDTO, int id)
        {
            try
            {
                if (updateVoucherDTO.DiscountAmount == true && updateVoucherDTO.DiscountValue <= 0)
                {
                    return BadRequest("Số tiền giảm phải lớn hơn 0");
                }

                if (updateVoucherDTO.DiscountPercentage == true && (updateVoucherDTO.DiscountValue <= 0 || updateVoucherDTO.DiscountValue > 100))
                {
                    return BadRequest("Phần trăm giảm phải nằm trong khoảng 1 đến 100");
                }

                if (updateVoucherDTO.EndDate <= updateVoucherDTO.StartDate)
                {
                    return BadRequest("Ngày bắt đầu và kết thúc không hợp lệ");
                }

                var voucher = await _voucherRepositoty.Update(updateVoucherDTO, id);
                if (voucher != null)
                {
                    return Ok(voucher);
                }
                else
                {
                    return BadRequest("Không tìm thấy id của voucher");
                }
            }
            catch
            {
                return BadRequest("Mã giảm giá đã tồn tại");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleleVoucher(int id)
        {
            try
            {
                var voucher = await _voucherRepositoty.Delete(id);
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

        [HttpGet]
        public async Task<IActionResult> GetVoucherCount()
        {
            try
            {
                var voucherCount = await _voucherRepositoty.Count();
                return Ok(voucherCount);
            }
            catch
            {
                return BadRequest("Lỗi");
            }
        }

    }
}
