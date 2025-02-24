namespace TeacherManagementAPI.models
{
    public class Class
    {
        public int Id { get; set; } // Khóa chính
        public string Name { get; set; } = string.Empty; // Tên lớp
        public string Level { get; set; } // Khối học (e.g., Khối 10)
        public string Type { get; set; } // Loại hình lớp (e.g., Khoa học Tự nhiên)
        public List<string> ListSubject { get; set; } = new List<string>(); // Danh sách tên môn học
    }


}
