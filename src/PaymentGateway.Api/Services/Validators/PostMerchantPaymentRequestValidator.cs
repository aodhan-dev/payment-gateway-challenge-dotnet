using FluentValidation;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services.Validators;

public class MerchantPaymentRequestValidator : AbstractValidator<MerchantPaymentRequest>
{
    public MerchantPaymentRequestValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .WithMessage("Card number must be provided.")
            .Length(14, 19)
            .WithMessage("Card number must be between 14 and 19 characters.")
            .Matches("^[0-9]+$")
            .WithMessage("Card number must only contain numeric characters.");

        RuleFor(x => x.ExpiryMonth)
            .InclusiveBetween(1, 12)
            .WithMessage("Expiry month must be between 1 and 12.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency must be provided.")
            .Must(BeInCurrencyEnum)
            .WithMessage("Currency must be EUR, GBP or USD.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Cvv)
            .NotEmpty()
            .WithMessage("CVV must be provided.")
            .Length(3, 4)
            .WithMessage("CVV must be between 3 and 4 characters.")
            .Matches("^[0-9]+$")
            .WithMessage("CVV must only contain numeric characters.");

        RuleFor(x => new { x.ExpiryMonth, x.ExpiryYear })
            .Must(x => IsExpiryDateInFuture(x.ExpiryMonth, x.ExpiryYear))
            .WithMessage("Expiry date must be in the future.");
    }

    private static bool IsExpiryDateInFuture(int expiryMonth, int expiryYear)
    {
        try
        {
            var expiryDate = new DateTime(expiryYear, expiryMonth, 1);
            return expiryDate > DateTime.Now;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }

    private bool BeInCurrencyEnum(string currency)
    {
        return !string.IsNullOrWhiteSpace(currency) && 
            Enum.TryParse(typeof(Currency), currency, true, out _);
    }
}