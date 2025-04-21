using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Models.Requests
{
    public class PostPaymentRequestMessage
    {
        public string CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string Cvv { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }

        public PostPaymentResponse ToResponse()
        {
            return new PostPaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = PaymentStatus.Authorized,
                CardNumberLastFour = int.Parse(CardNumber.Substring(CardNumber.Length - 4)),
                ExpiryMonth = ExpiryMonth,
                ExpiryYear = ExpiryYear,
                Currency = Currency,
                Amount = Amount,
            };
        }

        public AcquiringBankRequest ToAcquiringBankRequest()
        {
            return new AcquiringBankRequest
            {
                CardNumber = CardNumber,
                ExpiryDate = $"{ExpiryMonth:D2}/{ExpiryYear}",
                Cvv = Cvv,
                Amount = Amount,
                Currency = Currency
            };
        }
    }
}
