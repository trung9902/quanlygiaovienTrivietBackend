namespace TeacherManagementAPI.models
{
    public class Subject
    {
        public int Id { get; set; } // Khóa chính
        public string Name { get; set; } = string.Empty;// Tên môn học (e.g., Ngữ văn)
        public int level { get; set; } // Khối học (e.g., Khối 10)
        public int SoTiet { get; set; } // Số tiết
        public List<string> Types { get; set; } // Loại môn học (e.g., Chung)
  
    }
}