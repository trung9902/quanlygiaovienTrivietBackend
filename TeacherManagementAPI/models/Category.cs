﻿namespace TeacherManagementAPI.models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Slug { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

    }

}
