using System.Security.Claims;
using BlitzMall_Backend.Data;
using BlitzMall_Backend.DTOs.Order;
using BlitzMall_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace BlitzMall_Backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(
            AppDbContext db,
            IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        private int? GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(userIdClaim, out int userId)
                ? userId
                : null;
        }

        public async Task<List<OrderDto>> GetAllAsync()
        {
            return await _db.Orders
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus,
                    CreatedDate = o.CreatedDate,
                    UpdatedDate = o.UpdatedDate,
                    AddressId = o.AddressId,
                    Items = o.OrderItems!
                        .Select(i => new OrderItemDto
                        {
                            Id = i.Id,
                            OrderId = i.OrderId,
                            ProductId = i.ProductId,
                            Quantity = i.Quantity,
                            UnitPrice = i.UnitPrice
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            return await _db.Orders
                .Where(o => o.Id == id)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus,
                    CreatedDate = o.CreatedDate,
                    UpdatedDate = o.UpdatedDate,
                    AddressId = o.AddressId,
                    Items = o.OrderItems!
                        .Select(i => new OrderItemDto
                        {
                            Id = i.Id,
                            OrderId = i.OrderId,
                            ProductId = i.ProductId,
                            Quantity = i.Quantity,
                            UnitPrice = i.UnitPrice
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<OrderDto?> CreateAsync(CreateOrderDto dto)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return null;
            }

            var address = await _db.Addresses
                .FirstOrDefaultAsync(a =>
                    a.Id == dto.AddressId &&
                    a.UserId == userId);

            var cart = await _db.Carts
                .Include(c => c.CartItems!)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (address == null || cart?.CartItems == null || !cart.CartItems.Any())
            {
                return null;
            }

            var order = new Order
            {
                UserId = userId.Value,
                AddressId = dto.AddressId,
                OrderStatus = "Pending",
                CreatedDate = DateTime.UtcNow,
                TotalAmount = 0,
                OrderItems = new List<OrderItem>()
            };

            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product!.Price
                };

                order.OrderItems.Add(orderItem);
                order.TotalAmount +=
                    orderItem.Quantity * orderItem.UnitPrice;
            }

            _db.Orders.Add(order);
            _db.CartItems.RemoveRange(cart.CartItems);

            cart.UpdatedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return await GetByIdAsync(order.Id);
        }

        public async Task<OrderDto?> UpdateStatusAsync(
            int id,
            UpdateOrderStatusDto dto)
        {
            var order = await _db.Orders
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return null;
            }

            order.OrderStatus = dto.OrderStatus;
            order.UpdatedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return await GetByIdAsync(order.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _db.Orders
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return false;
            }

            _db.Orders.Remove(order);

            await _db.SaveChangesAsync();

            return true;
        }
    }
}
