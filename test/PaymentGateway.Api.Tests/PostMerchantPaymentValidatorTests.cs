using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services.Validators;

namespace PaymentGateway.Api.Tests
{
    public class MerchantPaymentRequestValidatorTests
    {
        private readonly MerchantPaymentRequestValidator _validator;

        public MerchantPaymentRequestValidatorTests()
        {
            _validator = new MerchantPaymentRequestValidator();
        }

        private static MerchantPaymentRequest GetValidPaymentRequest()
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

        [Fact]
        public void Should_Not_Have_Error_When_Payment_Is_Valid()
        {
            var payment = GetValidPaymentRequest();
            var result = _validator.Validate(payment);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Have_Error_When_CardNumber_Is_Empty()
        {
            var payment = GetValidPaymentRequest();
            payment.CardNumber = string.Empty;

            var result = _validator.Validate(payment);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.PropertyName == "CardNumber" &&
                e.ErrorMessage == "Card number must be provided.");
        }

        [Fact]
        public void Should_Have_Error_When_CardNumber_Is_Not_Between_14_And_19_Characters()
        {
            var payment = GetValidPaymentRequest();
            payment.CardNumber = "12345678910";

            var result = _validator.Validate(payment);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.PropertyName == "CardNumber" &&
                e.ErrorMessage == "Card number must be between 14 and 19 characters.");
        }

        [Fact]
        public void Should_Have_Error_When_CardNumber_Is_Not_Numeric()
        {
            var payment = GetValidPaymentRequest();
            payment.CardNumber = "abcdefghijklmnop";

            var result = _validator.Validate(payment);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.PropertyName == "CardNumber" &&
                e.ErrorMessage == "Card number must only contain numeric characters.");
        }

        [Fact]
        public void Should_Have_Error_When_Currency_Is_Empty()
        {
            var payment = GetValidPaymentRequest();
            payment.Currency = string.Empty;

            var result = _validator.Validate(payment);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.PropertyName == "Currency" &&
                e.ErrorMessage == "Currency must be provided.");
        }

        [Fact]
        public void Should_Have_Error_When_Currency_Is_Not_In_Enum()
        {
            var payment = GetValidPaymentRequest();
            payment.Currency = "AUD";

            var result = _validator.Validate(payment);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.PropertyName == "Currency" &&
                e.ErrorMessage == "Currency must be EUR, GBP or USD.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Currency_Is_EUR()
        {
            var payment = GetValidPaymentRequest();
            payment.Currency = "EUR";

            var result = _validator.Validate(payment);

            Assert.True(result.IsValid);
            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Currency");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Currency_Is_GBP()
        {
            var payment = GetValidPaymentRequest();
            payment.Currency = "GBP";

            var result = _validator.Validate(payment);

            Assert.True(result.IsValid);
            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Currency");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Currency_Is_USD()
        {
            var payment = GetValidPaymentRequest();
            payment.Currency = "USD";

            var result = _validator.Validate(payment);

            Assert.True(result.IsValid);
            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Currency");
        }

        [Fact]
        public void Should_Have_Error_When_Amount_Is_Zero()
        {
            var payment = GetValidPaymentRequest();
            payment.Amount = 0;

            var result = _validator.Validate(payment);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Amount");
        }

        [Fact]
        public void Should_Have_Error_When_Cvv_Is_Not_Provided()
        {
            var payment = GetValidPaymentRequest();
            payment.Cvv = "";

            var result = _validator.Validate(payment);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.PropertyName == "Cvv" &&
                e.ErrorMessage == "CVV must be provided.");
        }

        [Fact]
        public void Should_Have_Error_When_Cvv_Is_Not_Between_3_And_4_Characters()
        {
            var payment = GetValidPaymentRequest();
            payment.Cvv = "12";

            var result = _validator.Validate(payment);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.PropertyName == "Cvv" &&
                e.ErrorMessage == "CVV must be between 3 and 4 characters.");
        }

        [Fact]
        public void Should_Have_Error_When_Cvv_Is_Not_Numeric()
        {
            var payment = GetValidPaymentRequest();
            payment.Cvv = "ABC";

            var result = _validator.Validate(payment);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.PropertyName == "Cvv" &&
                e.ErrorMessage == "CVV must only contain numeric characters.");
        }

        [Fact]
        public void Should_Have_Error_When_Expiry_Date_Is_In_The_Past()
        {
            var payment = GetValidPaymentRequest();
            payment.ExpiryMonth = 1;
            payment.ExpiryYear = 2020;

            var result = _validator.Validate(payment);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.ErrorMessage == "Expiry date must be in the future.");
        }

        [Fact]
        public void Should_Have_Error_When_Expiry_Month_Is_Not_Between_1_and_12()
        {
            var payment = GetValidPaymentRequest();
            payment.ExpiryMonth = 13;

            var result = _validator.Validate(payment);
            var errors = result.Errors;

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.ErrorMessage == "Expiry month must be between 1 and 12.");
        }

        [Fact]
        public void Should_Have_Error_When_ExpiryDate_Is_In_The_Past()
        {
            var payment = GetValidPaymentRequest();
            payment.ExpiryMonth = 1;
            payment.ExpiryYear = 2020;

            var result = _validator.Validate(payment);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.ErrorMessage == "Expiry date must be in the future.");
        }
    }
}