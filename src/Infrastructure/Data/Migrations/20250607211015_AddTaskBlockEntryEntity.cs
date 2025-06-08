using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itishnik.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskBlockEntryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskTaskBlock");

            migrationBuilder.DropColumn(
                name: "Weights",
                table: "TaskBlocks");

            migrationBuilder.CreateTable(
                name: "TaskBlockEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskBlockId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskBlockEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskBlockEntry_TaskBlocks_TaskBlockId",
                        column: x => x.TaskBlockId,
                        principalTable: "TaskBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskBlockEntry_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskBlockEntry_TaskBlockId",
                table: "TaskBlockEntry",
                column: "TaskBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBlockEntry_TaskId",
                table: "TaskBlockEntry",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskBlockEntry");

            migrationBuilder.AddColumn<int[]>(
                name: "Weights",
                table: "TaskBlocks",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.CreateTable(
                name: "TaskTaskBlock",
                columns: table => new
                {
                    TaskBlocksId = table.Column<Guid>(type: "uuid", nullable: false),
                    TasksId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTaskBlock", x => new { x.TaskBlocksId, x.TasksId });
                    table.ForeignKey(
                        name: "FK_TaskTaskBlock_TaskBlocks_TaskBlocksId",
                        column: x => x.TaskBlocksId,
                        principalTable: "TaskBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskTaskBlock_Tasks_TasksId",
                        column: x => x.TasksId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskTaskBlock_TasksId",
                table: "TaskTaskBlock",
                column: "TasksId");
        }
    }
}
