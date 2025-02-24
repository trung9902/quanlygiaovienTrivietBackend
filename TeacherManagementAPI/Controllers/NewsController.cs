using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using TeacherManagementAPI.Data;
using TeacherManagementAPI.models;

namespace TeacherManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/News
        [HttpGet]
        public async Task<IActionResult> GetNews()
        {
            var news = await _context.News.ToListAsync();
            return Ok(news);
        }

        // GET: api/News/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsById(int id)
        {
            var news = await _context.News.FirstOrDefaultAsync(n => n.Id == id);
            if (news == null)
            {
                return NotFound();
            }
            return Ok(news);
        }

        // POST: api/News
        [HttpPost]
        public async Task<IActionResult> CreateNews([FromForm] News news, IFormFile image, IFormFile document)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (image != null && image.Length > 0)
            {
                // Kiểm tra loại file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(image.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Invalid file type.");
                }

                // Tạo tên file duy nhất
                var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                var imagePath = Path.Combine("wwwroot/images", uniqueFileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Lưu đường dẫn ảnh vào database
                news.Thumbnail = "/images/" + uniqueFileName;
            }
            if (document != null && document.Length > 0)
            {
                // Kiểm tra loại file
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                var fileExtension = Path.GetExtension(document.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Invalid file type.");
                }

                // Tạo tên file duy nhất
                var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                var documentPath = Path.Combine("wwwroot/documents", uniqueFileName);

                using (var stream = new FileStream(documentPath, FileMode.Create))
                {
                    await document.CopyToAsync(stream);
                }

                // Lưu đường dẫn tệp vào database
                news.DocumentPath = "/documents/" + uniqueFileName;
            }
            news.Author = "Admin";
            news.PublishedDate = DateTime.Now;
            news.Slug = GenerateSlug(news.Title);
            _context.News.Add(news);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetNewsById), new { id = news.Id }, news);
        }

        private string GenerateSlug(string name)
        {
            return name
                .ToLower() // Chuyển chuỗi về chữ thường
                .Trim() // Loại bỏ khoảng trắng đầu và cuối
                .Normalize(NormalizationForm.FormD) // Chuẩn hóa Unicode để tách dấu
                .Where(c => char.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark) // Loại bỏ dấu
                .Aggregate("", (current, c) => current + c) // Gộp lại thành chuỗi
                .Replace("đ", "d") // Thay 'đ' thành 'd'
                .Replace(" ", "-") // Thay khoảng trắng thành dấu '-'
                .Replace("--", "-") // Xử lý nếu có 2 dấu '-' liên tiếp
                .Replace("?", "") // Loại bỏ các ký tự đặc biệt
                .Replace("/", "") // Loại bỏ các ký tự đặc biệt khác
                .Replace("&", "")
                .Replace("#", "")
                .Replace("%", "")
                .Replace(".", "")
                .Replace(",", "");
        }
        // PATCH: api/News/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateNews(int id, [FromForm] News updatedNews, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin bài viết
            if (updatedNews.Title != null) news.Title = updatedNews.Title;
            if (updatedNews.Content != null) news.Content = updatedNews.Content;
            news.Status = updatedNews.Status;
            news.CategoryId = updatedNews.CategoryId;

            if (image != null && image.Length > 0)
            {
                // Kiểm tra loại file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(image.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Invalid file type.");
                }

                // Tạo tên file duy nhất
                var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                var imagePath = Path.Combine("wwwroot/images", uniqueFileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Lưu đường dẫn ảnh vào database
                news.Thumbnail = "/images/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // DELETE: api/News/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
