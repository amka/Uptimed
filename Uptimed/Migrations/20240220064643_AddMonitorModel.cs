using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uptimed.Migrations
{
    /// <inheritdoc />
    public partial class AddMonitorModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Monitors",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: false),
                    Alias = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    RequestMethod = table.Column<string>(type: "TEXT", nullable: false),
                    RequestBody = table.Column<string>(type: "TEXT", nullable: false),
                    RequestTimeout = table.Column<int>(type: "INTEGER", nullable: false),
                    OwnerId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monitors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monitors_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Monitors_Alias",
                table: "Monitors",
                column: "Alias",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Monitors_OwnerId",
                table: "Monitors",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Monitors");
        }
    }
}
