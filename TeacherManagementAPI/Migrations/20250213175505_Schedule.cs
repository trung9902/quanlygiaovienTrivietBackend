using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class Schedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Class",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "Teacher",
                table: "Schedules",
                newName: "TimeSlot");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "Schedules",
                newName: "SchoolYear");

            migrationBuilder.RenameColumn(
                name: "Day",
                table: "Schedules",
                newName: "RoomNumber");

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Schedules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Semester",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Schedules",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "Semester",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "TimeSlot",
                table: "Schedules",
                newName: "Teacher");

            migrationBuilder.RenameColumn(
                name: "SchoolYear",
                table: "Schedules",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "RoomNumber",
                table: "Schedules",
                newName: "Day");

            migrationBuilder.AddColumn<string>(
                name: "Class",
                table: "Schedules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
