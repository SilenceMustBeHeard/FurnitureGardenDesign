using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurnitureGardenDesign.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedDesignVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "DesignVariants",
                newName: "Image2DUrl");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "DesignVariants",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DesignVariants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Model3DUrl",
                table: "DesignVariants",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "DesignVariants");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DesignVariants");

            migrationBuilder.DropColumn(
                name: "Model3DUrl",
                table: "DesignVariants");

            migrationBuilder.RenameColumn(
                name: "Image2DUrl",
                table: "DesignVariants",
                newName: "ImageUrl");
        }
    }
}
