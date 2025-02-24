using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateteacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaiLieuLienQuan",
                table: "Teachers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaiLieuLienQuan",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
