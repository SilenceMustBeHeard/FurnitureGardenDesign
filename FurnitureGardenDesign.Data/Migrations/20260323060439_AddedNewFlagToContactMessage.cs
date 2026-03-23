using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurnitureGardenDesign.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewFlagToContactMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReadByAdmin",
                table: "ContactMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReadByAdmin",
                table: "ContactMessages");
        }
    }
}
