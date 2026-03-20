using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurnitureGardenDesign.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedNewNavigationToContactMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RespondedById",
                table: "ContactMessages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_RespondedById",
                table: "ContactMessages",
                column: "RespondedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_AspNetUsers_RespondedById",
                table: "ContactMessages",
                column: "RespondedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_AspNetUsers_RespondedById",
                table: "ContactMessages");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_RespondedById",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "RespondedById",
                table: "ContactMessages");
        }
    }
}
