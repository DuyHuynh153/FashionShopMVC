using FashionShopMVC.Models.DTO.VoucherDTO;

namespace FashionShopMVC.Models.ViewModel
{
    public class VoucherUpdateViewModel
    {
        public GetVoucherDTO OldVoucher { get; set; } // Thông tin cũ
        public UpdateVoucherDTO NewVoucher { get; set; } // Thông tin mới (dùng để cập nhật)
    }
}
