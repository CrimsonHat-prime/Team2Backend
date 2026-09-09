using BlitzMall_Backend.Data;
using BlitzMall_Backend.DTOs.Payment;
using BlitzMall_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace BlitzMall_Backend.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _db;

        public PaymentService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<PaymentDto>> GetAllAsync()
        {
            return await _db.Payments
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    OrderId = p.OrderId,
                    Amount = p.Amount,
                    Status = p.Status,
                    Method = p.Method,
                    TransactionId = p.TransactionId,
                    CreatedAt = p.CreatedAt,
                    CompletedAt = p.CompletedAt
                })
                .ToListAsync();
        }

        public async Task<PaymentDto?> GetByIdAsync(int id)
        {
            return await _db.Payments
                .Where(p => p.Id == id)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    OrderId = p.OrderId,
                    Amount = p.Amount,
                    Status = p.Status,
                    Method = p.Method,
                    TransactionId = p.TransactionId,
                    CreatedAt = p.CreatedAt,
                    CompletedAt = p.CompletedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PaymentDto?> CreateAsync(CreatePaymentDto dto)
        {
            var order = await _db.Orders
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId);

            if (order == null)
            {
                return null;
            }

            var payment = new Payment
            {
                OrderId = dto.OrderId,
                Amount = dto.Amount,
                Method = dto.Method,
                Status = "Pending",
                TransactionId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(payment.Id);
        }

        public async Task<PaymentDto?> UpdateAsync(
            int id,
            UpdatePaymentDto dto)
        {
            var payment = await _db.Payments
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                return null;
            }

            payment.Status = dto.Status;
            payment.CompletedAt = dto.CompletedAt;

            await _db.SaveChangesAsync();

            return await GetByIdAsync(payment.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var payment = await _db.Payments
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                return false;
            }

            _db.Payments.Remove(payment);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}