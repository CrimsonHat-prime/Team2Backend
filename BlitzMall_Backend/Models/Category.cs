namespace BlitzMall_Backend.Models
{
    public class Category
    {
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Product>? Products { get; set; }
      
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }

       
        public ICollection<Category>? ChildCategories { get; set; }
    }
}
