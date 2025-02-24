using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using TeacherManagementAPI.Data;
using TeacherManagementAPI.models;

namespace TeacherManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachingAssignmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeachingAssignmentController(ApplicationDbContext context)
        {
            _context = context;
        }
        // thêm môn học
        // Thêm môn học
        [HttpPost("AddSubject")]
        public async Task<IActionResult> AddSubject([FromBody] Subject subject)
        {
            if (string.IsNullOrWhiteSpace(subject.Name) || subject.Types == null || !subject.Types.Any())
            {
                return BadRequest(new { message = "Tên môn học và loại môn học không được để trống hoặc chỉ chứa khoảng trắng." });
            }

            // Tìm môn học cùng tên và khối trong CSDL
            var existingSubject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.Name == subject.Name && s.level == subject.level);

            if (existingSubject != null)
            {
                // Kiểm tra xem có trùng ít nhất một type không
                bool hasDuplicateType = subject.Types.Any(type => existingSubject.Types.Contains(type));

                if (hasDuplicateType)
                {
                    return BadRequest(new { message = $"Môn học '{subject.Name}' với các loại '{string.Join(", ", subject.Types)}' đã tồn tại trong khối {subject.level}." });
                }

                // Nếu môn học cùng tên nhưng có loại khác, cập nhật danh sách loại
                existingSubject.Types.AddRange(subject.Types);
                existingSubject.Types = existingSubject.Types.Distinct().ToList(); // Loại bỏ trùng lặp

                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Môn học đã được cập nhật loại môn học.", subject = existingSubject });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = $"Lỗi khi cập nhật môn học: {ex.Message}" });
                }
            }

            try
            {
                // Thêm môn học mới vào cơ sở dữ liệu
                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetSubjectById), new { id = subject.Id }, subject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi thêm môn học: {ex.Message}" });
            }
        }




        private List<string> GetSubjectsDetailByType(string lever, string type, List<Subject> allSubjects)
        {
            List<string> result = new List<string>();

            // Lấy môn bắt buộc theo khối
            List<string> monHocBatBuoc = new List<string>();
            if (lever == "10" || lever == "11")
            {
                monHocBatBuoc = allSubjects
                    .Where(subject => subject.Types.Contains("Bắt buộc 10-11"))
                    .Select(subject => subject.Name)
                    .ToList();
            }
            else if (lever == "khối 12")
            {
                monHocBatBuoc = allSubjects
                    .Where(subject => subject.Types.Contains("Bắt buộc 12"))
                    .Select(subject => subject.Name)
                    .ToList();
            }
            result.AddRange(monHocBatBuoc);

            // Danh sách các type cần lọc
            List<string> typesToAdd = new List<string>();
            switch (type)
            {
                case "Ban KHXH":
                    typesToAdd.Add("Ban KHXH");
                    break;

                case "Ban TN":
                    typesToAdd.Add("Ban TN");
                    break;

                case "Ban TN (Khối A)":
                    typesToAdd.Add("Ban TN (Khối A)");
                    break;

                case "Ban TN (Khối A1)":
                    typesToAdd.Add("Ban TN (Khối A1)");
                    break;

                case "Ban XH":
                    typesToAdd.Add("Ban XH");
                    break;
            }

            // Thêm các môn học theo type (cho phép nhiều type trên một môn)
            var monHocTuChon = allSubjects
                .Where(subject => subject.Types.Any(t => typesToAdd.Contains(t)))
                .Select(subject => subject.Name)
                .ToList();

            result.AddRange(monHocTuChon);
            // Loại bỏ các môn học trùng lặp
            result = result.Distinct().ToList();
            // Trả về danh sách đã trộn ngẫu nhiên
            return ShuffleArray(result);
        }




        // Hàm xáo trộn danh sách (giả lập chức năng shuffleArray)
        // Hàm xáo trộn danh sách (giả lập chức năng shuffleArray)
        private List<string> ShuffleArray(List<string> list)
        {
            return list.OrderBy(_ => Guid.NewGuid()).ToList();
        }



        // Controller hoặc API method để thêm lớp học
        [HttpPost("AddClass")]
        public async Task<IActionResult> AddClass([FromBody] Class Class)
        {
            if (Class == null)
            {
                return BadRequest("Thông tin lớp học không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(Class.Name) || string.IsNullOrWhiteSpace(Class.Level))
            {
                return BadRequest("Tên lớp và khối học là bắt buộc.");
            }

            try
            {
                // Lấy danh sách tất cả môn học từ cơ sở dữ liệu
                var allSubjects = await _context.Subjects.ToListAsync();
                if (!allSubjects.Any())
                {
                    return StatusCode(500, "Không có môn học nào trong cơ sở dữ liệu.");
                }

                // Gán danh sách môn học phù hợp vào lớp học
                Class.ListSubject = GetSubjectsDetailByType(Class.Level, Class.Type, allSubjects);
                if (!Class.ListSubject.Any())
                {
                    return StatusCode(500, $"Không có môn học phù hợp với loại {Class.Type}.");
                }

                // Thêm lớp học vào cơ sở dữ liệu
                _context.Class.Add(Class);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetClassById), new { id = Class.Id }, Class);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Lỗi cơ sở dữ liệu: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi không xác định: {ex.Message}");
            }
        }



        [HttpGet("ClassAll")]
        public async Task<IActionResult> GetClassAll()
        {
            var classes = await _context.Class.ToListAsync();

            if (classes == null || !classes.Any())
            {
                // Nếu không có lớp học, trả về Ok với thông báo.
                return Ok(new { message = "Không có lớp học nào" });
            }

            // Nếu có lớp học, trả về danh sách lớp học.
            return Ok(classes);
        }


        [HttpGet("SubjectALl")]
        public async Task<IActionResult> GetSubjectAll()
        {
            var subject = await _context.Subjects.ToListAsync();
            if (subject == null)
            {
                return NotFound($"không có môn hoc nào");
            }

            return Ok(subject);
        }
        // Lấy môn học theo ID
        [HttpGet("Subject/{id}")]
        public async Task<IActionResult> GetSubjectById(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound($"Không tìm thấy môn học với ID = {id}");
            }

            return Ok(subject);
        }

        // Lấy lớp học theo ID
        [HttpGet("Class/{id}")]
        public async Task<IActionResult> GetClassById(int id)
        {
            var Class = await _context.Class.FindAsync(id);
            if (Class == null)
            {
                return NotFound($"Không tìm thấy lớp học với ID = {id}");
            }

            return Ok(Class);
        }

    }
}
