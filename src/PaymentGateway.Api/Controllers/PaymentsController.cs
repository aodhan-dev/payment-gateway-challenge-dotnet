using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentsRepository paymentsRepository, IPaymentsProcessor paymentProcessor) : ControllerBase
{
    private readonly IPaymentsRepository _paymentsRepository = paymentsRepository;
    private readonly IPaymentsProcessor _paymentProcessor = paymentProcessor;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MerchantPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = await Task.Run(() => _paymentsRepository.Get(id));

        return payment is not null ?
            (ActionResult<MerchantPaymentResponse?>)new OkObjectResult(payment) :
            (ActionResult<MerchantPaymentResponse?>)new NotFoundObjectResult(payment);
    }

    [HttpPost]
    public async Task<ActionResult<MerchantPaymentResponse>> PostPaymentAsync([FromBody] MerchantPaymentRequest paymentRequest)
    {
        var response = await _paymentProcessor.ProcessPaymentAsync(paymentRequest);

        return response.Status is PaymentStatus.Rejected
            ? (ActionResult<MerchantPaymentResponse>)new BadRequestObjectResult(response)
            : (ActionResult<MerchantPaymentResponse>)new OkObjectResult(response);
    }
}

