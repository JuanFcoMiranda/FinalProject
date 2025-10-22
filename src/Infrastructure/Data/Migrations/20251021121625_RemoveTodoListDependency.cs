using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject.Infrastructure.Data.Migrations
{
  /// <inheritdoc />
    public partial class RemoveTodoListDependency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraint
  migrationBuilder.DropForeignKey(
   name: "FK_TodoItems_TodoLists_ListId",
   table: "TodoItems");

            // Drop index on ListId
       migrationBuilder.DropIndex(
         name: "IX_TodoItems_ListId",
            table: "TodoItems");

// Drop ListId column
     migrationBuilder.DropColumn(
      name: "ListId",
      table: "TodoItems");
      }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
     // Add ListId column back
        migrationBuilder.AddColumn<int>(
                name: "ListId",
      table: "TodoItems",
            type: "int",
    nullable: false,
           defaultValue: 0);

        // Recreate index
      migrationBuilder.CreateIndex(
         name: "IX_TodoItems_ListId",
      table: "TodoItems",
        column: "ListId");

    // Recreate foreign key constraint
      migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_TodoLists_ListId",
    table: "TodoItems",
         column: "ListId",
       principalTable: "TodoLists",
         principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);
        }
 }
}
