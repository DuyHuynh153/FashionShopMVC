function deleteVoucher(id) {
    if (confirm('Bạn có chắc chắn muốn xóa mã giảm giá mã: ' + id + ' này?')) {
        $.ajax({
            url: '/Admin/Vouchers/Delete/' + id,
            type: 'DELETE',
            success: function (response) {
                toastr.success('Xóa thành công!');
                setTimeout(function () {
                    window.location.href = index;
                }, 1000)
            },
            error: function (error) {
                toastr.error('Có lỗi xảy ra, vui lòng thử lại!');
            }
        });
    }
}