using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Responses;

using Refit;

namespace PaymentGateway.Api.Services;

public interface IAcquiringBankApi
{
    [Post("/payments")]
    Task<AcquiringBankPaymentResponse> PostAcquiringBankPaymentAsync([FromBody]HttpContent paymentRequest);
}
