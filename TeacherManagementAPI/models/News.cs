namespace TeacherManagementAPI.models
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime PublishedDate { get; set; }
        public bool Status { get; set; }
        public string? Thumbnail { get; set; }
        public string? Slug { get; set; }

        public int? CategoryId { get; set; }
        public string? DocumentPath { get; set; } // Thêm thuộc tính này để lưu đường dẫn tệp PDF hoặc Word

    }

}
