using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itishnik.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReferenceSolutionInTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RightSolutionId",
                table: "Tasks");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceSolutionText",
                table: "Tasks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceSolutionText",
                table: "Tasks");

            migrationBuilder.AddColumn<Guid>(
                name: "RightSolutionId",
                table: "Tasks",
                type: "uuid",
                nullable: true);
        }
    }
}
