using BlitzMall_Backend.DTOs.Order;
using BlitzMall_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlitzMall_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetAll()
        {
            try
            {
                return Ok(await _orderService.GetAllAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            try
            {
                return Ok(await _orderService.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create(
            [FromBody] CreateOrderDto dto)
        {
            try
            {
                var order = await _orderService.CreateAsync(dto);

                return order == null
                    ? BadRequest(new { message = "Unable to create order." })
                    : CreatedAtAction(
                        nameof(GetById),
                        new { id = order.Id },
                        order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<ActionResult<OrderDto>> UpdateStatus(
            int id,
            [FromBody] UpdateOrderStatusDto dto)
        {
            try
            {
                var order = await _orderService.UpdateStatusAsync(id, dto);

                return order == null
                    ? NotFound()
                    : Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                return await _orderService.DeleteAsync(id)
                    ? NoContent()
                    : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}