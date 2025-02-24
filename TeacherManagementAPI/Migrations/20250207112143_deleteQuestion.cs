using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class deleteQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Subjects_SubjectId",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Users_CreatedByUserId",
                table: "Exams");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Exams_CreatedByUserId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_SubjectId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Exams");

            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "Exams",
                newName: "ShareCount");

            migrationBuilder.RenameColumn(
                name: "Semester",
                table: "Exams",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "SchoolYear",
                table: "Exams",
                newName: "FileUrl");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Exams",
                newName: "DownloadCount");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Exams",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Exams");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Exams",
                newName: "Semester");

            migrationBuilder.RenameColumn(
                name: "ShareCount",
                table: "Exams",
                newName: "SubjectId");

            migrationBuilder.RenameColumn(
                name: "FileUrl",
                table: "Exams",
                newName: "SchoolYear");

            migrationBuilder.RenameColumn(
                name: "DownloadCount",
                table: "Exams",
                newName: "CreatedByUserId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Exams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Chapter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ExamId = table.Column<int>(type: "int", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exams_CreatedByUserId",
                table: "Exams",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_SubjectId",
                table: "Exams",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ExamId",
                table: "Questions",
                column: "ExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Subjects_SubjectId",
                table: "Exams",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Users_CreatedByUserId",
                table: "Exams",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
