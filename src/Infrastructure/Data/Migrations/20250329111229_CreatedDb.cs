using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itishnik.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreatedDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    Patronymic = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId1 = table.Column<string>(type: "text", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Teachers_TeacherId1",
                        column: x => x.TeacherId1,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EducationalProgram = table.Column<string>(type: "text", nullable: false),
                    EducationStartYear = table.Column<int>(type: "integer", nullable: false),
                    GroupNumber = table.Column<int>(type: "integer", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    Patronymic = table.Column<string>(type: "text", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "TaskBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeAllowed = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskBlocks_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GradedCourses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId1 = table.Column<string>(type: "text", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Grade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradedCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradedCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GradedCourses_Students_StudentId1",
                        column: x => x.StudentId1,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    TaskBlockId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_TaskBlocks_TaskBlockId",
                        column: x => x.TaskBlockId,
                        principalTable: "TaskBlocks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TaskBlockId = table.Column<Guid>(type: "uuid", nullable: true),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_TaskBlocks_TaskBlockId",
                        column: x => x.TaskBlockId,
                        principalTable: "TaskBlocks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GradedTaskBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId1 = table.Column<string>(type: "text", nullable: true),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskBlockId = table.Column<Guid>(type: "uuid", nullable: false),
                    Feedback = table.Column<string>(type: "text", nullable: true),
                    Grade = table.Column<int>(type: "integer", nullable: false),
                    GradedCourseId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradedTaskBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradedTaskBlocks_GradedCourses_GradedCourseId",
                        column: x => x.GradedCourseId,
                        principalTable: "GradedCourses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GradedTaskBlocks_Students_StudentId1",
                        column: x => x.StudentId1,
                        principalTable: "Students",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GradedTaskBlocks_TaskBlocks_TaskBlockId",
                        column: x => x.TaskBlockId,
                        principalTable: "TaskBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Solutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId1 = table.Column<string>(type: "text", nullable: true),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Grade = table.Column<int>(type: "integer", nullable: false),
                    GradedTaskBlockId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solutions_GradedTaskBlocks_GradedTaskBlockId",
                        column: x => x.GradedTaskBlockId,
                        principalTable: "GradedTaskBlocks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Solutions_Students_StudentId1",
                        column: x => x.StudentId1,
                        principalTable: "Students",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Solutions_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TeacherId1",
                table: "Courses",
                column: "TeacherId1");

            migrationBuilder.CreateIndex(
                name: "IX_Files_TaskBlockId",
                table: "Files",
                column: "TaskBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_GradedCourses_CourseId",
                table: "GradedCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_GradedCourses_StudentId1",
                table: "GradedCourses",
                column: "StudentId1");

            migrationBuilder.CreateIndex(
                name: "IX_GradedTaskBlocks_GradedCourseId",
                table: "GradedTaskBlocks",
                column: "GradedCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_GradedTaskBlocks_StudentId1",
                table: "GradedTaskBlocks",
                column: "StudentId1");

            migrationBuilder.CreateIndex(
                name: "IX_GradedTaskBlocks_TaskBlockId",
                table: "GradedTaskBlocks",
                column: "TaskBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_GradedTaskBlockId",
                table: "Solutions",
                column: "GradedTaskBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_StudentId1",
                table: "Solutions",
                column: "StudentId1");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_TaskId",
                table: "Solutions",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_CourseId",
                table: "Students",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TaskId",
                table: "Tags",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBlocks_CourseId",
                table: "TaskBlocks",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskBlockId",
                table: "Tasks",
                column: "TaskBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskId",
                table: "Tasks",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Solutions");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "GradedTaskBlocks");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "GradedCourses");

            migrationBuilder.DropTable(
                name: "TaskBlocks");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Teachers");
        }
    }
}
