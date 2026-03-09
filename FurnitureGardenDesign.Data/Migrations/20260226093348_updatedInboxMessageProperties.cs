using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurnitureGardenDesign.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedInboxMessageProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InboxMessages_AspNetUsers_ReceiverId",
                table: "InboxMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_InboxMessages_DesignVariants_DesignVariantId",
                table: "InboxMessages");

            migrationBuilder.AlterColumn<Guid>(
                name: "DesignVariantId",
                table: "InboxMessages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "InboxMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "InboxMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderId",
                table: "InboxMessages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessages_SenderId",
                table: "InboxMessages",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_InboxMessages_AspNetUsers_ReceiverId",
                table: "InboxMessages",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InboxMessages_AspNetUsers_SenderId",
                table: "InboxMessages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InboxMessages_DesignVariants_DesignVariantId",
                table: "InboxMessages",
                column: "DesignVariantId",
                principalTable: "DesignVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InboxMessages_AspNetUsers_ReceiverId",
                table: "InboxMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_InboxMessages_AspNetUsers_SenderId",
                table: "InboxMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_InboxMessages_DesignVariants_DesignVariantId",
                table: "InboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboxMessages_SenderId",
                table: "InboxMessages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "InboxMessages");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "InboxMessages");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "InboxMessages");

            migrationBuilder.AlterColumn<Guid>(
                name: "DesignVariantId",
                table: "InboxMessages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_InboxMessages_AspNetUsers_ReceiverId",
                table: "InboxMessages",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InboxMessages_DesignVariants_DesignVariantId",
                table: "InboxMessages",
                column: "DesignVariantId",
                principalTable: "DesignVariants",
                principalColumn: "Id");
        }
    }
}
