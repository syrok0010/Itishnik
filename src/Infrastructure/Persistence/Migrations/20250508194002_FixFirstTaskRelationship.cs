using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itishnik.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixFirstTaskRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_FirstVersionId",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_FirstVersionId",
                table: "Tasks",
                column: "FirstVersionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_FirstVersionId",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_FirstVersionId",
                table: "Tasks",
                column: "FirstVersionId",
                unique: true);
        }
    }
}
