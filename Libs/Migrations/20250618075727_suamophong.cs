using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libs.Migrations
{
    /// <inheritdoc />
    public partial class suamophong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MoPhongs_LoaiBangLaiId",
                table: "MoPhongs",
                column: "LoaiBangLaiId");

            migrationBuilder.AddForeignKey(
                name: "FK_MoPhongs_LoaiBangLais_LoaiBangLaiId",
                table: "MoPhongs",
                column: "LoaiBangLaiId",
                principalTable: "LoaiBangLais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MoPhongs_LoaiBangLais_LoaiBangLaiId",
                table: "MoPhongs");

            migrationBuilder.DropIndex(
                name: "IX_MoPhongs_LoaiBangLaiId",
                table: "MoPhongs");
        }
    }
}
