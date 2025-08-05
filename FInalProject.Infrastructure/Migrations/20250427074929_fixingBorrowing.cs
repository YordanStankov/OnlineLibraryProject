using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FInalProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixingBorrowing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("UPDATE [AspNetUsers] SET [CanBorrow] = 1 WHERE [CanBorrow] = 0;");

            migrationBuilder.AlterColumn<bool>(
                name: "CanBorrow",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                 name: "CanBorrow",
                 table: "AspNetUsers",
                 type: "bit",
                 nullable: false,
                 defaultValue: false,
                 oldClrType: typeof(bool),
                 oldType: "bit",
                 oldNullable: false);
        }
    }
}
