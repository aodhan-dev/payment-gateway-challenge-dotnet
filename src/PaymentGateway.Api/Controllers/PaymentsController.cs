using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentsRepository paymentsRepository) : Controller
{
    private readonly IPaymentsRepository _paymentsRepository = paymentsRepository;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostMerchantPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = await Task.Run(() => _paymentsRepository.Get(id));

        return payment is not null ? 
            (ActionResult<PostMerchantPaymentResponse?>)new OkObjectResult(payment) : 
            (ActionResult<PostMerchantPaymentResponse?>)new NotFoundObjectResult(payment);
    }
}