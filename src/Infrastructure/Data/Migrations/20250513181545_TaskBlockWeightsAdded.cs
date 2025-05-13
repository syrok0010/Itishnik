using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itishnik.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class TaskBlockWeightsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskBlocks_TaskBlockId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TaskBlockId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TaskBlockId",
                table: "Tasks");

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
                    TaskBlockId = table.Column<Guid>(type: "uuid", nullable: false),
                    TasksId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTaskBlock", x => new { x.TaskBlockId, x.TasksId });
                    table.ForeignKey(
                        name: "FK_TaskTaskBlock_TaskBlocks_TaskBlockId",
                        column: x => x.TaskBlockId,
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskTaskBlock");

            migrationBuilder.DropColumn(
                name: "Weights",
                table: "TaskBlocks");

            migrationBuilder.AddColumn<Guid>(
                name: "TaskBlockId",
                table: "Tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskBlockId",
                table: "Tasks",
                column: "TaskBlockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskBlocks_TaskBlockId",
                table: "Tasks",
                column: "TaskBlockId",
                principalTable: "TaskBlocks",
                principalColumn: "Id");
        }
    }
}
