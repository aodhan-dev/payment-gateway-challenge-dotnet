using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public interface IPaymentsProcessor
{
    Task<PostMerchantPaymentResponse> ProcessPaymentAsync(MerchantPaymentRequest paymentRequest);
}