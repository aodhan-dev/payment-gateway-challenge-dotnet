namespace PaymentGateway.Api.Models.Responses;

public record AcquiringBankPaymentResponse(
    bool Authorized,
    string AuthorizationCode
);