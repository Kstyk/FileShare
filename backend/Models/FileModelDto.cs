namespace backend.Models
{
    public class FileModelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime UploadedAt { get; set; }
        public int Downloads { get; set; }
        public bool IsPublic { get; set; }
    }
}
