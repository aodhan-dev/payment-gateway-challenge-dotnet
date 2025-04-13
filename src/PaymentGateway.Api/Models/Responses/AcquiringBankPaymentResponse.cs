namespace PaymentGateway.Api.Models.Responses;

public record AcquiringBankPaymentResponse(
    bool IsAuthorized,
    string AuthorizationCode
);