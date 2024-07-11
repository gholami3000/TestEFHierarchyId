using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestEFHierarchyId.Migrations
{
    /// <inheritdoc />
    public partial class m8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Storages_StorageId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_StorageId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "Locations");

            migrationBuilder.CreateIndex(
                name: "IX_Storages_LocationId",
                table: "Storages",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Storages_Locations_LocationId",
                table: "Storages",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Storages_Locations_LocationId",
                table: "Storages");

            migrationBuilder.DropIndex(
                name: "IX_Storages_LocationId",
                table: "Storages");

            migrationBuilder.AddColumn<int>(
                name: "StorageId",
                table: "Locations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_StorageId",
                table: "Locations",
                column: "StorageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Storages_StorageId",
                table: "Locations",
                column: "StorageId",
                principalTable: "Storages",
                principalColumn: "Id");
        }
    }
}
