using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TeacherManagementAPI.Models;
using TeacherManagementAPI.Dtos;
using TeacherManagementAPI.Data; 
using Rolee = TeacherManagementAPI.Dtos.RoleDto;
namespace TeacherManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Thêm tài khoản người dùng
        [HttpPost("add-user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUser([FromBody] RegisterDto dto)
        {
            if (_context.Users.Any(u => u.Username == dto.Username))
                return BadRequest("Username already exists!");

            // Kiểm tra nếu đã có tài khoản admin
            if (dto.Role == (int)Rolee.Admin && _context.Users.Any(u => u.Role == (int)Rolee.Admin))
            {
                return BadRequest("An admin account already exists. Only one admin is allowed.");
            }

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.HoTen,
                Email = dto.Email,
                Phone = dto.Sdt,
                Role = (Models.Role)dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User added successfully.");
        }

        // Sửa thông tin tài khoản người dùng
        [HttpPut("update-user/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            // Tìm người dùng theo ID
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Cập nhật các trường thông tin khác (nếu có)
            user.Username = dto.Username ?? user.Username;
            user.FullName = dto.fullName ?? user.FullName;
            user.Email = dto.Email ?? user.Email;
            user.Phone = dto.phone ?? user.Phone;

            // Xử lý mật khẩu
            if (!string.IsNullOrEmpty(dto.Password))
            {
                // Nếu có mật khẩu mới, mã hóa và cập nhật
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }
           

            // Cập nhật vai trò (Role) nếu có giá trị
            if (dto.Role.HasValue)
            {
                user.Role = (Models.Role)dto.Role.Value;
            }

            // Cập nhật dữ liệu vào database
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Phản hồi kết quả
            return Ok("User updated successfully.");
        }


        // Xóa tài khoản người dùng
        [HttpDelete("delete-user/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Không cho phép xóa tài khoản admin
            if (user.Role == (int)Rolee.Admin)
            {
                return BadRequest("Cannot delete the Admin account.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User deleted successfully.");
        }

        // Lấy danh sách người dùng
        [HttpGet("get-users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
    }
}
