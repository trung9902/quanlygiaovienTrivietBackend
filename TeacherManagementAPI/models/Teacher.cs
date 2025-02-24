using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeacherManagementAPI.Models;

namespace TeacherManagementAPI.models
{
    public class Teacher
    {
        [Key]
        public int MaGiaoVien { get; set; } // ID giáo viên
        public string? HoTen { get; set; }
        public string? GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? NoiSinh { get; set; }
        public string? DiaChi { get; set; }
        public string? SDT { get; set; }
        public string? Email { get; set; }
        public string? TrinhDoHocVan { get; set; }
        public string? ChuyenNganh { get; set; }
        public string? KinhNghiem { get; set; }
        public string? ChungChi { get; set; }
        public string? TrangThai { get; set; } // Trạng thái làm việc: "Đang làm việc", "Nghỉ việc", v.v.
        public string? AnhHoSo { get; set; } // Đường dẫn đến hình ảnh hồ sơ
        public int? lopChuNhiemId { get; set; } // ID lớp chủ nhiệm
        public string? lopChuNhiem { get; set; } // Tên lớp chủ nhiệm

        public List<string> Subject { get; set; } // Môn học

        public int? UserId { get; set; } // Khóa ngoại

        [ForeignKey("UserId")] // Định nghĩa khóa ngoại
        public User? User { get; set; } // Quan hệ với bảng User
    }
}
