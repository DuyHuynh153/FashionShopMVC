const StatisticalModule = (() => {
    let chartInstance = null; // Store the chart instance globally

    function init() {
        // Setup the click handler for the link
        $('a[data-url]').on('click', function (e) {
            e.preventDefault();
            var url = $(this).data('url');
            loadStatistical(url); // Load statistical data and render chart
        });
    }

    function loadStatistical(url, page = 1, searchQuery = '') {
        console.log("You load statistical");

        // Perform AJAX request to load the content
        $.ajax({
            url: url, // The URL that returns the chart partial view
            type: 'GET',
            success: function (response) {
                $('#ajax-content').html(response); // Insert the new content (including canvas)

                // Initialize or reinitialize the chart after the content is loaded
                renderChart();
            },
            error: function () {
                console.error("Error loading data");
            }
        });
    }

    // Function to render the chart
    function renderChart() {
        const chartElement = document.getElementById('myChart');
        if (!chartElement) {
            console.error("Canvas element with id 'myChart' not found.");
            return;
        }

        // Destroy the previous chart instance if it exists
        if (chartInstance) {
            chartInstance.destroy();
        }

        const ctx = chartElement.getContext('2d');
        chartInstance = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange'],
                datasets: [{
                    label: '# of Votes',
                    data: [12, 19, 3, 5, 2, 3],
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }

    return { init };
})();

$(document).ready(() => {
    StatisticalModule.init(); // Initialize the module
});
