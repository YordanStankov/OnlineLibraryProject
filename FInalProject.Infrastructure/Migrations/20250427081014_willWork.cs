using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FInalProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class willWork : Migration
    {
        private const string TableName = "AspNetUsers";
        private const string Schema = "dbo";
        private const string Column = "CanBorrow";
        private const string DfltName = "DF_AspNetUsers_CanBorrow";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Drop any existing default constraint on dbo.AspNetUsers.CanBorrow
            migrationBuilder.Sql($@"
DECLARE @cname NVARCHAR(200);

SELECT @cname = dc.name
FROM sys.default_constraints dc
JOIN sys.columns c 
  ON dc.parent_object_id = c.object_id 
 AND dc.parent_column_id = c.column_id
JOIN sys.objects o 
  ON o.object_id = dc.parent_object_id
WHERE o.name = '{TableName}'
  AND SCHEMA_NAME(o.schema_id) = '{Schema}'
  AND c.name = '{Column}';

IF @cname IS NOT NULL
    EXEC('ALTER TABLE [{Schema}].[{TableName}] DROP CONSTRAINT ' + @cname);
");

            // 2) Alter the column (type and nullability unchanged) and add our new default
            migrationBuilder.AlterColumn<bool>(
                name: Column,
                table: TableName,
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            // 3) Explicitly add our default constraint of 1 (true)
            migrationBuilder.Sql($@"
ALTER TABLE [{Schema}].[{TableName}] 
    ADD CONSTRAINT {DfltName} 
    DEFAULT 1 FOR [{Column}];
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the constraint we added in Up()
            migrationBuilder.Sql($@"
IF OBJECT_ID('{Schema}.{DfltName}', 'D') IS NOT NULL
    ALTER TABLE [{Schema}].[{TableName}] DROP CONSTRAINT {DfltName};
");

            // Revert the column (this leaves it bit NOT NULL with no default)
            migrationBuilder.AlterColumn<bool>(
                name: Column,
                table: TableName,
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
