using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TeacherManagementAPI.Data;
using TeacherManagementAPI.models;

namespace TeacherManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSchedule()
        {
            var schedule = await _context.Schedules.ToListAsync();
            return Ok(schedule);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            return Ok(schedule);
        }
        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetScheduleByTeacherId(int teacherId)
        {
            var schedules = await _context.Schedules
                .Where(s => s.TeacherId == teacherId)
                .ToListAsync();

            if (schedules == null || !schedules.Any())
            {
                return NotFound(new { message = "Không tìm thấy lịch học nào cho giáo viên này." });
            }

            return Ok(schedules);
        }
        [HttpPost]
        public async Task<IActionResult> SaveSchedule([FromBody] scheduleData schedule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem SubjectId có giá trị hợp lệ hay không
            if (schedule.SubjectId == null)
            {
                return BadRequest(new { message = "SubjectId không được để trống." });
            }

            // Tìm đối tượng Subject trong cơ sở dữ liệu
            var subject = await _context.Subjects.FindAsync(schedule.SubjectId);
            if (subject == null)
            {
                return NotFound(new { message = "Không tìm thấy môn học với SubjectId đã cung cấp." });
            }

            // Gán tên môn học cho schedule
            schedule.SubjectName = subject.Name;

            // Thêm schedule vào cơ sở dữ liệu
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetScheduleById), new { id = schedule.Id }, schedule);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] scheduleData schedule)
        {
            if (id != schedule.Id)
            {
                return BadRequest();
            }

            _context.Entry(schedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScheduleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("DeleteAll")]
        public async Task<IActionResult> DeleteAllSchedules()
        {
            var allSchedules = await _context.Schedules.ToListAsync();
            if (!allSchedules.Any())
            {
                return NotFound(new { message = "Không có lịch nào để xóa." });
            }

            _context.Schedules.RemoveRange(allSchedules);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        private bool ScheduleExists(int id)
        {
            return _context.Schedules.Any(e => e.Id == id);
        }
    }
}
