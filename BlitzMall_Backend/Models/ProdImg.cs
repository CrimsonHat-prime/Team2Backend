namespace BlitzMall_Backend.Models
{
    public class ProdImg
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public string? UrlImage { get; set; }
    }
}
