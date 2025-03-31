
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services
{
    public interface IPaymentProcessingService
    {
        Task<AcquringBankPaymentResponse> PostAquringBankPaymentAsync(AcquringBankPaymentRequest paymentRequest);
    }
}