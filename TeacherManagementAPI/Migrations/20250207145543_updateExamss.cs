using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateExamss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "subject",
                table: "Exams",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "classs",
                table: "Exams",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "classs",
                table: "Exams");

            migrationBuilder.AlterColumn<string>(
                name: "subject",
                table: "Exams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
