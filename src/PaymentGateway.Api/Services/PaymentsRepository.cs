using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository : IPaymentsRepository
{
    public List<PostMerchantPaymentResponse> Payments = [];

    public void Add(PostMerchantPaymentResponse payment)
    {
        Payments.Add(payment);
    }

    public PostMerchantPaymentResponse? Get(Guid id)
    {
        return Payments.FirstOrDefault(p => p.Id == id);
    }
}