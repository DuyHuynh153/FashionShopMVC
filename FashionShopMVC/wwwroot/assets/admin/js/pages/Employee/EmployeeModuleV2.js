$(document).ready(function () {
    // Check if URL ends with "/Admin/Employee"
    if (window.location.href.endsWith("/Admin/Employee")) {
        loadEmployees(); // Initial load when page loads

        // Handle search button click
        $(document).on('click', '.search-employee-btn', function () {
            var searchQuery = $('#searchEmployee').val();
            loadEmployees(1, searchQuery); // Reload data on search button click
        });

        // Handle pagination click event
        $(document).on('click', '.pagination a.page-link', function (e) {
            e.preventDefault(); // Prevent default action of <a> tag
            var page = $(this).data('page'); // Get page number from <a> tag
            loadEmployees(page); // Load employee data for selected page
        });

        // Attach other events (like creating, editing, deleting employees) if needed
        $(document).on('click', '.create-employee-btn', loadEmployeeCreateForm);
        $(document).on('submit', '#createEmployeeForm', submitEmployeeCreateForm);
        $(document).on('click', '.edit-employee-btn', loadEmployeeEditForm);
        $(document).on('submit', '#editEmployeeForm', submitEmployeeEditForm);
        $(document).on('click', '.delete-employee-btn', loadDeleteEmployeeConfirmation);
        $(document).on('submit', '#deleteEmployeeForm', submitEmployeeDeleteForm);

        $(document).on('click', '.toggle-lock-employee-btn', function () {
            var employeeId = $(this).data('id');
            toggleLockAccountEmployee(employeeId);
        });
    }
});

function loadEmployees(page = 1, searchQuery = '') {
    console.log("Employee page selected: ", page);
    console.log("Employee search query: ", searchQuery);
    $('#loading').show();
    $.ajax({
        url: '/Admin/Employee', // Adjust the URL as needed
        type: 'GET',
        data: {
            page: page,
            pageSize: 2, // Set the desired page size
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

// Example function for creating an employee
function loadEmployeeCreateForm() {
    console.log("you enter create form")
    $.ajax({
        url: '/Admin/Employee/Create', // Adjust URL as needed
        type: 'GET',
        success: function (data) {
            $('#ajax-content').html(data);
        },
        error: function () {
            alert("Failed to load create employee form.");
        }
    });
}

// Example function to handle form submission for creating an employee
function submitEmployeeCreateForm(e) {
    e.preventDefault();
    var form = $(this);
    console.log("you enter submit form employee")

    $.ajax({
        url: '/Admin/Employee/Create', // Adjust URL as needed
        type: 'POST',
        data: form.serialize(),
        success: function (response) {
            if (response.success) {
                loadEmployees(); // Reload employee list after successful creation
            } else {
                $('#ajax-content').html(response);
            }
        },

        error: function () {
            alert("Failed to create employee.");
        }
    });
}

// Example function for editing an employee
function loadEmployeeEditForm() {
    var employeeId = $(this).data('id');
    $.ajax({
        url: '/Admin/Employee/Edit/' + employeeId, // Adjust URL as needed
        type: 'GET',
        success: function (data) {
            $('#ajax-content').html(data);
        },
        error: function () {
            alert("Failed to load edit form.");
        }
    });
}

// Example function for submitting the edit form for employee updates
function submitEmployeeEditForm(e) {
    e.preventDefault();
    var form = $(this);
    var employeeId = form.find('input[name="id"]').val();

    $.ajax({
        url: '/Admin/Employee/Edit/' + employeeId, // Adjust URL as needed
        type: 'POST',
        data: form.serialize(),
        success: function (response) {
            if (response.success) {
                loadEmployees(); // Reload employee list after successful update
            } else {
                $('#ajax-content').html(response);
            }
        },
        error: function () {
            alert("Failed to update employee.");
        }
    });
}

// Example function for deleting an employee
function loadDeleteEmployeeConfirmation() {
    var employeeId = $(this).data('id');
    $.ajax({
        url: '/Admin/Employee/Delete/' + employeeId, // Adjust URL as needed
        type: 'GET',
        success: function (data) {
            $('#ajax-content').html(data);
        },
        error: function () {
            alert("Failed to load delete confirmation.");
        }
    });
}

// Example function for submitting the delete form for employee deletion
function submitEmployeeDeleteForm(e) {
    e.preventDefault();
    var form = $(this);

    $.ajax({
        url: '/Admin/Employee/Delete/' + form.data('id'), // Adjust URL as needed
        type: 'POST',
        data: form.serialize(),
        success: function (response) {
            if (response.success) {
                loadEmployees(); // Reload employee list after successful deletion
            } else {
                $('#ajax-content').html(response);
            }
        },
        error: function () {
            alert("Failed to delete employee.");
        }
    });
}

// Example function for toggling employee account status (lock/unlock)
function toggleLockAccountEmployee(employeeId) {
    $('#loading').show();
    $.ajax({
        url: '/Admin/Employee/ToggleLockAccount/' + employeeId, // Adjust URL as needed
        type: 'POST',
        success: function (response) {
            if (response.success) {
                loadEmployees(); // Reload employee list after successful lock/unlock
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
