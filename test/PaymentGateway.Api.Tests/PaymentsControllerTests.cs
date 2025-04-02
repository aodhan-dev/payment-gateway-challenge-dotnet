using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

using Xunit;

namespace PaymentGateway.Api.Tests
{
    public class PaymentsControllerTests
    {
        private readonly Random _random = new();

        [Fact]
        public async Task GetPaymentAsync_RetrievesAPaymentSuccessfully()
        {
            // Arrange
            var payment = CreateValidMerchantPaymentResponse();

            var paymentsRepository = new PaymentsRepository();
            paymentsRepository.Add(payment);

            var client = SetupWebApplicationFactory(paymentsRepository);

            // Act
            var response = await client.GetAsync($"/api/Payments/{payment.Id}");
            var paymentResponse = await response.Content.ReadFromJsonAsync<PostMerchantPaymentResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(paymentResponse);
        }

        [Fact]
        public async Task GetPaymentAsync_Returns404IfPaymentNotFound()
        {
            // Arrange
            var paymentsRepository = new PaymentsRepository();
            var client = SetupWebApplicationFactory(paymentsRepository);

            // Act
            var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task WhenRequestIsValid_PostPaymentAsync_ReturnsPaymentStatus_Authorized()
        {
            // Arrange
            var paymentRequest = CreateValidMerchantPaymentRequest();
            var paymentResponse = CreateValidMerchantPaymentResponse();
            paymentResponse.Status = PaymentStatus.Authorized;

            var mockPaymentsProcessor = new Mock<IPaymentsProcessor>();
            mockPaymentsProcessor
                .Setup(p => p.ProcessPaymentAsync(It.IsAny<MerchantPaymentRequest>()))
                .ReturnsAsync(paymentResponse);

            var client = SetupWebApplicationFactoryWithProcessor(mockPaymentsProcessor.Object);

            // Act
            var response = await client.PostAsJsonAsync("/api/Payments", paymentRequest);
            var actualResponse = await response.Content.ReadFromJsonAsync<PostMerchantPaymentResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(PaymentStatus.Authorized, actualResponse?.Status);
        }

        [Fact]
        public async Task WhenRequestIsDeclinedByAcquiringBank_PostPaymentAsync_ReturnsPaymentStatus_Declined()
        {
            // Arrange
            var paymentRequest = CreateValidMerchantPaymentRequest();
            paymentRequest.CardNumber = "12345678912456789"; // Card number ending in an odd number will be declined

            var paymentResponse = CreateValidMerchantPaymentResponse();
            paymentResponse.Status = PaymentStatus.Declined;

            var mockPaymentsProcessor = new Mock<IPaymentsProcessor>();
            mockPaymentsProcessor
                .Setup(p => p.ProcessPaymentAsync(It.IsAny<MerchantPaymentRequest>()))
                .ReturnsAsync(paymentResponse);

            var client = SetupWebApplicationFactoryWithProcessor(mockPaymentsProcessor.Object);

            // Act
            var response = await client.PostAsJsonAsync("/api/Payments", paymentRequest);
            var actualResponse = await response.Content.ReadFromJsonAsync<PostMerchantPaymentResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(PaymentStatus.Declined, actualResponse?.Status);
        }

        [Fact]
        public async Task WhenRequestIsNotValid_PostPaymentAsync_ReturnsPaymentStatus_Rejected()
        {
            // Arrange
            var paymentRequest = CreateValidMerchantPaymentRequest();
            paymentRequest.CardNumber = "";

            var paymentResponse = CreateValidMerchantPaymentResponse();
            paymentResponse.Status = PaymentStatus.Rejected;

            var mockPaymentsProcessor = new Mock<IPaymentsProcessor>();
            mockPaymentsProcessor
                .Setup(p => p.ProcessPaymentAsync(It.IsAny<MerchantPaymentRequest>()))
                .ReturnsAsync(paymentResponse);

            var client = SetupWebApplicationFactoryWithProcessor(mockPaymentsProcessor.Object);

            // Act
            var response = await client.PostAsJsonAsync("/api/Payments", paymentRequest);
            var actualResponse = await response.Content.ReadFromJsonAsync<PostMerchantPaymentResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(PaymentStatus.Rejected, actualResponse?.Status);
        }

        private MerchantPaymentRequest CreateValidMerchantPaymentRequest()
        {
            return new MerchantPaymentRequest
            {
                CardNumber = "1234567812345678",
                ExpiryMonth = _random.Next(1, 12),
                ExpiryYear = _random.Next(2023, 2030),
                Amount = _random.Next(1, 10000),
                Currency = "GBP",
                Cvv = "123"
            };
        }

        private PostMerchantPaymentResponse CreateValidMerchantPaymentResponse()
        {
            return new PostMerchantPaymentResponse
            {
                Id = Guid.NewGuid(),
                ExpiryYear = _random.Next(2027, 2030),
                ExpiryMonth = _random.Next(1, 12),
                Amount = _random.Next(1, 10000),
                CardNumberLastFour = _random.Next(1111, 9999),
                Currency = "GBP",
                Status = PaymentStatus.Authorized
            };
        }

        private static HttpClient SetupWebApplicationFactory(IPaymentsRepository paymentsRepository)
        {
            var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
            var client = webApplicationFactory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services => ((ServiceCollection)services)
                    .AddSingleton(paymentsRepository)))
                .CreateClient();
            return client;
        }

        private static HttpClient SetupWebApplicationFactoryWithProcessor(IPaymentsProcessor paymentsProcessor)
        {
            var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
            var client = webApplicationFactory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services => ((ServiceCollection)services)
                    .AddSingleton(paymentsProcessor)))
                .CreateClient();
            return client;
        }
    }
}


