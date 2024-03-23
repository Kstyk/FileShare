namespace backend.Entities
{
    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
