using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropTodoListsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
    {
   // Drop TodoLists table
        migrationBuilder.DropTable(
       name: "TodoLists");
        }

  /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
       // Recreate TodoLists table if migration is reverted
          migrationBuilder.CreateTable(
     name: "TodoLists",
   columns: table => new
        {
           Id = table.Column<int>(type: "int", nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
         Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
   Colour_Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
            CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
          LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
     LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
              },
       constraints: table =>
   {
        table.PrimaryKey("PK_TodoLists", x => x.Id);
  });
        }
    }
}
