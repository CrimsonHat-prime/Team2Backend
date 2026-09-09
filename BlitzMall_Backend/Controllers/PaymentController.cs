using BlitzMall_Backend.DTOs.Payment;
using BlitzMall_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlitzMall_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<PaymentDto>>> GetAll()
        {
            try
            {
                return Ok(await _paymentService.GetAllAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetById(int id)
        {
            try
            {
                var payment = await _paymentService.GetByIdAsync(id);

                return payment == null
                    ? NotFound()
                    : Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<PaymentDto>> Create(
            [FromBody] CreatePaymentDto dto)
        {
            try
            {
                var payment = await _paymentService.CreateAsync(dto);

                return payment == null
                    ? BadRequest(new { message = "Order not found." })
                    : CreatedAtAction(
                        nameof(GetById),
                        new { id = payment.Id },
                        payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<PaymentDto>> Update(
            int id,
            [FromBody] UpdatePaymentDto dto)
        {
            try
            {
                var payment = await _paymentService.UpdateAsync(id, dto);

                return payment == null
                    ? NotFound()
                    : Ok(payment);
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
                return await _paymentService.DeleteAsync(id)
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