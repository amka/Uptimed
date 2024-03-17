using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uptimed.Migrations
{
    /// <inheritdoc />
    public partial class AddMonitorJobId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Monitors_AspNetUsers_OwnerId",
                table: "Monitors");

            migrationBuilder.DropIndex(
                name: "IX_Monitors_Alias",
                table: "Monitors");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Monitors",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobId",
                table: "Monitors",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Monitors_Alias_OwnerId",
                table: "Monitors",
                columns: new[] { "Alias", "OwnerId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Monitors_AspNetUsers_OwnerId",
                table: "Monitors",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Monitors_AspNetUsers_OwnerId",
                table: "Monitors");

            migrationBuilder.DropIndex(
                name: "IX_Monitors_Alias_OwnerId",
                table: "Monitors");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Monitors");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Monitors",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Monitors_Alias",
                table: "Monitors",
                column: "Alias",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Monitors_AspNetUsers_OwnerId",
                table: "Monitors",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
