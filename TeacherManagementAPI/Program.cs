using Microsoft.EntityFrameworkCore;
using TeacherManagementAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TeacherManagementAPI.Models;
using System.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Thêm cấu hình JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Lấy giá trị từ appsettings.json
        ValidAudience = builder.Configuration["Jwt:Audience"],  // Lấy giá trị từ appsettings.json
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))  // Lấy Key từ appsettings.json
    };
});

// Đăng ký DbContext với chuỗi kết nối
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký các dịch vụ khác
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình CORS nếu cần
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()   // Cho phép bất kỳ origin nào
               .AllowAnyMethod()   // Cho phép bất kỳ phương thức HTTP nào
               .AllowAnyHeader();  // Cho phép bất kỳ header nào
    });
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TeacherManagementAPI", Version = "v1" });
    // Thêm hỗ trợ Bearer Token vào Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Please enter JWT with Bearer into field"
    });

    // Yêu cầu xác thực JWT cho tất cả các yêu cầu API
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Cấu hình middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Thêm middleware kiểm tra danh sách đen
app.UseMiddleware<TokenBlacklistMiddleware>();
// Sử dụng middleware CORS (nếu cần)
app.UseCors("AllowAllOrigins"); // Nếu bạn đã cấu hình CORS
app.UseCors(builder =>
    builder.WithOrigins("http://localhost:8080") // 🔹 Thay bằng URL Vue.js của bạn
           .AllowAnyMethod()
           .AllowAnyHeader()
           .WithExposedHeaders("Content-Disposition")); // 🔥 Cho phép header này

app.UseAuthentication();  // Thêm middleware Authentication trước Authorization
app.UseAuthorization();   // Cấu hình Authorization
app.UseStaticFiles();    // Sử dụng Static Files (wwwroot)
app.MapControllers();  // Đảm bảo các controller được ánh xạ đúng
await CreateDefaultAdminUser(app);
app.Run();

async Task CreateDefaultAdminUser(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Kiểm tra xem đã có tài khoản admin chưa
        var existingAdmin = await context.Users
                                         .FirstOrDefaultAsync(u => u.Username == "admin");

        if (existingAdmin == null)
        {
            // Tạo tài khoản admin mới
            var adminUser = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"), // Mã hóa mật khẩu
                FullName = "Admin",
                Email = "admin@example.com",
                Phone = "123456789",
                Role = Role.Admin // Hoặc bạn có thể dùng Enum cho Role nếu có
            };

            // Thêm và lưu tài khoản admin vào cơ sở dữ liệu
            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }
    }
}
