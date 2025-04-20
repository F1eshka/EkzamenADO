namespace EkzamenADO.Models
{
    public class Ad
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageFileName { get; set; }
        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }
        public int CategoryId { get; set; }
    }
}
