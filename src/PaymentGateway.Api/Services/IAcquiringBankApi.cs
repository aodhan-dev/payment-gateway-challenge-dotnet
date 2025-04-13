using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

using Refit;

namespace PaymentGateway.Api.Services;

public interface IAcquiringBankApi
{
    [Post("/payments")]
    Task<AcquiringBankPaymentResponse> PostAcquiringBankPaymentAsync([FromBody] AcquiringBankPaymentRequest paymentRequest);
}
