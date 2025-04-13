using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Requests;

// Changed the record positional parameters to properties to allow the use of JsonPropertyName attribute
public record AcquiringBankPaymentRequest
{
    [JsonPropertyName("card_number")]
    public string CardNumber { get; init; } = default!;

    [JsonPropertyName("expiry_date")]
    public string ExpiryDate { get; init; } = default!;

    [JsonPropertyName("currency")]
    public string Currency { get; init; } = default!;

    [JsonPropertyName("amount")]
    public int Amount { get; init; }

    [JsonPropertyName("cvv")]
    public int Cvv { get; init; }
}