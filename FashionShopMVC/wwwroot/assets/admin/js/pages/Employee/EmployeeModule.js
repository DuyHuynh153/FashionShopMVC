const EmployeeModule = (() => {

    function init() {
        // load data employee

        $('a[data-url]').on('click', function (e) {
            e.preventDefault();
            var url = $(this).data('url');
            loadEmployeeContent(url);
        });
    }

    function loadEmployeeContent(url, page = 1, searchQuery = '') {
        console.log("you load employee");
        $('#loading').show();
        $.ajax({
            url: url,
            type: 'GET',
            /*data: {
                page: page,
                pageSize: 2,
                searchQuery: searchQuery // just empty now
            },*/
            success: function (data) {
                $('#ajax-content').html(data);
            },
            error: function () {
                $('#ajax-content').html('<p>Error loading content employee.</p>');
            },
            complete: function () {
                $('#loading').hide();
                $('.pagination a.page-link').removeClass('disabled').prop('disabled', false);
            }
        });
    }

    return { init };
})();

$(document).ready(() => {
    if (window.location.href.endsWith("/Admin/Employee")) {
        EmployeeModule.init();

    }
});
