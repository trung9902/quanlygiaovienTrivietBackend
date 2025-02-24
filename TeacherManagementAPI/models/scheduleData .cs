using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TeacherManagementAPI.models
{
    public class scheduleData
    {
        [Key]
        public int Id { get; set; }  // Khóa chính, tự động tăng


        public int ClassId { get; set; }  // ID của lớp học


        public int? SubjectId { get; set; }  // ID của môn học
        public string? SubjectName { get; set; }  // Tên môn học


        public int TeacherId { get; set; }  // ID của giáo viên phụ trách


        public string? DayOfWeek { get; set; }  // Thứ trong tuần (Thứ 2 - Thứ 7)

        public int Period { get; set; }  // Tiết học (1-9)


        public string? TimeSlot { get; set; }  // Khung giờ cụ thể


        public int Semester { get; set; }  // Học kỳ (1 hoặc 2)

        public string? SchoolYear { get; set; }  // Năm học (VD: "2024-2025")


        public string? RoomNumber { get; set; }  // Phòng học


        public DateTime CreatedAt { get; set; }  // Thời gian tạo bản ghi

        public DateTime? UpdatedAt { get; set; }  // Thời gian cập nhật bản ghi
    }
}
