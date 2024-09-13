using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _5dDiplomacyWithMultiverseTimeTravel.Migrations
{
    /// <inheritdoc />
    public partial class FixOneToManyUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_UnitId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "UnitId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UnitId",
                table: "Orders",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_UnitId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "UnitId",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UnitId",
                table: "Orders",
                column: "UnitId",
                unique: true,
                filter: "[UnitId] IS NOT NULL");
        }
    }
}
