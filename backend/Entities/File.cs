namespace backend.Entities
{
    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int OwnerId { get; set; }
        public User Owner { get; set; }
        public DateTime UploadedAt { get; set; }
        public int Downloads { get; set; } = 0;
        public bool IsPublic { get; set; } = false;
    }
}
