$(document).ready(function () {
    // Check if URL ends with "/Admin/User"
    if (window.location.href.endsWith("/Admin/User")) {
        loadUsers(); // Initial load when page loads

        // Handle search button click
        /*$('.search-user-btn').click(function () {
            loadUsers(); // Reload data on search button click
        });*/
        $(document).on('click', '.search-user-btn', function () {

            var searchQuery = $('#searchInput').val();

           

            loadUsers(searchQuery= searchQuery);
        });

        // Handle pagination click event
        $(document).on('click', '.pagination a.page-link', function (e) {
            e.preventDefault(); // Prevent default action of <a> tag
            var page = $(this).data('page'); // Get page number from <a> tag
            loadUsers(page);
        });

        // Attach events for create, edit, delete, and toggle lock functionality
        $(document).on('click', '.create-btn', loadCreateForm);
        $(document).on('submit', '#createUserForm', submitCreateForm);
        $(document).on('click', '.edit-btn', loadEditForm);
        $(document).on('submit', '#editUserForm', submitEditForm);
        $(document).on('click', '.delete-btn', loadDeleteConfirmation);
        $(document).on('submit', '#deleteUserForm', submitDeleteForm);
        $(document).on('click', '.toggle-lock-btn', function () {
            var userId = $(this).data('id');
            toggleLockAccount(userId);
        });
    }
});

function loadUsers(page = 1, searchQuery = '') {
    var searchQuery = $('#searchInput').val(); // Get search query from input

    console.log("User page selected: ", page);
    console.log("User search query: ", searchQuery);
    $('#loading').show();
    $.ajax({
        url: '/Admin/User',
        type: 'GET',
        data: {
            page: page,
            pageSize: 2,
            searchQuery: searchQuery
        },
        success: function (data) {
            $('#ajax-content').html(data); // Update content
        },
        error: function () {
            $('#ajax-content').html('<p>Error loading content.</p>');
        },
        complete: function () {
            $('#loading').hide();
        }
    });
}

function loadCreateForm() {
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
                loadUsers();
            } else {
                $('#ajax-content').html(response);
            }
        },
        error: function () {
            alert("Failed to create user.");
        }
    });
}

function loadEditForm() {
    var userId = $(this).data('id');
    $.ajax({
        url: '/Admin/User/Edit/' + userId,
        type: 'GET',
        success: function (data) {
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
                loadUsers();
            } else {
                $('#ajax-content').html(response);
            }
        },
        error: function () {
            alert("Failed to update user.");
        }
    });
}

function loadDeleteConfirmation() {
    var userId = $(this).data('id');
    $.ajax({
        url: '/Admin/User/Delete/' + userId,
        type: 'GET',
        success: function (data) {
            $('#ajax-content').html(data);
        },
        error: function () {
            alert("Failed to load delete confirmation.");
        }
    });
}

function submitDeleteForm(e) {
    e.preventDefault();
    var form = $(this);

    $.ajax({
        url: '/Admin/User/Delete/' + form.data('id'),
        type: 'POST',
        data: form.serialize(),
        success: function (response) {
            if (response.success) {
                loadUsers();
            } else {
                $('#ajax-content').html(response);
            }
        },
        error: function () {
            alert("Failed to delete user.");
        }
    });
}

function toggleLockAccount(userId) {
    $('#loading').show();
    $.ajax({
        url: '/Admin/User/ToggleLockAccount/' + userId,
        type: 'POST',
        success: function (response) {
            if (response.success) {
                loadUsers();
            } else {
                $('#ajax-content').html(response);
            }
        },
        error: function () {
            alert('An error occurred while toggling the account lock status.');
        },
        complete: function () {
            $('#loading').hide();
        }
    });
}
