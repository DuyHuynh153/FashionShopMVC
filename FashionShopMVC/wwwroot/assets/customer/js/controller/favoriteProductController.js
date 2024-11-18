var favorite = {
    init: function () {
        favorite.registerEvents();
    },
    registerEvents: function () {
        $('.btnAddProductFavorite').off('click').on('click', function (e) {
            e.preventDefault();

            var productId = parseInt($(this).data('id'));
            favorite.addFavoriteProduct(productId);
        });
        $('.btnDeleteProductFavorite').off('click').on('click', function (e) {
            e.preventDefault();

            var productId = parseInt($(this).data('id'));
            favorite.deleteFavoriteProduct(productId);
        })
    },
    addFavoriteProduct: function (productID) {
        console.log("you click the addEventListener with ID:" + productID);
        $.ajax({
            url: '/Account/AddFavoriteProduct',
            data: {
                ProductID: productID
            },
            type: 'POST',
            dataType: 'json',
            success: function (res) {
                if (res.status) {
                    // Success notification using Noty (client-side)
                    console.log("success")
                   
                } else {
                    // Error notification using Noty (client-side)
                    console.log("faile")
                }
            },
            
            error: function (xhr, status, error) {
                // AJAX error handling
                console.error("AJAX Error: " + status + " - " + error);
                
            }
        });
    },
    deleteFavoriteProduct: function (productID) {
        $.ajax({
            url: '/Account/DeleteFavoriteProduct',
            data: {
                ProductID: productID
            },
            type: 'POST',
            dataType: 'json',
            success: function (res) {
                if (res.status) {
                    //alert('Đã xóa sản phẩm khỏi danh sách yêu thích');
                    location.reload(true);
                }
                else {
                    alert('Lỗi');
                }
            }
        });
    }
}
$(document).ready(function () {
    console.log("ok favorite")
    favorite.init();
});
