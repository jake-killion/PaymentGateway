namespace PaymentGateway.Api.Models.Responses
{
    public class ProcessPaymentResponse
    {
        public PaymentStatus PaymentStatus { get; set; }
        public List<string> ErrorMessages { get; set; }
        public PostPaymentResponse Result { get; set; }
    }
}
