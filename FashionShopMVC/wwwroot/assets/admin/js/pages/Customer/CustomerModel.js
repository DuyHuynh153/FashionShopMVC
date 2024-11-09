$(document).ready(function () {
    if (window.location.href.endsWith("/Admin/User/GetListCustomers")) {
        loadCustomers(); // Tải dữ liệu lần đầu khi trang vừa tải xong

        $('#btnCustomersSearch').click(function () {
            loadCustomers(); // Tải lại dữ liệu khi nhấn nút tìm kiếm
        });

        $(document).on('click', '.pagination a.page-link', function (e) {
            e.preventDefault(); // Ngăn hành động mặc định của thẻ a
            var page = $(this).text() - 1; // Lấy số trang từ nội dung của thẻ <a>
            loadCustomers(page); // Gọi lại hàm loadOrders với trang mới
        });
    }
});

function loadCustomers(page = 0) {
    // Lấy giá trị từ các ô tìm kiếm
    var searchByPhoneNumber = $('#searchBySDT').val();
    var searchByEmail = $('#searchByEmail').val();
    var searchName = $('#searchByName').val();
    $('#loading').show();
    // Gửi yêu cầu AJAX
    $.ajax({
        url: '/Admin/User/loadCustomersPartial',
        type: 'GET',
        data: {
            page: page,
            pageSize: 5,
            searchByName: searchName,
            phoneNumber: searchByPhoneNumber,
            email: searchByEmail,
        },
        success: function (data) {
            $('#searchCustomerResults').html(data); // Thay thế nội dung bảng bằng kết quả mới
            //toastr.success("Tải dữ liệu thành công!", "Thông báo");
        },
        error: function (xhr, status, error) {
            console.error("Lỗi:", error);
            toastr.error("Lỗi tải dữ liệu vui lòng thử lại!", "Lỗi");
            $('#searchCustomerResults').html('<p>Lỗi khi tải dữ liệu. Vui lòng thử lại.</p>');
        },
        complete: function () {
            // Ẩn hiệu ứng đang tải sau khi hoàn tất
            $('#loading').hide();
        }
    });
}