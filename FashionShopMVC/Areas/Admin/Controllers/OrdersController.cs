using FashionShopMVC.Models;
using FashionShopMVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Diagnostics;
using System.Globalization;
using System.Net.Mime;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace FashionShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        [HttpGet]
        [Route("loadOrdersPartial")]
        //[AuthorizeRoles("Quản trị viên", "Nhân viên")]
        public IActionResult loadOrdersPartial(int page = 0, int pageSize = 5, int? typePayment = null, int? searchByID = null, string? searchByName = null, string? searchBySDT = null)
        {
            try
            {
                // Lấy danh sách đơn hàng từ repository
                var listOrders = _orderRepository.GetAll(page, pageSize, typePayment, searchByID, searchByName, searchBySDT);

                // Trả về view và truyền dữ liệu listOrders vào view
                return PartialView("_OrderSearchResults", listOrders);
            }
            catch
            {
                // Trả về view lỗi nếu có lỗi xảy ra
                return PartialView("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        [Route("")]
        //[AuthorizeRoles("Quản trị viên", "Nhân viên")]
        public IActionResult index()
        {
            return View();
        }

        [HttpGet]
        [Route("TotalPayment/{id}")]
        //[AuthorizeRoles("Quản trị viên", "Nhân viên")]
        public IActionResult TotalPayment(int id)
        {
            try
            {
                var totalPayment = _orderRepository.AdminTotalPayment(id);

                return Ok(totalPayment);
            }
            catch
            {
                return BadRequest("Lỗi");
            }
        }

        [HttpGet]
        [Route("Details/{id}")]
        //[AuthorizeRoles("Quản trị viên", "Nhân viên")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Lấy đơn hàng theo ID từ repository
                var order = await _orderRepository.GetOrderById(id);

                if (order != null)
                {
                    // Trả về view và truyền dữ liệu order vào view
                    return View(order);
                }
                else
                {
                    // Trả về view lỗi nếu không tìm thấy đơn hàng
                    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                }
            }
            catch
            {
                // Trả về view lỗi nếu có lỗi xảy ra
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        [Route("ExportExcel/{id}")]
        public async Task<IActionResult> ExportExcel(int id)
        {
            var order = await _orderRepository.GetOrderById(id);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DataSheet");

                worksheet.Column(2).Width = 36;
                worksheet.Column(3).Width = 13;
                worksheet.Column(4).Width = 13;
                worksheet.Column(5).Width = 13;

                worksheet.Cells["A1:B3"].Merge = true;
                worksheet.Cells["A1:B3"].Value = "FASHION SHOP";
                worksheet.Cells["A1:B3"].Style.Font.Bold = true;
                worksheet.Cells["A1:B3"].Style.Font.Size = 14;
                worksheet.Cells["A1:B3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:B3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["C1:E3"].Merge = true;
                worksheet.Cells["C1:E3"].Value = "HÓA ĐƠN BÁN HÀNG";
                worksheet.Cells["C1:E3"].Style.Font.Bold = true;
                worksheet.Cells["C1:E3"].Style.Font.Size = 14;
                worksheet.Cells["C1:E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C1:E3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["A8:E8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A8:E8"].Style.Font.Bold = true;

                // Insert customer data into template
                worksheet.Cells[4, 1].Value = "Tên khách hàng: " + order.FullName;
                worksheet.Cells[5, 1].Value = "Địa chỉ: " + order.Address;
                worksheet.Cells[6, 1].Value = "Số điện thoại: " + order.PhoneNumber;

                // Start Row for Detail Rows
                worksheet.Cells[8, 1].Value = "STT";
                worksheet.Cells[8, 2].Value = "Tên Hàng";
                worksheet.Cells[8, 3].Value = "Giá";
                worksheet.Cells[8, 4].Value = "SL";
                worksheet.Cells[8, 5].Value = "TT";

                // load order Detail
                int rowIndex = 9;
                int count = 1;
                double totalMoney = 0;
                foreach (var item in order.OrderDetails)
                {
                    worksheet.Cells[rowIndex, 1].Value = count.ToString();
                    worksheet.Cells[rowIndex, 2].Value = item.Product.Name.ToString();

                    var price = item.Price / item.Quantity;
                    worksheet.Cells[rowIndex, 3].Value = price.ToString();
                    worksheet.Cells[rowIndex, 4].Value = item.Quantity.ToString();

                    var tt = item.Price * item.Quantity;
                    worksheet.Cells[rowIndex, 5].Value = tt.ToString();

                    totalMoney += tt;
                    rowIndex++;
                    count++;
                }

                worksheet.Cells[$"A{rowIndex}:B{rowIndex}"].Merge = true;
                worksheet.Cells[$"A{rowIndex}:B{rowIndex}"].Value = "Tổng cộng";
                worksheet.Cells[$"A{rowIndex}:B{rowIndex}"].Style.Font.Bold = true;
                worksheet.Cells[$"A9:B{rowIndex}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[$"C8:E{rowIndex}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                worksheet.Cells[rowIndex, 5].Value = totalMoney.ToString();

                var cellRange = worksheet.Cells[$"A8:E{rowIndex}"];
                cellRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cellRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cellRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                cellRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ++rowIndex;
                worksheet.Cells[rowIndex, 4].Value = "Vận chuyển: ";
                worksheet.Cells[rowIndex, 5].Value = order.DeliveryFee.ToString("C0", new CultureInfo("vi-VN"));
                worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                ++rowIndex;
                worksheet.Cells[rowIndex, 4].Value = "Voucher: ";
                double voucherValue = 0;
                if (order.Voucher != null)
                {
                    if (order.Voucher.DiscountAmount == true)
                    {
                        voucherValue = order.Voucher.DiscountValue;
                        worksheet.Cells[rowIndex, 5].Value = "-" + voucherValue.ToString("C0", new CultureInfo("vi-VN"));
                    }
                    else
                    {
                        voucherValue = totalMoney * (order.Voucher.DiscountValue / 100);
                        worksheet.Cells[rowIndex, 5].Value = "-" + voucherValue.ToString("C0", new CultureInfo("vi-VN"));
                    }
                }
                else
                {
                    worksheet.Cells[rowIndex, 5].Value = "-" + 0.ToString("C0", new CultureInfo("vi-VN"));
                }

                worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                var totalPayment = totalMoney + order.DeliveryFee - voucherValue;

                ++rowIndex;
                worksheet.Cells[rowIndex, 4].Value = "Thành tiền: ";
                worksheet.Cells[rowIndex, 5].Value = totalPayment.ToString("C0", new CultureInfo("vi-VN"));
                worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                rowIndex += 3;
                worksheet.Cells[$"C{rowIndex}:E{rowIndex}"].Merge = true;
                var date = order.OrderDate;
                var dateFormat = "Ngày " + date.Day + " tháng " + date.Month + " năm " + date.Year;
                worksheet.Cells[rowIndex, 3].Value = dateFormat;
                worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ++rowIndex;
                worksheet.Cells[$"C{rowIndex}:E{rowIndex}"].Merge = true;
                worksheet.Cells[rowIndex, 2].Value = "KHÁCH HÀNG";
                worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, 3].Value = "NGƯỜI BÁN HÀNG";
                worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                var contentType = MediaTypeNames.Application.Octet;
                var fileExtension = "xlsx";

                var contentDisposition = new ContentDisposition
                {
                    Inline = false,
                    FileName = $"HoaDon-{order.ID}.{fileExtension}"
                };

                Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

                var bytes = package.GetAsByteArray();

                return File(bytes, contentType, $"HoaDon-{order.ID}.{fileExtension}");
            }
        }

        [HttpGet]
        //[AuthorizeRoles("Quản trị viên", "Nhân viên")]
        [Route("GetCountOrder")]
        public async Task<IActionResult> GetCountOrder()
        {
            try
            {
                var count = await _orderRepository.Count();

                return Ok(count);
            }
            catch
            {
                return BadRequest("Lỗi");
            }
        }
    }
}
