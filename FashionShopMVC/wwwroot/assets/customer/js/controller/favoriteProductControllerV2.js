$(document).ready(function () {
    favorite.init(); // Initialize the favorite functionality

    // Handle Add to Favorite button click event
    $(document).on('click', '.btnAddProductFavorite', function (e) {
        e.preventDefault();

        var productId = parseInt($(this).data('id'));
        favorite.addFavoriteProduct(productId);
    });

    // Handle Delete from Favorite button click event
    $(document).on('click', '.btnDeleteProductFavorite', function (e) {
        e.preventDefault();

        var productId = parseInt($(this).data('id'));
        favorite.deleteFavoriteProduct(productId);
    });
});

var favorite = {
    init: function () {
        // Initialization logic can go here if needed
    },
    addFavoriteProduct: function (productId) {
        console.log("You clicked the add to favorite with ID: " + productId);
        $.ajax({
            url: '/Account/AddFavoriteProduct',
            type: 'POST',
            data: {
                ProductID: productId
            },
            dataType: 'json',
            success: function (res) {
                if (res.status) {
                    // Handle success (e.g., update the UI, show a notification)
                    console.log("Successfully added to favorites");
                    toastr.success(res.message);
                    // Optionally, update the button UI or show a success message
                } else {
                    // Handle failure
                    console.log("Failed to add to favorites");
                    toastr.error(res.message);
                    // Optionally, show an error message
                }

            },
            error: function (xhr, status, error) {
                // Handle AJAX error
                console.error("AJAX Error: " + status + " - " + error);
            }
        });
    },
    deleteFavoriteProduct: function (productId) {
        $.ajax({
            url: '/Account/DeleteFavoriteProduct',
            type: 'POST',
            data: {
                ProductID: productId
            },
            dataType: 'json',
            success: function (res) {
                if (res.status) {
                    // Handle success (e.g., refresh the page or update the UI)
                    console.log("Successfully removed from favorites");
                    location.reload(true); // Reload the page to reflect changes
                } else {
                    // Handle failure
                    console.log("Failed to remove from favorites");
                    alert('Failed to remove from favorites');
                }
            },
            error: function (xhr, status, error) {
                // Handle AJAX error
                console.error("AJAX Error: " + status + " - " + error);
            }
        });
    }
};
