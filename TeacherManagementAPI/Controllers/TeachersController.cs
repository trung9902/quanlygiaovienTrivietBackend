using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherManagementAPI.Data;
using TeacherManagementAPI.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeacherManagementAPI.models;

namespace TeacherManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeachersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/teachers
        [HttpGet]
        public async Task<ActionResult> GetTeachers()
        {
            var teachers = await _context.Teachers.ToListAsync();
            return Ok(teachers);
        }

        // GET: api/teachers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher == null)
                return NotFound("Teacher not found");

            return Ok(teacher);
        }



        // PUT: api/teachers/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateTeacher(int id, [FromForm] Teacher teacher, IFormFile imageFile)
        {
            var existingTeacher = await _context.Teachers.FindAsync(id);
            if (existingTeacher == null)
                return NotFound("Teacher not found");

            // Cập nhật các thông tin giáo viên nếu có
            if (teacher.HoTen != null)
                existingTeacher.HoTen = teacher.HoTen;
            if (teacher.GioiTinh != null)
                existingTeacher.GioiTinh = teacher.GioiTinh;
            if (teacher.NgaySinh.HasValue)
                existingTeacher.NgaySinh = teacher.NgaySinh;
            if (teacher.NoiSinh != null)
                existingTeacher.NoiSinh = teacher.NoiSinh;
            if (teacher.DiaChi != null)
                existingTeacher.DiaChi = teacher.DiaChi;
            if (teacher.SDT != null)
                existingTeacher.SDT = teacher.SDT;
            if (teacher.Email != null)
                existingTeacher.Email = teacher.Email;
            if (teacher.TrinhDoHocVan != null)
                existingTeacher.TrinhDoHocVan = teacher.TrinhDoHocVan;
            if (teacher.ChuyenNganh != null)
                existingTeacher.ChuyenNganh = teacher.ChuyenNganh;
            if (teacher.KinhNghiem != null)
                existingTeacher.KinhNghiem = teacher.KinhNghiem;
            if (teacher.ChungChi != null)
                existingTeacher.ChungChi = teacher.ChungChi;
            if (teacher.TrangThai != null)
                existingTeacher.TrangThai = teacher.TrangThai;

            // Đảm bảo thư mục images và documents tồn tại
            var imageDirectory = Path.Combine("wwwroot", "images");
            var documentDirectory = Path.Combine("wwwroot", "documents");

            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            if (!Directory.Exists(documentDirectory))
            {
                Directory.CreateDirectory(documentDirectory);
            }

            // Cập nhật hình ảnh và tài liệu nếu có
            if (imageFile != null)
            {
                var imagePath = Path.Combine(imageDirectory, imageFile.FileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                existingTeacher.AnhHoSo = imagePath;
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.Teachers.Update(existingTeacher);
            await _context.SaveChangesAsync();

            return NoContent();
        }




        // DELETE: api/teachers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
                return NotFound("Teacher not found");

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
