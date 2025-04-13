
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public interface IPaymentsRepository
{
    void Add(MerchantPaymentResponse payment);
    MerchantPaymentResponse? Get(Guid id);
}