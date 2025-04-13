using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository : IPaymentsRepository
{
    public List<MerchantPaymentResponse> Payments = [];

    public void Add(MerchantPaymentResponse payment)
    {
        Payments.Add(payment);
    }

    public MerchantPaymentResponse? Get(Guid id)
    {
        return Payments.FirstOrDefault(p => p.Id == id);
    }
}