using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TeacherManagementAPI.models;
using TeacherManagementAPI.Models;

namespace TeacherManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }


        // DbSet declarations
        public DbSet<User> Users { get; set; }
        public DbSet<Class> Class { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<scheduleData> Schedules { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<TokenBlacklist> TokenBlacklist { get; set; }





    }
}
