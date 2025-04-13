using FluentValidation;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentProcessor(
    IAcquiringBankApi acquiringBankPaymentApi,
    IPaymentsRepository paymentsRepository,
    IValidator<MerchantPaymentRequest> paymentRequestValidator)
    : IPaymentsProcessor
{
    private readonly IAcquiringBankApi _acquiringBankPaymentApi = acquiringBankPaymentApi;
    private readonly IPaymentsRepository _paymentsRepository = paymentsRepository;
    private readonly IValidator<MerchantPaymentRequest> _paymentRequestValidator = paymentRequestValidator;

    public async Task<MerchantPaymentResponse> ProcessPaymentAsync(MerchantPaymentRequest paymentRequest)
    {
        // Validate the request
        var validationResult = _paymentRequestValidator.Validate(paymentRequest);
        if (!validationResult.IsValid)
        {
            return new MerchantPaymentResponse
            {
                Id = Guid.Empty,
                Status = PaymentStatus.Rejected,
                ValidationErrors = [.. validationResult.Errors.Select(e => e.ErrorMessage)]
            };
        }

        try
        {
            // Create the acquiring bank payment request
            var acquiringBankPaymentRequest = CreateAcquiringBankPaymentRequest(paymentRequest);

            // Call the acquiring bank API
            var acquiringBankPaymentResponse = await _acquiringBankPaymentApi.PostAcquiringBankPaymentAsync(acquiringBankPaymentRequest);

            // Create the merchant payment response
            var merchantPaymentResponse = CreateMerchantPaymentResponse(paymentRequest, acquiringBankPaymentResponse);

            // Save the payment to the repository
            _paymentsRepository.Add(merchantPaymentResponse);

            return merchantPaymentResponse;
        }
        catch (Exception ex)
        {
            return new MerchantPaymentResponse
            {
                Id = Guid.Empty,
                Status = PaymentStatus.Rejected,
                ValidationErrors = [$"An error occurred while processing the payment with the Acquiring Bank: {ex.Message}."]
            };
        }
    }

    private static AcquiringBankPaymentRequest CreateAcquiringBankPaymentRequest(MerchantPaymentRequest paymentRequest)
    {
        return new AcquiringBankPaymentRequest
        {
            CardNumber = paymentRequest.CardNumber,
            ExpiryDate = $"{paymentRequest.ExpiryMonth:D2}/{paymentRequest.ExpiryYear}",
            Amount = paymentRequest.Amount,
            Currency = paymentRequest.Currency,
            Cvv = int.Parse(paymentRequest.Cvv)
        };
    }

    private static MerchantPaymentResponse CreateMerchantPaymentResponse(
        MerchantPaymentRequest paymentRequest,
        AcquiringBankPaymentResponse acquiringBankPaymentResponse)
    {
        return new MerchantPaymentResponse
        {
            Id = Guid.NewGuid(),
            ExpiryYear = paymentRequest.ExpiryYear,
            ExpiryMonth = paymentRequest.ExpiryMonth,
            Amount = paymentRequest.Amount,
            CardNumberLastFour = int.Parse(paymentRequest.CardNumber[^4..]),
            Currency = paymentRequest.Currency,
            Status = acquiringBankPaymentResponse.IsAuthorized ? PaymentStatus.Authorized : PaymentStatus.Declined
        };
    }
}