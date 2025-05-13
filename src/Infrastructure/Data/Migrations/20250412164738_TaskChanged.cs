using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itishnik.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TaskChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Tasks_TaskId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TaskId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "Tasks",
                newName: "PreviousVersionId");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Tasks",
                newName: "Text");

            migrationBuilder.AddColumn<Guid>(
                name: "FirstVersionId",
                table: "Tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_FirstVersionId",
                table: "Tasks",
                column: "FirstVersionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_PreviousVersionId",
                table: "Tasks",
                column: "PreviousVersionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Tasks_FirstVersionId",
                table: "Tasks",
                column: "FirstVersionId",
                principalTable: "Tasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Tasks_PreviousVersionId",
                table: "Tasks",
                column: "PreviousVersionId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Tasks_FirstVersionId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Tasks_PreviousVersionId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_FirstVersionId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_PreviousVersionId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "FirstVersionId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Tasks",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "PreviousVersionId",
                table: "Tasks",
                newName: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskId",
                table: "Tasks",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Tasks_TaskId",
                table: "Tasks",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }
    }
}
