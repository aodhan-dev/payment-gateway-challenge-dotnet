namespace PaymentGateway.Api.Configuration;

public class AcquiringBankApiOptions : IAcquiringBankApiOptions
{
    public string BaseUri { get; set; } = default!;
}