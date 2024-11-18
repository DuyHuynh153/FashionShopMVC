$(document).ready(function () {
    $('#editVoucherForm').on('submit', function (e) {
        e.preventDefault();  // Ngừng việc submit form mặc định

        var formData = $(this).serialize();  // Lấy dữ liệu form
        if (confirm('Bạn có chắc chắn muốn lưu thay đổi này')) {
            $.ajax({
                url: editVoucherUrl,  // Thay đổi URL cho phù hợp
                type: 'POST',
                data: formData,
                success: function (response) {
                    // Kiểm tra phản hồi
                    if (response.success) {
                        toastr.success(response.message);
                        setTimeout(function () {
                            window.location.href = indexUrl;  // Điều hướng sau một khoảng thời gian ngắn
                        }, 1000);
                    } else {
                        // Xóa thông báo lỗi cũ trước khi hiển thị lỗi mới
                        $('span.text-danger').text(''); // Xóa các thông báo lỗi trước

                        if (response.errors) {
                            $.each(response.errors, function (key, messages) {
                                // Tìm đúng thẻ <span> cho từng trường dựa trên asp-validation-for
                                let errorSpan = $(`[data-valmsg-for="${key}"]`);
                                if (errorSpan.length) {
                                    // Hiển thị lỗi
                                    errorSpan.text(messages.join(', '));
                                } else {
                                    toastr.error("loi");
                                }
                            });
                        } else {
                            toastr.error(response.message);  // Hiển thị thông báo lỗi
                        }
                    }
                },
                error: function (xhr, status, error) {
                    toastr.error(error);
                }
            });
        } else {
            toastr.error("Bạn đã hủy thao tác");
            setTimeout(function () {
                window.location.href = editVoucherUrl;  // Điều hướng sau một khoảng thời gian ngắn
            }, 1000);

        }
       
    });

    


    // Hàm để ẩn/hiển thị các trường nhập liệu

    // Gọi hàm ban đầu để đảm bảo giao diện đúng ngay từ khi trang được tải
    toggleDiscountFields();
});

function toggleDiscountFields() {
    if ($('#flexradiodefault1').prop('checked')) {
        $('#flexradiodefault2').prop('checked', false);
        $('#discountAmountDiv').show();
        $('#discountLabel').text('Số tiền giảm ₫'); // Cập nhật label
    } else if ($('#flexradiodefault2').prop('checked')) {
        $('#flexradiodefault1').prop('checked', false);
        $('#discountAmountDiv').show();
        $('#discountLabel').text('Phần trăm giảm %'); // Cập nhật label
    } else {
        // Nếu không chọn gì thì ẩn cả hai
        $('#discountAmountDiv').hide();
    }
}

