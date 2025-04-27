using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FInalProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class changedNametoCantBorrow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE dbo.AspNetUsers
                SET CanBorrow = 0
                WHERE CanBorrow = 1;
            ");

            migrationBuilder.RenameColumn(
                name: "CanBorrow",
                table: "AspNetUsers",
                newName: "CantBorrow");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CantBorrow",
                table: "AspNetUsers",
                newName: "CanBorrow");
        }
    }
}
