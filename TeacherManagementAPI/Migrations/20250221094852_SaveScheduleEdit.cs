using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class SaveScheduleEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubjectName",
                table: "Schedules",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubjectName",
                table: "Schedules");
        }
    }
}
