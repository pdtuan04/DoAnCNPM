using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libs.Migrations
{
    /// <inheritdoc />
    public partial class thanhtoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TinhNangMoKhoas",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "NgayTao",
                table: "TinhNangMoKhoas",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "NgayTao",
                table: "GiaoDichThanhToans",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "NgayTao",
                table: "DonHangs",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.CreateIndex(
                name: "UX_User_TinhNang_Active",
                table: "TinhNangMoKhoas",
                columns: new[] { "UserId", "TenTinhNang", "DangHoatDong" });

            migrationBuilder.CreateIndex(
                name: "IX_GiaoDichThanhToans_MaDonCong",
                table: "GiaoDichThanhToans",
                column: "MaDonCong");

            migrationBuilder.CreateIndex(
                name: "IX_GiaoDichThanhToans_MaGiaoDichCuoi",
                table: "GiaoDichThanhToans",
                column: "MaGiaoDichCuoi",
                unique: true,
                filter: "[MaGiaoDichCuoi] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GiaoDichThanhToans_TrangThai_NgayTao",
                table: "GiaoDichThanhToans",
                columns: new[] { "TrangThai", "NgayTao" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_User_TinhNang_Active",
                table: "TinhNangMoKhoas");

            migrationBuilder.DropIndex(
                name: "IX_GiaoDichThanhToans_MaDonCong",
                table: "GiaoDichThanhToans");

            migrationBuilder.DropIndex(
                name: "IX_GiaoDichThanhToans_MaGiaoDichCuoi",
                table: "GiaoDichThanhToans");

            migrationBuilder.DropIndex(
                name: "IX_GiaoDichThanhToans_TrangThai_NgayTao",
                table: "GiaoDichThanhToans");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TinhNangMoKhoas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "NgayTao",
                table: "TinhNangMoKhoas",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "NgayTao",
                table: "GiaoDichThanhToans",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "NgayTao",
                table: "DonHangs",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "GETUTCDATE()");
        }
    }
}
