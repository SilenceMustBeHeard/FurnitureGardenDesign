using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurnitureGardenDesign.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedSoftDeleteToFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Favorites",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Favorites");
        }
    }
}
