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
    public async Task<ActionResult<PostMerchantPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = await Task.Run(() => _paymentsRepository.Get(id));

        return payment is not null ?
            (ActionResult<PostMerchantPaymentResponse?>)new OkObjectResult(payment) :
            (ActionResult<PostMerchantPaymentResponse?>)new NotFoundObjectResult(payment);
    }

    [HttpPost]
    public async Task<ActionResult<PostMerchantPaymentResponse>> PostPaymentAsync([FromBody] MerchantPaymentRequest paymentRequest)
    {
        var response = await _paymentProcessor.ProcessPaymentAsync(paymentRequest);

        return response.Status is PaymentStatus.Rejected
            ? (ActionResult<PostMerchantPaymentResponse>)new BadRequestObjectResult(response)
            : (ActionResult<PostMerchantPaymentResponse>)new OkObjectResult(response);
    }
}

