

var purchase 
{
    init: function () {
        this.registerEvent();
      
    },
    registerEvent: function()
    {
        $('.btnDeleteItem').off('click').on('click', function (e)
        {
            e.preventDefault(); // Chặn hành động mặc định của nút (nếu có)

            // Lấy productId từ thuộc tính data-id của nút nhấn
            var productId = $(this).data('id');

            // Xác nhận trước khi xóa
            if (confirm('Are you sure you want to delete this item?')) {
                // Gửi AJAX đến controller
                $.ajax({
                    url: '/Cart/DeleteItem', // URL của controller và action
                    type: 'POST', // Phương thức gửi là POST
                    data: { productId: productId }, // Dữ liệu gửi đi (ID của sản phẩm)
                    success: function (response) {
                        if (response.success) {
                            alert('Product deleted successfully!');
                            // Loại bỏ sản phẩm khỏi giao diện người dùng
                            $('button[data-id="' + productId + '"]').closest('li').remove();
                        } else {
                            alert('Error: ' + response.message);
                        }
                    },
                    error: function (xhr, status, error) {
                        console.log('Error:', error);
                        alert('There was an error processing your request.');
                    }
                });
            }
        });

    }

}
purchase.init();