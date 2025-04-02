using FluentValidation;

using Moq;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests
{
    public class PaymentsProcessorTests
    {
        private readonly Mock<IAcquiringBankApi> _mockAcquiringBankApi;
        private readonly Mock<IValidator<MerchantPaymentRequest>> _mockMerchantPaymentRequestValidator;
        private readonly Mock<IPaymentsRepository> _mockPaymentsRepository;
        private readonly PaymentsProcessor _paymentsProcessor;

        public PaymentsProcessorTests()
        {
            _mockAcquiringBankApi = new Mock<IAcquiringBankApi>(MockBehavior.Strict);
            _mockPaymentsRepository = new Mock<IPaymentsRepository>(MockBehavior.Strict);
            _mockMerchantPaymentRequestValidator = new Mock<IValidator<MerchantPaymentRequest>>(MockBehavior.Strict);

            _paymentsProcessor = new PaymentsProcessor(
                _mockAcquiringBankApi.Object,
                _mockPaymentsRepository.Object, 
                _mockMerchantPaymentRequestValidator.Object
            );
        }

        private static MerchantPaymentRequest CreateValidMerchantPaymentRequest()
        {
            return new MerchantPaymentRequest
            {
                CardNumber = "1234567812345678",
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Currency = "GBP",
                Amount = 100,
                Cvv = "123"
            };
        }

        private static PostMerchantPaymentResponse CreateExpectedResponse(PaymentStatus status)
        {
            return new PostMerchantPaymentResponse
            {
                Id = Guid.NewGuid(),
                ExpiryYear = 2025,
                ExpiryMonth = 12,
                Amount = 100,
                CardNumberLastFour = 5678,
                Currency = "GBP",
                Status = status
            };
        }

        [Fact]
        public async Task ProcessPaymentAsync_WhenValidMerchantRequest_ReturnsAuthorizedStatus()
        {
            // Arrange
            var merchantPaymentRequest = CreateValidMerchantPaymentRequest();
            var acquiringBankPaymentResponse = new AcquiringBankPaymentResponse(true, "123456");
            var expectedResponse = CreateExpectedResponse(PaymentStatus.Authorized);

            SetupMockedMethods(acquiringBankPaymentResponse);

            // Act
            var actualResponse = await _paymentsProcessor.ProcessPaymentAsync(merchantPaymentRequest);

            // Assert
            Assert.Equal(expectedResponse.Status, actualResponse.Status);
            Assert.Equal(expectedResponse.CardNumberLastFour, actualResponse.CardNumberLastFour);
            Assert.Equal(expectedResponse.Currency, actualResponse.Currency);
            Assert.Equal(expectedResponse.Amount, actualResponse.Amount);
            Assert.Equal(expectedResponse.ExpiryMonth, actualResponse.ExpiryMonth);
            Assert.Equal(expectedResponse.ExpiryYear, actualResponse.ExpiryYear);

            _mockMerchantPaymentRequestValidator.Verify(v => v.Validate(It.IsAny<MerchantPaymentRequest>()), Times.Once);
            _mockAcquiringBankApi.Verify(api => api.PostAcquiringBankPaymentAsync(It.IsAny<HttpContent>()), Times.Once);
            _mockPaymentsRepository.Verify(repo => repo.Add(It.IsAny<PostMerchantPaymentResponse>()), Times.Once);
        }

        [Fact]
        public async Task ProcessPaymentAsync_WhenMerchantRequestNotValid_ReturnsDeclinedStatus()
        {
            // Arrange
            var merchantPaymentRequest = CreateValidMerchantPaymentRequest();
            var acquiringBankPaymentResponse = new AcquiringBankPaymentResponse(false, "");
            var expectedResponse = CreateExpectedResponse(PaymentStatus.Declined);

            SetupMockedMethods(acquiringBankPaymentResponse);

            // Act
            var actualResponse = await _paymentsProcessor.ProcessPaymentAsync(merchantPaymentRequest);

            // Assert
            Assert.Equal(expectedResponse.Status, actualResponse.Status);
            Assert.Equal(expectedResponse.CardNumberLastFour, actualResponse.CardNumberLastFour);
            Assert.Equal(expectedResponse.Currency, actualResponse.Currency);
            Assert.Equal(expectedResponse.Amount, actualResponse.Amount);
            Assert.Equal(expectedResponse.ExpiryMonth, actualResponse.ExpiryMonth);
            Assert.Equal(expectedResponse.ExpiryYear, actualResponse.ExpiryYear);

            _mockMerchantPaymentRequestValidator.Verify(v => v.Validate(It.IsAny<MerchantPaymentRequest>()), Times.Once);
            _mockAcquiringBankApi.Verify(api => api.PostAcquiringBankPaymentAsync(It.IsAny<HttpContent>()), Times.Once);
            _mockPaymentsRepository.Verify(repo => repo.Add(It.IsAny<PostMerchantPaymentResponse>()), Times.Once);
        }

        private void SetupMockedMethods(AcquiringBankPaymentResponse acquiringBankPaymentResponse)
        {
            _mockMerchantPaymentRequestValidator
                            .Setup(v => v.Validate(It.IsAny<MerchantPaymentRequest>()))
                            .Returns(new FluentValidation.Results.ValidationResult());

            _mockAcquiringBankApi
                .Setup(api => api.PostAcquiringBankPaymentAsync(It.IsAny<HttpContent>()))
                .ReturnsAsync(acquiringBankPaymentResponse);

            _mockPaymentsRepository
                .Setup(repo => repo.Add(It.IsAny<PostMerchantPaymentResponse>()));
        }
    }
}
