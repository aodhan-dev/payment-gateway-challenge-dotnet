using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

using Refit;

namespace PaymentGateway.Api.Services;

public interface IAcquiringBankApi
{
    [Post("payments/{paymentRequest}")]
    Task<AcquringBankPaymentResponse> PostAcquiringBankPaymentAsync(AcquringBankPaymentRequest paymentRequest);
}
