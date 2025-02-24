using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeacherManagementAPI.Data;
using TeacherManagementAPI.models;
using TeacherManagementAPI.Models;

namespace TeacherManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 📌 Lấy tất cả đề kiểm tra
        [HttpGet]
        public async Task<IActionResult> GetExams()
        {
            var exams = await _context.Exams.ToListAsync();
            return Ok(exams);
        }

        // 📌 Thêm đề kiểm tra (upload file)
        [HttpPost]
        public async Task<IActionResult> CreateExam([FromForm] Exam Exam, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File không hợp lệ.");
            }

            var filePath = Path.Combine("wwwroot/documents", file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            Exam.FileUrl = "/uploads/" + file.FileName;
            Exam.CreatedAt = DateTime.Now.Date;



            _context.Exams.Add(Exam);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetExamById), new { id = Exam.Id }, Exam);
        }

        // 📌 Lấy chi tiết đề kiểm tra
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExamById(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
            {
                return NotFound("Không tìm thấy đề kiểm tra.");
            }
            return Ok(exam);
        }

        // 📌 Cập nhật đề kiểm tra
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateExam(int id, [FromForm] Exam updatedExam, IFormFile file)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
            {
                return NotFound("Không tìm thấy đề kiểm tra.");
            }

            if (file != null && file.Length > 0)
            {
                var filePath = Path.Combine("wwwroot/documents", file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                exam.FileUrl = "/uploads/" + file.FileName;
            }

            exam.Title = updatedExam.Title;
            await _context.SaveChangesAsync();
            return Ok("Cập nhật thành công.");
        }

        // 📌 Xóa đề kiểm tra
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
            {
                return NotFound("Không tìm thấy đề kiểm tra.");
            }

            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
            return Ok("Xóa thành công.");
        }

        // 📌 Tăng số lượt tải xuống
        [HttpPost("Download/{id}")]
        public async Task<IActionResult> IncrementDownloadCount(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
            {
                return NotFound("Không tìm thấy đề kiểm tra.");
            }

            exam.DownloadCount++;
            await _context.SaveChangesAsync();
            return Ok("Cập nhật lượt tải xuống thành công.");
        }

        // 📌 Tăng số lượt chia sẻ
        [HttpPost("Share/{id}")]
        public async Task<IActionResult> IncrementShareCount(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
            {
                return NotFound("Không tìm thấy đề kiểm tra.");
            }

            exam.ShareCount++;
            await _context.SaveChangesAsync();
            return Ok("Cập nhật lượt chia sẻ thành công.");
        }
        [HttpGet("DownloadFile/{id}")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
            {
                return NotFound("Không tìm thấy đề kiểm tra.");
            }

            var filePath = Path.Combine("wwwroot/documents", Path.GetFileName(exam.FileUrl));
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Không tìm thấy tệp.");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            // 🔹 Thêm header `Content-Disposition`
            Response.Headers["Content-Disposition"] = $"attachment; filename=\"{Path.GetFileName(exam.FileUrl)}\"";

            return File(memory, "application/octet-stream");
        }

    }
}
