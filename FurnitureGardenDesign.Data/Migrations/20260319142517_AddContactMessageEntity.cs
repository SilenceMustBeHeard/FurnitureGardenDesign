using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurnitureGardenDesign.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddContactMessageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "SystemInboxMessages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppUserId1",
                table: "SystemInboxMessages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "SystemInboxMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "SystemInboxMessages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "SystemInboxMessages",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RespondedAt",
                table: "SystemInboxMessages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "SystemInboxMessages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "SystemInboxMessages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemInboxMessages_AppUserId",
                table: "SystemInboxMessages",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemInboxMessages_AppUserId1",
                table: "SystemInboxMessages",
                column: "AppUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_SystemInboxMessages_AspNetUsers_AppUserId",
                table: "SystemInboxMessages",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SystemInboxMessages_AspNetUsers_AppUserId1",
                table: "SystemInboxMessages",
                column: "AppUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemInboxMessages_AspNetUsers_AppUserId",
                table: "SystemInboxMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_SystemInboxMessages_AspNetUsers_AppUserId1",
                table: "SystemInboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_SystemInboxMessages_AppUserId",
                table: "SystemInboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_SystemInboxMessages_AppUserId1",
                table: "SystemInboxMessages");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "SystemInboxMessages");

            migrationBuilder.DropColumn(
                name: "AppUserId1",
                table: "SystemInboxMessages");

            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "SystemInboxMessages");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "SystemInboxMessages");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "SystemInboxMessages");

            migrationBuilder.DropColumn(
                name: "RespondedAt",
                table: "SystemInboxMessages");

            migrationBuilder.DropColumn(
                name: "Response",
                table: "SystemInboxMessages");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "SystemInboxMessages");
        }
    }
}
