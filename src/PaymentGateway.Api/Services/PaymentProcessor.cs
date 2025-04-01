using FluentValidation;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentProcessor(IAcquiringBankApi acquiringBankApi) : IPaymentProcessor
{
    private readonly IAcquiringBankApi _acquiringBankApi = acquiringBankApi;
    private readonly IValidator<PostMerchantPaymentRequest> _paymentRequestValidator;

    public async Task<PostMerchantPaymentResponse> ProcessPaymentAsync(PostMerchantPaymentRequest paymentRequest)
    {
        if (!_paymentRequestValidator.Validate(paymentRequest).IsValid)
        {
            throw new ValidationException("Invalid payment request");
        }
        var acquiringBankPaymentRequest = new AcquringBankPaymentRequest(
            CardNumber: paymentRequest.CardNumber,
            ExpiryDate: $"{paymentRequest.ExpiryMonth}/{paymentRequest.ExpiryYear}",
            Amount: paymentRequest.Amount,
            Currency: paymentRequest.Currency,
            Cvv: int.Parse(paymentRequest.Cvv)
        );
        var acquiringBankPaymentResponse = await PostAquringBankPaymentAsync(acquiringBankPaymentRequest);

        return new PostMerchantPaymentResponse
        {
            Id = Guid.NewGuid(),
            ExpiryYear = paymentRequest.ExpiryYear,
            ExpiryMonth = paymentRequest.ExpiryMonth,
            Amount = paymentRequest.Amount,
            CardNumberLastFour = int.Parse(paymentRequest.CardNumber[^4..]),
            Currency = paymentRequest.Currency,
            Status = acquiringBankPaymentResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined
        };
    }
    public async Task<AcquringBankPaymentResponse> PostAquringBankPaymentAsync(AcquringBankPaymentRequest paymentRequest)
    {
        return await _acquiringBankApi.PostAcquiringBankPaymentAsync(paymentRequest);
    }
}
