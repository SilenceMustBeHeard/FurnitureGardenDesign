using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurnitureGardenDesign.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseMessageInheritance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_AspNetUsers_RespondedById",
                table: "ContactMessages");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "ContactMessages",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "ContactMessages",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_AspNetUsers_RespondedById",
                table: "ContactMessages",
                column: "RespondedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_AspNetUsers_RespondedById",
                table: "ContactMessages");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "ContactMessages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "ContactMessages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_AspNetUsers_RespondedById",
                table: "ContactMessages",
                column: "RespondedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
