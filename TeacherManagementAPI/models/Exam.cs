using System.ComponentModel.DataAnnotations;
using TeacherManagementAPI.Models;

namespace TeacherManagementAPI.models
{
    public class Exam
    {
        public int Id { get; set; }

        public string Title { get; set; }  // Tên đề kiểm tra

        public DateTime CreatedAt { get; set; } = DateTime.Now;  // Ngày tạo
        public int? subject { get; set; }  // Môn học
        public string? subjectsub { get; set; }
        public int? classs { get; set; }  // Lớp học
        public string? classsub { get; set; }
        public int CreatedBy { get; set; }  // Người tạo
        public string? CreatedByName { get; set; }  // Tên người tạo

        public int? DownloadCount { get; set; } // Số lượt tải xuống

        public int? ShareCount { get; set; } // Số lượt chia sẻ
        public string FileUrl { get; set; }  // Đường dẫn file
    }
}
