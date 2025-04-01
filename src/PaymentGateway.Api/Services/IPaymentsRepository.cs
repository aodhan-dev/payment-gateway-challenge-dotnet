
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public interface IPaymentsRepository
{
    void Add(PostMerchantPaymentResponse payment);
    PostMerchantPaymentResponse? Get(Guid id);
}