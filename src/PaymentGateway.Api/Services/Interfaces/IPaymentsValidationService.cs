using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services.Interfaces
{
    public interface IPaymentsValidationService
    {
        List<string> Validate(PostPaymentRequestMessage request);
    }
}
