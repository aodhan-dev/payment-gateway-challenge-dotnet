namespace PaymentGateway.Api.Models.Responses;

public record AcquringBankPaymentResponse(
    bool Authorised,
    string AuthorisationCode
    );