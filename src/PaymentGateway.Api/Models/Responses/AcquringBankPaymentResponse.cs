namespace PaymentGateway.Api.Models.Responses;

public record AcquringBankPaymentResponse(
    bool Authorized,
    string AuthorizationCode
);