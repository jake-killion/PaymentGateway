using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Repositories.Interfaces
{
    public interface IPaymentsRepository
    {
        Task Add(PostPaymentResponse payment);
        Task<GetPaymentResponse> Get(Guid id);
    }
}
