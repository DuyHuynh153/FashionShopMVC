$(document).ready(function () {
    // Kiểm tra xem URL có kết thúc bằng "/Admin/Orders" không
    if (window.location.href.endsWith("/Admin/Orders")) {
        loadOrders(); // Tải dữ liệu lần đầu khi trang vừa tải xong

        $('#btnOrdersSearch').click(function () {
            loadOrders(); // Tải lại dữ liệu khi nhấn nút tìm kiếm
        });
    }
    if (window.location.href.endsWith("/Admin/Contacts")) {
        loadContacts(); // Tải dữ liệu lần đầu khi trang vừa tải xong
    }

    
    // Xử lý phân trang với sự kiện click động
    $(document).on('click', '.pagination a.page-link', function (e) {
        e.preventDefault(); // Ngăn hành động mặc định của thẻ a
        var page = $(this).text() - 1; // Lấy số trang từ nội dung của thẻ <a>
        loadOrders(page); // Gọi lại hàm loadOrders với trang mới
    });
});

function loadOrders(page = 0) {
    // Lấy giá trị từ các ô tìm kiếm
    var searchByID = $('#searchByID').val();
    var searchByName = $('#searchByName').val();
    var searchBySDT = $('#searchBySDT').val();
    var category = $('#category').val();

    $('#loading').show();
    // Gửi yêu cầu AJAX
    $.ajax({
        url: '/Admin/Orders/loadOrdersPartial',
        type: 'GET',
        data: {
            page: page,
            pageSize: defaultOrdersPageSize,
            typePayment: category,
            searchByID: searchByID,
            searchByName: searchByName,
            searchBySDT: searchBySDT
        },
        success: function (data) {
            $('#searchOrderResults').html(data); // Thay thế nội dung bảng bằng kết quả mới
        },
        error: function (xhr, status, error) {
            console.error("Lỗi:", error);
            $('#searchOrderResults').html('<p>Lỗi khi tải dữ liệu. Vui lòng thử lại.</p>');
        },
        complete: function () {
            // Ẩn hiệu ứng đang tải sau khi hoàn tất
            $('#loading').hide();
        }
    });
}

function loadContacts(page = 0) {
    // Lấy giá trị từ các ô tìm kiếm
    var searchByPhoneNumber = $('#searchByPhoneNumber').val();

    $('#loading').show();
    // Gửi yêu cầu AJAX
    $.ajax({
        url: '/Admin/Contacts/loadContactsPartial',
        type: 'GET',
        data: {
            page: page,
            pageSize: 5,
            searchByPhoneNumber: searchByPhoneNumber
        },
        success: function (data) {
            $('#searchContactResults').html(data); // Thay thế nội dung bảng bằng kết quả mới
        },
        error: function (xhr, status, error) {
            console.error("Lỗi:", error);
            $('#searchContactResults').html('<p>Lỗi khi tải dữ liệu. Vui lòng thử lại.</p>');
        },
        complete: function () {
            // Ẩn hiệu ứng đang tải sau khi hoàn tất
            $('#loading').hide();
        }
    });

}