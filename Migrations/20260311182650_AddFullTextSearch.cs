using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace InventoryManagerApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFullTextSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "Inventories",
                type: "tsvector",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_SearchVector",
                table: "Inventories",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inventories_SearchVector",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "Inventories");
        }
    }
}
