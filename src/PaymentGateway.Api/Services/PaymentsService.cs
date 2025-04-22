using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories.Interfaces;
using PaymentGateway.Api.Services.Interfaces;

namespace PaymentGateway.Api.Services
{
    public class PaymentsService : IPaymentsService
    {
        private readonly IPaymentsRepository _paymentsRepository;
        private readonly IPaymentsValidationService _paymentsValidationService;
        private readonly IAcquiringBankService _acquiringBankService;
        public PaymentsService(IPaymentsRepository paymentsRepository, IPaymentsValidationService paymentsValidationService, IAcquiringBankService acquiringBankService)
        {
            _acquiringBankService = acquiringBankService;
            _paymentsRepository = paymentsRepository;
            _paymentsValidationService = paymentsValidationService;
        }

        public async Task<ProcessPaymentResponse> Process(PostPaymentRequestMessage request)
        {
            // Validates the request (e.g. card number, expiry date, etc.)
            // Would like to seperate this into its own method but this is a simple example
            List<string> validationErrors = _paymentsValidationService.Validate(request);
            PostPaymentResponse responsePayment = request.ToResponse();
            if (validationErrors.Count > 0)
            {
                responsePayment.Status = PaymentStatus.Rejected;
                await _paymentsRepository.Add(responsePayment);
                return new ProcessPaymentResponse()
                {
                    ErrorMessages = validationErrors,
                    PaymentStatus = PaymentStatus.Rejected,
                    Result = responsePayment
                };
            }

            // If the request is valid, we send it to the acquiring bank
            // Also would like to separate this into its own method but this is a simple example
            AcquiringBankRequest acquiringBankRequest = request.ToAcquiringBankRequest();
            AcquiringBankResponse acquiringBankResponse = await _acquiringBankService.Authorize(acquiringBankRequest);
            if (acquiringBankResponse.Authorized == false)
            {
                responsePayment.Status = PaymentStatus.Declined;
                return new ProcessPaymentResponse()
                {
                    ErrorMessages = ["Payment could not be processed."],
                    PaymentStatus = PaymentStatus.Declined,
                    Result = responsePayment
                };
            }

            await _paymentsRepository.Add(responsePayment);
            return new ProcessPaymentResponse()
            {
                ErrorMessages = new List<string>(),
                PaymentStatus = PaymentStatus.Authorized,
                Result = responsePayment
            };
        }

        public async Task<GetPaymentResponse> Get(Guid id)
        {
            return await _paymentsRepository.Get(id);
        }
    }
}
