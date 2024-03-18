using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uptimed.Migrations
{
    /// <inheritdoc />
    public partial class AddMonitorUserAgent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsEnabled",
                table: "Monitors",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "Monitors",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "Monitors");

            migrationBuilder.AlterColumn<int>(
                name: "IsEnabled",
                table: "Monitors",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");
        }
    }
}
