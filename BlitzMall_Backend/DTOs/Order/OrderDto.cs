namespace BlitzMall_Backend.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string? OrderStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int AddressId { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}