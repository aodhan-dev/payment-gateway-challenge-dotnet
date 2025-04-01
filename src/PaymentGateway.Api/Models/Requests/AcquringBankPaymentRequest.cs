namespace PaymentGateway.Api.Models.Requests;

public record AcquringBankPaymentRequest(
    string CardNumber, 
    string ExpiryDate,
    string Currency, 
    int Amount, 
    int Cvv
);