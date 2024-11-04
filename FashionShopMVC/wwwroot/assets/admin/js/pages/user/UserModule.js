const UserModule = (() => {
    function init() {
        // Attach events or initialize user-specific features
        $('a[data-url]').on('click', function (e) {
            e.preventDefault();
            var url = $(this).data('url');
            loadUserContent(url); // Pass the correct URL
        });

        $(document).on('click', '.edit-btn', loadEditForm);
        $(document).on('click', '.create-btn', loadCreateForm);
        $(document).on('submit', '#createUserForm', submitCreateForm);
        $(document).on('submit', '#editUserForm', submitEditForm);


        $(document).on('click', '.delete-btn', loadDeleteConfirmation);
        $(document).on('submit', '#deleteUserForm', submitDeleteForm);

        // Handle cancel button for delete confirmation
        $(document).on('click', '#cancelDelete', function () {
            window.history.back(); // Go back to the previous page
        });

        // Pagination click event handling
        $(document).on('click', '.pagination a.page-link', function (e) {
            e.preventDefault();

            $('.pagination a.page-link').addClass('disabled').prop('disabled', true);

            var page = $(this).data('page'); // Get page number from pagination link
            var url = '/Admin/User'; // Get the URL from the pagination link
            loadUserContent(url, page); // Load the user list for the selected page
        });

        // Attach event for the toggle lock button
        $(document).on('click', '.toggle-lock-btn', function () {
            var userId = $(this).data('id');
            toggleLockAccount( userId); // Call the toggle lock function with the user ID
        });

        $(document).on('click', '.search-user-btn', function () {

            var searchQuery = $('#searchInput').val();

            console.log("you click search button, searchName: " + searchQuery);

            loadUserContent('/Admin/User', 1, searchQuery);
        });

        
        /*$('#searchInput').on('keypress', function (e) {
            if (e.which === 13) { // Enter key
                e.preventDefault();
                $('#searchBtn').click(); // Trigger search button click
            }
        });*/
    }

    function loadUserContent(url,  page = 1, searchQuery = '') {
        // e.preventDefault();
        $('#loading').show();

        console.log("the page you select: ", page);

        // var url = $(this).data('url');
        $.ajax({
            url: url,
            type: 'GET',
            data: {
                page: page,
                pageSize: 2,
                searchQuery: searchQuery // just empty now
            },
            success: function (data) {
                
                $('#ajax-content').html(data);
            },
            error: function () {
                $('#ajax-content').html('<p>Error loading content.</p>');
            },

            complete: function () {
                $('#loading').hide();

                $('.pagination a.page-link').removeClass('disabled').prop('disabled', false);

            }
        });
    }

    function loadCreateForm() {
        console.log("You click create user button.");
        $.ajax({
            url: '/Admin/User/Create',
            type: 'GET',
            success: function (data) {
                $('#ajax-content').html(data);
            },
            error: function () {
                alert("Failed to load create user form.");
            }
        });
    }

    function submitCreateForm(e) {
        e.preventDefault();

        var form = $(this);

        $.ajax({
            url: '/Admin/User/Create',
            type: 'POST',
            data: form.serialize(),
            success: function (response) {
                if (response.success) {
                    // Call function to reload the user list
                    // Replace the user list table with the new partial view data
                    loadUserContent(url = '/Admin/User');

                } else {
                    // Handle validation errors or other messages from the server
                    $('#ajax-content').html(response);
                }
            },
            error: function () {
                alert("Failed to create user.");
            }
        })
    }



    function loadEditForm() {
        var userId = $(this).data('id');

        console.log("You lick edit user button. User ID: " + userId + ".")
        $.ajax({
            url: '/Admin/User/Edit/' + userId,
            type: 'GET',
            success: function (data) {
                console.log(data);
                $('#ajax-content').html(data);
            },
            error: function () {
                alert("Failed to load edit form.");
            }
        });
    }

    function submitEditForm(e) {
        e.preventDefault();
        var form = $(this);
        var userId = form.find('input[name="id"]').val();
        $.ajax({
            url: '/Admin/User/Edit/' + userId,
            type: 'POST',
            data: form.serialize(),
            success: function (response) {
                if (response.success) {
                    // Call function to reload the user list
                    loadUserContent('/Admin/User'); // Reload the user list after edit
                } else {
                    // Handle validation errors or other messages from the server
                    $('#ajax-content').html(response);
                }
            },
            error: function () {
                alert("Failed to update user.");
            }
        });
    }


    function loadDeleteConfirmation() {

        console.log("You click delete user button.");
        var userId = $(this).data('id');
        $.ajax({
            url: '/Admin/User/Delete/' + userId,
            type: 'GET',
            success: function (data) {
                // console.log( "delete data: ", data);
                $('#ajax-content').html(data); // Load the delete confirmation partial view
            },
            error: function () {
                alert("Failed to load delete confirmation.");
            }
        });
    }

    function submitDeleteForm(e) {
        e.preventDefault(); // Prevent default form submission
        var form = $(this); // Current form context
        var userId = $(this).data('id');

        $.ajax({
            url: '/Admin/User/Delete/' + userId,
            type: 'POST',
            data: form.serialize(), // Serialize the form data
            success: function (response) {
                if (response.success) {
                    // Reload the user list for the current page after deletion
                    loadUserContent('/Admin/User', 1);
                } else {
                    // Handle validation errors or other messages from the server
                    $('#ajax-content').html(response);
                }
            },
            error: function () {
                alert("Failed to delete user."); // Handle any errors from the AJAX request
            }
        });
    }


    function toggleLockAccount( userId) {
        // Show loading spinner if necessary
        $('#loading').show();
        var url = '/Admin/User/ToggleLockAccount'; // Adjust this path if needed

        console.log("you click toggle lock button. url: " + url);
        $.ajax({
            url: url + "/" + userId,
            type: 'POST',
            success: function (response) {
                // Update the UI based on response

                loadUserContent("Admin/User");
            },
            error: function (xhr, status, error) {
                // Handle error
                alert('An error occurred while toggling the account lock status.');
            },
            complete: function () {
                // Hide loading spinner
                $('#loading').hide();
            }
        });
    }
    return { init };
})();

$(document).ready(function () {
    UserModule.init();
});
