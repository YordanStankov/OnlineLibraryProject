using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FInalProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixingBorrowing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTaken",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "UntillReturn",
                table: "Books");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTaken",
                table: "BorrowedBooks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UntillReturn",
                table: "BorrowedBooks",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTaken",
                table: "BorrowedBooks");

            migrationBuilder.DropColumn(
                name: "UntillReturn",
                table: "BorrowedBooks");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTaken",
                table: "Books",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UntillReturn",
                table: "Books",
                type: "datetimeoffset",
                nullable: true);
        }
    }
}
