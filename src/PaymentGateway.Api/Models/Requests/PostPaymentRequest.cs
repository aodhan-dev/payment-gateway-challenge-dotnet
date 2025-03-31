namespace PaymentGateway.Api.Models.Requests;

public class PostMerchantPaymentRequest
{
    public string CardNumber { get; set; } = default!;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; } = default!;
    public int Amount { get; set; }
    public string Cvv { get; set; } = default!;
}