using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurnitureGardenDesign.Data.Migrations
{
    /// <inheritdoc />
    public partial class Added3DmodelOnCatalogDesigns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "CatalogDesigns",
                newName: "Image2DUrl");

            migrationBuilder.AddColumn<int>(
                name: "Model3DStatus",
                table: "CatalogDesigns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Model3DUrl",
                table: "CatalogDesigns",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Model3DStatus",
                table: "CatalogDesigns");

            migrationBuilder.DropColumn(
                name: "Model3DUrl",
                table: "CatalogDesigns");

            migrationBuilder.RenameColumn(
                name: "Image2DUrl",
                table: "CatalogDesigns",
                newName: "ImageUrl");
        }
    }
}
