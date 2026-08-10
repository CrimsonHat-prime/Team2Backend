using System.ComponentModel.DataAnnotations;

namespace BlitzMall_Backend.Models
{
    public class User
    {


      namespace BlitzMall_Backend.Models
    {
        public class User
        {
            public int Id { get; set; }

            public string? Name { get; set; }

            public string? Status { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public DateTime CreatedAt { get; set; }

            public DateTime? UpdatedAt { get; set; }

            public Cart? Cart { get; set; }

         
            public Seller? Seller { get; set; }
        }
    }

}
}
