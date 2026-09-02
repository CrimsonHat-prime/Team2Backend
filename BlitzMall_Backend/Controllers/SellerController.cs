using BlitzMall_Backend.DTOs.Seller;
using BlitzMall_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlitzMall_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SellerController : ControllerBase
    {
        private readonly ISellerService _sellerService;

        public SellerController(ISellerService sellerService)
        {
            _sellerService = sellerService;
        }

        // GET: api/Seller
        [HttpGet]
        public async Task<ActionResult<List<SellerDto>>> GetAll()
        {
            var sellers = await _sellerService.GetAllAsync();

            return Ok(sellers);
        }

        // GET: api/Seller/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SellerDto>> GetById(int id)
        {
            var seller = await _sellerService.GetByIdAsync(id);

            if (seller == null)
                return NotFound("Seller not found.");

            return Ok(seller);
        }

        // POST: api/Seller
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<SellerDto>> Create(
            [FromBody] CreateSellerDto dto)
        {
            try
            {
                var seller = await _sellerService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = seller.Id },
                    seller);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Seller/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<SellerDto>> Update(
            int id,
            [FromBody] UpdateSellerDto dto)
        {
            try
            {
                var seller = await _sellerService.UpdateAsync(id, dto);

                if (seller == null)
                    return NotFound("Seller not found.");

                return Ok(seller);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        // DELETE: api/Seller/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _sellerService.DeleteAsync(id);

                if (!deleted)
                    return NotFound("Seller not found.");

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}