using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagerApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPaddingToCustomIdPart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Padding",
                table: "InventoryCustomIdParts",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Padding",
                table: "InventoryCustomIdParts");
        }
    }
}
