using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itishnik.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class TaskTaskBlocksRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskTaskBlock_TaskBlocks_TaskBlockId",
                table: "TaskTaskBlock");

            migrationBuilder.RenameColumn(
                name: "TaskBlockId",
                table: "TaskTaskBlock",
                newName: "TaskBlocksId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTaskBlock_TaskBlocks_TaskBlocksId",
                table: "TaskTaskBlock",
                column: "TaskBlocksId",
                principalTable: "TaskBlocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskTaskBlock_TaskBlocks_TaskBlocksId",
                table: "TaskTaskBlock");

            migrationBuilder.RenameColumn(
                name: "TaskBlocksId",
                table: "TaskTaskBlock",
                newName: "TaskBlockId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTaskBlock_TaskBlocks_TaskBlockId",
                table: "TaskTaskBlock",
                column: "TaskBlockId",
                principalTable: "TaskBlocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
