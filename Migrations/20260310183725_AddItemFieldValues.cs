using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagerApp.Migrations
{
    /// <inheritdoc />
    public partial class AddItemFieldValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemsFieldValues_InventoryFields_FieldId",
                table: "ItemsFieldValues");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemsFieldValues_InventoryItems_ItemId",
                table: "ItemsFieldValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemsFieldValues",
                table: "ItemsFieldValues");

            migrationBuilder.RenameTable(
                name: "ItemsFieldValues",
                newName: "ItemFieldValue");

            migrationBuilder.RenameIndex(
                name: "IX_ItemsFieldValues_ItemId",
                table: "ItemFieldValue",
                newName: "IX_ItemFieldValue_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemsFieldValues_FieldId",
                table: "ItemFieldValue",
                newName: "IX_ItemFieldValue_FieldId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemFieldValue",
                table: "ItemFieldValue",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemFieldValue_InventoryFields_FieldId",
                table: "ItemFieldValue",
                column: "FieldId",
                principalTable: "InventoryFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemFieldValue_InventoryItems_ItemId",
                table: "ItemFieldValue",
                column: "ItemId",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemFieldValue_InventoryFields_FieldId",
                table: "ItemFieldValue");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemFieldValue_InventoryItems_ItemId",
                table: "ItemFieldValue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemFieldValue",
                table: "ItemFieldValue");

            migrationBuilder.RenameTable(
                name: "ItemFieldValue",
                newName: "ItemsFieldValues");

            migrationBuilder.RenameIndex(
                name: "IX_ItemFieldValue_ItemId",
                table: "ItemsFieldValues",
                newName: "IX_ItemsFieldValues_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemFieldValue_FieldId",
                table: "ItemsFieldValues",
                newName: "IX_ItemsFieldValues_FieldId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemsFieldValues",
                table: "ItemsFieldValues",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsFieldValues_InventoryFields_FieldId",
                table: "ItemsFieldValues",
                column: "FieldId",
                principalTable: "InventoryFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsFieldValues_InventoryItems_ItemId",
                table: "ItemsFieldValues",
                column: "ItemId",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
