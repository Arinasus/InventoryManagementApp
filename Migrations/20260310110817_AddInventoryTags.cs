using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InventoryManagerApp.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTagsMapping",
                columns: table => new
                {
                    InventoriesId = table.Column<int>(type: "integer", nullable: false),
                    TagsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTagsMapping", x => new { x.InventoriesId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_InventoryTagsMapping_Inventories_InventoriesId",
                        column: x => x.InventoriesId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryTagsMapping_InventoryTags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "InventoryTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTagsMapping_TagsId",
                table: "InventoryTagsMapping",
                column: "TagsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryTagsMapping");

            migrationBuilder.DropTable(
                name: "InventoryTags");
        }
    }
}
