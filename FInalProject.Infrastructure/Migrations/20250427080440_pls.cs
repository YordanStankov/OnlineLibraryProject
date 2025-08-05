using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FInalProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class pls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        DECLARE @constraintName NVARCHAR(200)
        SELECT @constraintName = d.name 
        FROM sys.default_constraints d
        JOIN sys.columns c ON d.parent_object_id = c.object_id AND d.parent_column_id = c.column_id
        WHERE OBJECT_NAME(d.parent_object_id) = 'AspNetUsers' AND c.name = 'CanBorrow';
        
        IF @constraintName IS NOT NULL
            EXEC('ALTER TABLE [AspNetUsers] DROP CONSTRAINT ' + @constraintName);
    ");

            migrationBuilder.AlterColumn<bool>(
                name: "CanBorrow",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true,  // This now will apply after dropping old constraint
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: false
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert the column to its original state (no default value)
            migrationBuilder.AlterColumn<bool>(
                name: "CanBorrow",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: false,
                defaultValue: false  // Set the default back to whatever it was before, if needed.
            );

            // Recreate the default constraint for 'CanBorrow' if necessary
            migrationBuilder.Sql(@"
        ALTER TABLE [AspNetUsers] ADD CONSTRAINT DF_AspNetUsers_CanBorrow DEFAULT 0 FOR [CanBorrow];
    ");
        }
    }
}
