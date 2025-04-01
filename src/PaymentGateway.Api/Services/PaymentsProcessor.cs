using System.Text.Json;

using FluentValidation;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsProcessor(
    IAcquiringBankApi acquiringBankApi,
    IPaymentsRepository paymentsRepository,
    IValidator<MerchantPaymentRequest> paymentRequestValidator)
    : IPaymentsProcessor
{
    private readonly IAcquiringBankApi _acquiringBankApi = acquiringBankApi;
    private readonly IPaymentsRepository _paymentsRepository = paymentsRepository;
    private readonly IValidator<MerchantPaymentRequest> _paymentRequestValidator = paymentRequestValidator;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public async Task<PostMerchantPaymentResponse> ProcessPaymentAsync(MerchantPaymentRequest paymentRequest)
    {
        if (!_paymentRequestValidator.Validate(paymentRequest).IsValid)
        {
            return new PostMerchantPaymentResponse
            {
                Id = Guid.Empty,
                Status = PaymentStatus.Rejected
            };
        }

        var acquiringBankPaymentRequest = new AcquiringBankPaymentRequest(
            CardNumber: paymentRequest.CardNumber,
            ExpiryDate: $"{paymentRequest.ExpiryMonth}/{paymentRequest.ExpiryYear}",
            Amount: paymentRequest.Amount,
            Currency: paymentRequest.Currency,
            Cvv: int.Parse(paymentRequest.Cvv)
        );


        var acquiringBankPaymentResponse = await PostAquiringBankPaymentAsync(acquiringBankPaymentRequest);

        var postMerchantPaymentResponse = new PostMerchantPaymentResponse
        {
            Id = Guid.NewGuid(),
            ExpiryYear = paymentRequest.ExpiryYear,
            ExpiryMonth = paymentRequest.ExpiryMonth,
            Amount = paymentRequest.Amount,
            CardNumberLastFour = int.Parse(paymentRequest.CardNumber[^4..]),
            Currency = paymentRequest.Currency,
            Status = acquiringBankPaymentResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined
        };

        // Save successful payment to PaymentsRepository
        _paymentsRepository.Add(postMerchantPaymentResponse);
        return postMerchantPaymentResponse;
    }
    public async Task<AcquiringBankPaymentResponse> PostAquiringBankPaymentAsync(AcquiringBankPaymentRequest paymentRequest)
    {
        // serialize the payment request and send it to the acquiring bank
        var jsonRequest = JsonSerializer.Serialize(paymentRequest, _jsonSerializerOptions);
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await _acquiringBankApi.PostAcquiringBankPaymentAsync(content);
        return response;
    }
}
