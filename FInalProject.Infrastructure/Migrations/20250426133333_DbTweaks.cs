using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FInalProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class DbTweaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "StrikeGiven",
                table: "BorrowedBooks",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanBorrow",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Strikes",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StrikeGiven",
                table: "BorrowedBooks");

            migrationBuilder.DropColumn(
                name: "CanBorrow",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Strikes",
                table: "AspNetUsers");
        }
    }
}
