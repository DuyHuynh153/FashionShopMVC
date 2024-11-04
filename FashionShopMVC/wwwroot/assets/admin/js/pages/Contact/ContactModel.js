$(document).ready(function () {
    if (window.location.href.endsWith("/Admin/Contacts")) {
        loadContacts(); // Tải dữ liệu lần đầu khi trang vừa tải xong
    }
});

function loadContacts(page = 0) {
    // Lấy giá trị từ các ô tìm kiếm
    var searchByPhoneNumber = $('#searchBySDT').val();

    $('#loading').show();
    // Gửi yêu cầu AJAX
    $.ajax({
        url: '/Admin/Contacts/loadContactsPartial',
        type: 'GET',
        data: {
            page: page,
            pageSize: 5,
            searchByPhoneNumber: searchByPhoneNumber,
        },
        success: function (data) {
            $('#searchContactResults').html(data); // Thay thế nội dung bảng bằng kết quả mới
        },
        error: function (xhr, status, error) {
            console.error("Lỗi:", error);
            toastr.error("Lỗi tải dữ liệu vui lòng thử lại!", "Lỗi");
            $('#searchContactResults').html('<p>Lỗi khi tải dữ liệu. Vui lòng thử lại.</p>');
        },
        complete: function () {
            // Ẩn hiệu ứng đang tải sau khi hoàn tất
            $('#loading').hide();
        }
    });
}