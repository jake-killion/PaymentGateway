using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services.Interfaces;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentsService _paymentService;

    public PaymentsController(IPaymentsService paymentsService)
    {
        _paymentService = paymentsService;
    }

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse>> PostPaymentAsync([FromBody] PostPaymentRequestMessage request) 
    {
        try
        {
            ProcessPaymentResponse response = await _paymentService.Process(request);
            if (response.ErrorMessages.Count > 0)
            {
                return BadRequest(response);
            }
            return Ok(response.Result);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error occurred during payment processing.");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetPaymentResponse>> GetPaymentAsync(Guid id)
    {
        try
        {
            GetPaymentResponse payment = await _paymentService.Get(id);
            if (payment == null)
            {
                return NotFound($"Payment of Id '{id}' not found.");
            }
            return Ok(payment);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error occurred during payment retrieval.");
        }
    }
}