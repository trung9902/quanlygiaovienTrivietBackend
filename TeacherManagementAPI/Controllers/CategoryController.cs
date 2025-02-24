using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using TeacherManagementAPI.Data;
using TeacherManagementAPI.models;

namespace TeacherManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            category.CreatedDate = DateTime.Now;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Tự động tạo Slug từ Name
            category.Slug = GenerateSlug(category.Name);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        // Hàm tạo Slug
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


        // PATCH: api/Category/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] JsonPatchDocument<Category> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest("Patch document is null.");
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Áp dụng thay đổi từ patchDoc vào thực thể
            patchDoc.ApplyTo(category, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
