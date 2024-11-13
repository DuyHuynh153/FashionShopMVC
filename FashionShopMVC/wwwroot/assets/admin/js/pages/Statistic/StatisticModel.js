document.addEventListener("DOMContentLoaded", function () {
    // Lấy ngày đầu và ngày cuối của tháng hiện tại
    const now = new Date();
    const firstDay = new Date(now.getFullYear(), now.getMonth(), 1);
    const lastDay = new Date(now.getFullYear(), now.getMonth() + 1, 0);

    //const firstDay = new Date(2023, 9, 1);
    //const lastDay = new Date(2023, 9 + 1, 0);

    // Định dạng ngày giờ theo ISO để sử dụng cho input datetime-local
    const formatDate = (date) => {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        const hours = String(date.getHours()).padStart(2, '0');
        const minutes = String(date.getMinutes()).padStart(2, '0');
        return `${year}-${month}-${day}T${hours}:${minutes}`;
    };

    // Thiết lập giá trị mặc định cho các input
    const fromDateInput = document.getElementById("fromDate");
    const toDateInput = document.getElementById("toDate");
    if (!fromDateInput.value) {
        fromDateInput.value = formatDate(firstDay);
    }
    if (!toDateInput.value) {
        toDateInput.value = formatDate(lastDay);
    }
});
$(document).ready(function () {
    loadRevenueChart(); // Default view on page load
    $('#btnLoadStatistic').click(function () {
        loadPage();
    })
    //$('#revenueType').change(function () {
    //    var type = $(this).val();
    //    let fromDate = $('#fromDate').val();
    //    let toDate = $('#toDate').val();
    //    loadRevenueChart(type);
    //});
});

function loadPage() {
    let revenueType = $('#revenueType').val();
    let fromDate = $('#fromDate').val();
    let toDate = $('#toDate').val();
    $.ajax({
        url: `/Admin/Statistics/GetRevenueStatistic`,
        type: 'GET',
        data: {
            fromDate: fromDate,
            toDate: toDate,
            revenueType: revenueType
        },
        success: function (response) {
            $('#statisticResults').html(response);
            loadRevenueChart();
            toastr.success("Load data thành công.", "Thông báo");
        },
        error: function (xhr, status, error) {
            console.error("Lỗi:", error);
            toastr.error("Lỗi khi tải dữ liệu. Vui lòng thử lại.", "Lỗi");
            $('#searchOrderResults').html('<p>Lỗi khi tải dữ liệu. Vui lòng thử lại.</p>');
        },
        complete: function () {
            // Ẩn hiệu ứng đang tải sau khi hoàn tất
            $('#loading').hide();
        }
    });
}
function loadRevenueChart() {
    drawChart();
}
function drawChart() {
    let revenueType = $('#revenueType').val();
    let fromDate = $('#fromDate').val();
    let toDate = $('#toDate').val();
    $.ajax({
        url: `/Admin/Statistics/GetRevenueStatisticList`,
        type: 'GET',
        data: {
            fromDate: fromDate,
            toDate: toDate,
            revenueType: revenueType
        },
        success: function (response) {
            if (response.listRevenueStatistic.length > 0) {
                updateChart(response.listRevenueStatistic);
            } else {
                toastr.success("Ngày chọn không có dữ liệu.", "Thông báo");
            }
            
        },
        error: function (xhr, status, error) {
            console.error("Lỗi:", error);
            toastr.error("Lỗi khi tải dữ liệu. Vui lòng thử lại.", "Lỗi");
            $('#searchOrderResults').html('<p>Lỗi khi tải dữ liệu. Vui lòng thử lại.</p>');
        },
        complete: function () {
            // Ẩn hiệu ứng đang tải sau khi hoàn tất
            $('#loading').hide();
        }
    });
}
function updateChart(data) {
    let revenueType = $('#revenueType').val();
    let chartType = "";
    if (revenueType == "day") {
        chartType = "line";
    } else {
        chartType = "bar";
    }
    if (data.length == 0) {
        toastr.error("Không có dữ liệu.");
        return;
    }
    const ctx = document.getElementById('revenueChart').getContext('2d');
    const labels = data.map(item => item.date);
    if (revenueType === "quarter") {
        label = data.map(item => convertQuarterString(item.date))
    }
    const revenues = data.map(item => item.revenues);
    const benefits = data.map(item => item.benefit);
    if (window.myChart) {
        window.myChart.destroy();
    }
    window.myChart = new Chart(ctx, {
        type: chartType,
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Doanh Thu',
                    data: revenues,
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 2,
                    fill: false
                },
                {
                    label: 'Lợi Nhuận',
                    data: benefits,
                    borderColor: 'rgba(153, 102, 255, 1)',
                    borderWidth: 2,
                    fill: false
                }
            ]
        },
        options: {
            responsive: true,
            scales: {
                x: { title: { display: true, text: 'Ngày' } },
                y: { title: { display: true, text: 'Số tiền (VNĐ)' } }
            }
        },
    });
    toastr.success("Tải bảng thành công");
}

function convertQuarterString(dateString) {
    return dateString.replace(/-Q(\d)/, (match, quarter) => {
        return ` Quý ${quarter}`;
    });
}