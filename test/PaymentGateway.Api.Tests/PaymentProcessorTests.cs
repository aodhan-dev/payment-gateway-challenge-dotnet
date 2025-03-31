using Moq;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests
{
    public class PaymentProcessorTests
    {
        [Fact]
        public async Task PostAcquiringBankPaymentAsync_ReturnsExpectedResponse()
        {
            // Arrange
            var mockAcquiringBankApi = new Mock<IAcquiringBankApi>(MockBehavior.Strict);
            var paymentRequest = new AcquringBankPaymentRequest(
                CardNumber: "1234567890123456",
                ExpiryDate: "12/2027",
                Amount: 1000,
                Currency: "USD",
                Cvv: 123
            );

            var expectedResponse = new AcquringBankPaymentResponse(
                Authorised: false,
                AuthorisationCode: Guid.NewGuid().ToString()
            );

            mockAcquiringBankApi
                .Setup(api => api.PostAcquiringBankPaymentAsync(paymentRequest))
                .ReturnsAsync(expectedResponse);

            var paymentProcessor = new PaymentProcessor(mockAcquiringBankApi.Object);

            // Act
            var actualResponse = await paymentProcessor.PostAquringBankPaymentAsync(paymentRequest);

            // Assert
            Assert.Equal(expectedResponse.Authorised, actualResponse.Authorised);
            Assert.Equal(expectedResponse.AuthorisationCode, actualResponse.AuthorisationCode);

            mockAcquiringBankApi.Verify(api => api.PostAcquiringBankPaymentAsync(paymentRequest), Times.Once);
        }
    }
}
