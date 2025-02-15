using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _5dDiplomacyWithMultiverseTimeTravel.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class EnableMultiProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsSandbox = table.Column<bool>(type: "bit", nullable: false),
                    HasStrictAdjacencies = table.Column<bool>(type: "bit", nullable: false),
                    Players = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayersSubmitted = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Worlds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    Iteration = table.Column<int>(type: "int", nullable: false),
                    Winner = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worlds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Worlds_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Boards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorldId = table.Column<int>(type: "int", nullable: false),
                    Timeline = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Phase = table.Column<int>(type: "int", nullable: false),
                    ChildTimelines = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MightAdvance = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Boards_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Centres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoardId = table.Column<int>(type: "int", nullable: false),
                    Owner = table.Column<int>(type: "int", nullable: true),
                    Location_Phase = table.Column<int>(type: "int", nullable: false),
                    Location_RegionId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Location_Timeline = table.Column<int>(type: "int", nullable: false),
                    Location_Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Centres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Centres_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoardId = table.Column<int>(type: "int", nullable: false),
                    Owner = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MustRetreat = table.Column<bool>(type: "bit", nullable: false),
                    Location_Phase = table.Column<int>(type: "int", nullable: false),
                    Location_RegionId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Location_Timeline = table.Column<int>(type: "int", nullable: false),
                    Location_Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Units_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorldId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Location_Phase = table.Column<int>(type: "int", nullable: false),
                    Location_RegionId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Location_Timeline = table.Column<int>(type: "int", nullable: false),
                    Location_Year = table.Column<int>(type: "int", nullable: false),
                    Destination_Phase = table.Column<int>(type: "int", nullable: true),
                    Destination_RegionId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Destination_Timeline = table.Column<int>(type: "int", nullable: true),
                    Destination_Year = table.Column<int>(type: "int", nullable: true),
                    Midpoint_Phase = table.Column<int>(type: "int", nullable: true),
                    Midpoint_RegionId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Midpoint_Timeline = table.Column<int>(type: "int", nullable: true),
                    Midpoint_Year = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boards_WorldId",
                table: "Boards",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Centres_BoardId",
                table: "Centres",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UnitId",
                table: "Orders",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_WorldId",
                table: "Orders",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_BoardId",
                table: "Units",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_GameId",
                table: "Worlds",
                column: "GameId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Centres");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Boards");

            migrationBuilder.DropTable(
                name: "Worlds");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
