using System.ComponentModel.DataAnnotations;

namespace BlitzMall_Backend.DTOs.Order
{
    public class CreateOrderDto
    {
        [Range(1, int.MaxValue)]
        public int AddressId { get; set; }
    }
}
