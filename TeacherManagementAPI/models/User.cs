using TeacherManagementAPI.models;

namespace TeacherManagementAPI.Models
{
    public enum Role
    {
        Admin,
        Teacher,
        Supervisor
    }
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Role Role { get; set; } // Chỉ có thể là "Teacher" hoặc "Supervisor"
        public ICollection<Teacher> Teachers { get; set; } // Một user có thể quản lý nhiều giáo viên
        public string? OtpCode { get; internal set; }
        public DateTime? OtpExpiry { get; internal set; }
    }
}
