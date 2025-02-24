using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateSubjectttt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "level",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "level",
                table: "Subjects");
        }
    }
}
