using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services.Interfaces
{
    public interface IAcquiringBankService
    {
        Task<AcquiringBankResponse> Authorize(AcquiringBankRequest request);
    }
}
