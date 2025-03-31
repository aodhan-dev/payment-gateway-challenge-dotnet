using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentProcessingService(IAcquiringBankApi acquiringBankApi) : IPaymentProcessingService
{
    private readonly IAcquiringBankApi _acquiringBankApi = acquiringBankApi;

    public async Task<AcquringBankPaymentResponse> PostAquringBankPaymentAsync(AcquringBankPaymentRequest paymentRequest)
    {
        return await _acquiringBankApi.PostAcquiringBankPaymentAsync(paymentRequest);
    }
}
