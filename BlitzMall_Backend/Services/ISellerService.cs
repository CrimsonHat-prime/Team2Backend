using BlitzMall_Backend.DTOs.Seller;

namespace BlitzMall_Backend.Services
{
    public interface ISellerService
    {
        Task<List<SellerDto>> GetAllAsync();
        Task<SellerDto?> GetByIdAsync(int id);
        Task<SellerDto> CreateAsync(CreateSellerDto dto);
        Task<SellerDto?> UpdateAsync(int id, UpdateSellerDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
