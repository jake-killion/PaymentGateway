using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services.Interfaces
{
    public interface IPaymentsService
    {
        Task<ProcessPaymentResponse> Process(PostPaymentRequestMessage request);
        Task<GetPaymentResponse> Get(Guid id);
    }
}
