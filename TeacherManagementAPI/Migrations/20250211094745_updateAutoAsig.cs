using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateAutoAsig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Subjects",
                newName: "Types");

            migrationBuilder.AddColumn<string>(
                name: "lopChuNhiem",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "lopChuNhiemId",
                table: "Teachers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lopChuNhiem",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "lopChuNhiemId",
                table: "Teachers");

            migrationBuilder.RenameColumn(
                name: "Types",
                table: "Subjects",
                newName: "Type");
        }
    }
}
