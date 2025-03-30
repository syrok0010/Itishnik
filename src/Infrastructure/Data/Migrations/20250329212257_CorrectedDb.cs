using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itishnik.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorrectedDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Teachers_TeacherId1",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_GradedCourses_Students_StudentId1",
                table: "GradedCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_GradedTaskBlocks_Students_StudentId1",
                table: "GradedTaskBlocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Students_StudentId1",
                table: "Solutions");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Solutions_StudentId1",
                table: "Solutions");

            migrationBuilder.DropIndex(
                name: "IX_GradedTaskBlocks_StudentId1",
                table: "GradedTaskBlocks");

            migrationBuilder.DropIndex(
                name: "IX_GradedCourses_StudentId1",
                table: "GradedCourses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_TeacherId1",
                table: "Courses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teachers",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "GradedTaskBlocks");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "GradedCourses");

            migrationBuilder.DropColumn(
                name: "TeacherId1",
                table: "Courses");

            migrationBuilder.RenameTable(
                name: "Teachers",
                newName: "User");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tasks",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tasks",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "RightSolutionId",
                table: "Tasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RightSolutionId1",
                table: "Tasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "TeacherId",
                table: "Tasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TaskBlocks",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Tags",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Solutions",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Solutions",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "GradedTaskBlocks",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "GradedCourses",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "Files",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TeacherId",
                table: "Courses",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Courses",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Surname",
                table: "User",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Patronymic",
                table: "User",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "User",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "User",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "User",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EducationStartYear",
                table: "User",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducationalProgram",
                table: "User",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupNumber",
                table: "User",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_RightSolutionId1",
                table: "Tasks",
                column: "RightSolutionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TeacherId",
                table: "Tasks",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_StudentId",
                table: "Solutions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_GradedTaskBlocks_StudentId",
                table: "GradedTaskBlocks",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_GradedCourses_StudentId",
                table: "GradedCourses",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TeacherId",
                table: "Courses",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_User_CourseId",
                table: "User",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_User_TeacherId",
                table: "Courses",
                column: "TeacherId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GradedCourses_User_StudentId",
                table: "GradedCourses",
                column: "StudentId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GradedTaskBlocks_User_StudentId",
                table: "GradedTaskBlocks",
                column: "StudentId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_User_StudentId",
                table: "Solutions",
                column: "StudentId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Solutions_RightSolutionId1",
                table: "Tasks",
                column: "RightSolutionId1",
                principalTable: "Solutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_User_TeacherId",
                table: "Tasks",
                column: "TeacherId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Courses_CourseId",
                table: "User",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_User_TeacherId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_GradedCourses_User_StudentId",
                table: "GradedCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_GradedTaskBlocks_User_StudentId",
                table: "GradedTaskBlocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_User_StudentId",
                table: "Solutions");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Solutions_RightSolutionId1",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_User_TeacherId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Courses_CourseId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_RightSolutionId1",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TeacherId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Solutions_StudentId",
                table: "Solutions");

            migrationBuilder.DropIndex(
                name: "IX_GradedTaskBlocks_StudentId",
                table: "GradedTaskBlocks");

            migrationBuilder.DropIndex(
                name: "IX_GradedCourses_StudentId",
                table: "GradedCourses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_TeacherId",
                table: "Courses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_CourseId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "RightSolutionId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "RightSolutionId1",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "User");

            migrationBuilder.DropColumn(
                name: "EducationStartYear",
                table: "User");

            migrationBuilder.DropColumn(
                name: "EducationalProgram",
                table: "User");

            migrationBuilder.DropColumn(
                name: "GroupNumber",
                table: "User");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Teachers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tasks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tasks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TaskBlocks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Tags",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Solutions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                table: "Solutions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "StudentId1",
                table: "Solutions",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                table: "GradedTaskBlocks",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "StudentId1",
                table: "GradedTaskBlocks",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                table: "GradedCourses",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "StudentId1",
                table: "GradedCourses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "Files",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<Guid>(
                name: "TeacherId",
                table: "Courses",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Courses",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "TeacherId1",
                table: "Courses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Surname",
                table: "Teachers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Patronymic",
                table: "Teachers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Teachers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teachers",
                table: "Teachers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: true),
                    EducationStartYear = table.Column<int>(type: "integer", nullable: false),
                    EducationalProgram = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    GroupNumber = table.Column<int>(type: "integer", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedEmail = table.Column<string>(type: "text", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    Patronymic = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_StudentId1",
                table: "Solutions",
                column: "StudentId1");

            migrationBuilder.CreateIndex(
                name: "IX_GradedTaskBlocks_StudentId1",
                table: "GradedTaskBlocks",
                column: "StudentId1");

            migrationBuilder.CreateIndex(
                name: "IX_GradedCourses_StudentId1",
                table: "GradedCourses",
                column: "StudentId1");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TeacherId1",
                table: "Courses",
                column: "TeacherId1");

            migrationBuilder.CreateIndex(
                name: "IX_Students_CourseId",
                table: "Students",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Teachers_TeacherId1",
                table: "Courses",
                column: "TeacherId1",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GradedCourses_Students_StudentId1",
                table: "GradedCourses",
                column: "StudentId1",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GradedTaskBlocks_Students_StudentId1",
                table: "GradedTaskBlocks",
                column: "StudentId1",
                principalTable: "Students",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Students_StudentId1",
                table: "Solutions",
                column: "StudentId1",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}
