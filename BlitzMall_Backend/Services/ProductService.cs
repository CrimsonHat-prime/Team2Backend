using BlitzMall_Backend.Data;
using BlitzMall_Backend.DTOs.Auth;
using BlitzMall_Backend.DTOs.Product;
using BlitzMall_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;


namespace BlitzMall_Backend.Services
{
    public class ProductService:IProductService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _db;

        public ProductService(
       AppDbContext db,
       IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<ProductDto>> GetAllAsync()
        {

            return await _db.Products
             
                .Select(p => new ProductDto
                {
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    IsActive = p.IsActive,

                    BrandName = p.Brand.Name,
                    CategoryName = p.Category.Name,
                    SellerName = p.Seller.Name,

                    ImgUrls = p.ProdImgs
                .Select(i => i.UrlImage!)
                .ToList()
                })
                .ToListAsync();
        }
       public async Task<ProductDto?> GetByIdAsync(int id)
        {
            return await _db.Products
               .Where(p => p.Id == id)

                .Select(p => new ProductDto
                {
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    IsActive = p.IsActive,

                    BrandName = p.Brand.Name,
                    CategoryName = p.Category.Name,
                    SellerName = p.Seller.Name,

                    ImgUrls = p.ProdImgs
                .Select(i => i.UrlImage!)
                .ToList()
                })
                .FirstOrDefaultAsync();
        }
 
    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                BrandId = dto.BrandId,
                Quantity = dto.Quantity,
                CategoryId = dto.CategoryId
            };

            var userIdClaim = _httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException();
            }

            int userId = int.Parse(userIdClaim);

            var seller = await _db.Sellers
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (seller == null)
            {
                throw new InvalidOperationException("Seller not found.");
            }

            product.SellerId = seller.Id;
            product.CreatedDate = DateTime.UtcNow;

            _db.Products.Add(product);

            await _db.SaveChangesAsync();

            if (dto.ImgUrls != null)
            {
                foreach (var url in dto.ImgUrls)
                {
                    var image = new ProdImg
                    {
                        ProductId = product.Id,
                        UrlImage = url
                    };

                    _db.ProdImgs.Add(image);
                }
    
                await _db.SaveChangesAsync();
            }
            return await GetByIdAsync(product.Id)
?? throw new InvalidOperationException("Product was not found after creation.");

        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _db.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return null;
            }

            var userIdClaim = _httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException();
            }

            int userId = int.Parse(userIdClaim);

            var seller = await _db.Sellers
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (seller == null)
            {
                throw new InvalidOperationException("Seller not found.");
            }

            if (product.SellerId != seller.Id)
            {
                throw new UnauthorizedAccessException(
                    "You cannot update this product.");
            }

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Quantity = dto.Quantity;
            product.BrandId = dto.BrandId;
            product.CategoryId = dto.CategoryId;
            product.IsActive = dto.IsActive;
            product.UpdatedDate = DateTime.UtcNow;

            if (dto.ImgUrls != null)
            {
                var oldImages = await _db.ProdImgs
                    .Where(i => i.ProductId == product.Id)
                    .ToListAsync();

                _db.ProdImgs.RemoveRange(oldImages);

                foreach (var url in dto.ImgUrls)
                {
                    var image = new ProdImg
                    {
                        ProductId = product.Id,
                        UrlImage = url
                    };

                    _db.ProdImgs.Add(image);
                }
            }

            await _db.SaveChangesAsync();

            return await GetByIdAsync(id);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _db.Products
    .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return false;
            }
            var userIdClaim = _httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException();
            }

            int userId = int.Parse(userIdClaim);

            var seller = await _db.Sellers
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (seller == null)
            {
                throw new InvalidOperationException("Seller not found.");
            }

            if (product.SellerId != seller.Id)
            {
                throw new UnauthorizedAccessException(
                    "you cannot delete");
            }
            _db.Products.Remove(product);

            await _db.SaveChangesAsync();

            return true;
        }
   
    
    
    }
    }

