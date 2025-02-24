using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TeacherManagementAPI.Data;
using TeacherManagementAPI.Models;
using TeacherManagementAPI.Dtos;
using BCrypt.Net;
using Role = TeacherManagementAPI.Dtos.RoleDto;
using TeacherManagementAPI.models;
using System.Numerics;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;

[ApiController]
[Route("api/")]
public class TeachersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public TeachersController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        // Kiểm tra nếu username đã tồn tại
        if (_context.Users.Any(u => u.Username == dto.Username))
            return BadRequest("Username already exists!");

        // Kiểm tra thông tin giáo viên
        if (dto.Role == Role.Teacher)
        {
            if (string.IsNullOrEmpty(dto.HoTen) || dto.Subject == null || !dto.Subject.Any())
                return BadRequest("HoTen and at least one Subject are required for Teacher role.");
        }

        // Tạo đối tượng User mới
        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FullName = dto.HoTen, // Gán họ tên từ Teacher
            Email = dto.Email,
            Phone = dto.Sdt,
            Role = (TeacherManagementAPI.Models.Role)dto.Role // Sử dụng kiểu Role trực tiếp
        };

        // Thêm người dùng vào CSDL và lưu
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // 
        if (dto.Role == Role.Teacher)
        {
            var teacher = new Teacher
            {
                HoTen = dto.HoTen,
                Subject = dto.Subject, // Lưu danh sách môn học vào bảng Teacher
                UserId = user.Id, // Liên kết với UserId
                Email = dto.Email,
                SDT = dto.Sdt
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
        }

        // Tạo và trả về JWT token
        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        // Kiểm tra nếu username có tồn tại trong CSDL
        var user = _context.Users.FirstOrDefault(u => u.Username == dto.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        // Tạo và trả về JWT token
        var token = GenerateJwtToken(user);
        return Ok(new
        {
            Token = token,
            Role = user.Role.ToString()
        });
    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("Token không hợp lệ.");
        }

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
        if (jwtToken == null)
        {
            return BadRequest("Token không hợp lệ.");
        }

        var expiryDate = jwtToken.ValidTo;

        var tokenBlacklist = new TokenBlacklist
        {
            Token = token,
            ExpiryDate = expiryDate
        };

        _context.TokenBlacklist.Add(tokenBlacklist);
        await _context.SaveChangesAsync();

        return Ok("Đăng xuất thành công.");
    }
    // Hàm tạo JWT Token
    private string GenerateJwtToken(User user)
    {
        // Cấu hình key từ appsettings.json
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? string.Empty));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Claims chứa thông tin của người dùng
        var claims = new[]
        {
              new Claim("UserId", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())  // Thêm Role vào claims
        };

        // Tạo và ký JWT Token
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return Ok(new { success = false, message = "Email không tồn tại trong hệ thống!" });
        }

        string otp = GenerateOtp();

        bool emailSent = SendOtpEmail(email, otp);
        if (!emailSent)
        {
            return Ok(new { success = false, message = "Gửi OTP thất bại, vui lòng thử lại!" });
        }

        user.OtpCode = otp;
        user.OtpExpiry = DateTime.Now.AddMinutes(5); // OTP có hiệu lực trong 5 phút
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "OTP đã được gửi thành công, vui lòng kiểm tra email!" });
    }



    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(string email, string otp, string newPassword)
    {
        // Kiểm tra xem email có tồn tại không
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return Ok(new { success = false, message = "Email không tồn tại trong hệ thống!" });
        }


        // Kiểm tra OTP
        if (user.OtpCode != otp || user.OtpExpiry < DateTime.Now)
        {
            return Ok(new { success = false, message = "OTP không hợp lệ hoặc đã hết hạn!" });
        }

        // Mã hóa mật khẩu mới
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

        // Xóa OTP sau khi đặt lại mật khẩu
        user.OtpCode = null;
        user.OtpExpiry = null;

        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Đặt lại mật khẩu thành công!" });
    }


    private string GenerateOtp()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    private bool SendOtpEmail(string email, string otp)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Gmail", "ngthanhtrung09092002@gmail.com"));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = "OTP for password reset";
            message.Body = new TextPart("plain")
            {
                Text = $"Your OTP is: {otp}"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("ngthanhtrung09092002@gmail.com", "ngcv txmb ozha xvht");
                client.Send(message);
                client.Disconnect(true);
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }
}

