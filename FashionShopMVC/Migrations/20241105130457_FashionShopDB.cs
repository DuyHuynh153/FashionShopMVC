using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FashionShopMVC.Migrations
{
    public partial class FashionShopDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "25d9875c-878d-414e-8e6f-b4c28815f739");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "3195156e-ef20-4c3d-9406-7bc7e87fd6f6");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "9cd0f7a2-741d-405a-a8a3-a34b22da200c");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "25d9875c-878d-414e-8e6f-b4c28815f739", "25d9875c-878d-414e-8e6f-b4c28815f739", "Quản trị viên", "QUẢN TRỊ VIÊN" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3195156e-ef20-4c3d-9406-7bc7e87fd6f6", "3195156e-ef20-4c3d-9406-7bc7e87fd6f6", "Nhân viên", "NHÂN VIÊN" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9cd0f7a2-741d-405a-a8a3-a34b22da200c", "9cd0f7a2-741d-405a-a8a3-a34b22da200c", "Khách hàng", "KHÁCH HÀNG" });
        }
    }
}
