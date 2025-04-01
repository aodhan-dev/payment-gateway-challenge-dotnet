namespace PaymentGateway.Api.Models.Requests;

public record AcquiringBankPaymentRequest(
    string CardNumber, 
    string ExpiryDate,
    string Currency, 
    int Amount, 
    int Cvv
);