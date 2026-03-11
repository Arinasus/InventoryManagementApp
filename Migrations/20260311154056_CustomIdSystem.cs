using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagerApp.Migrations
{
    /// <inheritdoc />
    public partial class CustomIdSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCustomIdPart_Inventories_InventoryId",
                table: "InventoryCustomIdPart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryCustomIdPart",
                table: "InventoryCustomIdPart");

            migrationBuilder.RenameTable(
                name: "InventoryCustomIdPart",
                newName: "InventoryCustomIdParts");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryCustomIdPart_InventoryId",
                table: "InventoryCustomIdParts",
                newName: "IX_InventoryCustomIdParts_InventoryId");

            migrationBuilder.AddColumn<int>(
                name: "SequenceNumber",
                table: "InventoryItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryCustomIdParts",
                table: "InventoryCustomIdParts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCustomIdParts_Inventories_InventoryId",
                table: "InventoryCustomIdParts",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCustomIdParts_Inventories_InventoryId",
                table: "InventoryCustomIdParts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryCustomIdParts",
                table: "InventoryCustomIdParts");

            migrationBuilder.DropColumn(
                name: "SequenceNumber",
                table: "InventoryItems");

            migrationBuilder.RenameTable(
                name: "InventoryCustomIdParts",
                newName: "InventoryCustomIdPart");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryCustomIdParts_InventoryId",
                table: "InventoryCustomIdPart",
                newName: "IX_InventoryCustomIdPart_InventoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryCustomIdPart",
                table: "InventoryCustomIdPart",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCustomIdPart_Inventories_InventoryId",
                table: "InventoryCustomIdPart",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
