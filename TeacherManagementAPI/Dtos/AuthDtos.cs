using System.Data;

namespace TeacherManagementAPI.Dtos
{
    public enum RoleDto
    {
        Admin,
        Teacher,
    }
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string Sdt { get; set; }
        public RoleDto Role { get; set; }  // Sử dụng enum Role thay vì string
        public List<string> Subject { get; set; } // Đảm bảo có trường Subject cho Teacher
    }
    // Thêm lớp UpdateUserDto
    public class UpdateUserDto
    {
        public string Username { get; set; }
        public string? Password { get; set; }
        public string fullName { get; set; }
        public string Email { get; set; }
        public string phone { get; set; }
        public RoleDto? Role { get; set; }  // Role có thể null nếu không thay đổi
    }
}
