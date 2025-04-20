namespace EkzamenADO.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public byte[] HashedPassword { get; set; }
        public byte[] Salt { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
