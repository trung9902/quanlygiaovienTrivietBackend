using System.ComponentModel.DataAnnotations;

namespace TeacherManagementAPI.models
{
    public class TokenBlacklist
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
