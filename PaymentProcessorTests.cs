using Moq;
using System.Threading.Tasks;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using Xunit;

namespace PaymentGateway.Api.Tests;

public class PaymentProcessorTests
{
    [Fact]
    public async Task PostAcquiringBankPaymentAsync_ReturnsExpectedResponse()
    {
        // Arrange
        var mockAcquiringBankApi = new Mock<IAcquiringBankApi>();
        var paymentRequest = new AcquringBankPaymentRequest
        {
            CardNumber = "1234567890123456",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            CVV = 123,
            Amount = 100.00m,
            Currency = "USD"
        };
        var expectedResponse = new AcquringBankPaymentResponse
        {
            IsSuccessful = true,
            TransactionId = "txn_12345",
            Message = "Payment processed successfully"
        };

        mockAcquiringBankApi
            .Setup(api => api.PostAcquiringBankPaymentAsync(paymentRequest))
            .ReturnsAsync(expectedResponse);

        var paymentProcessor = new PaymentProcessor(mockAcquiringBankApi.Object);

        // Act
        var actualResponse = await paymentProcessor.PostAquringBankPaymentAsync(paymentRequest);

        // Assert
        Assert.Equal(expectedResponse.IsSuccessful, actualResponse.IsSuccessful);
        Assert.Equal(expectedResponse.TransactionId, actualResponse.TransactionId);
        Assert.Equal(expectedResponse.Message, actualResponse.Message);
    }
}
