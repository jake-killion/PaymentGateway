namespace PaymentGateway.Api.Models.Responses;

public class PostPaymentResponse
{
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public int CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }

    // Easily converts PostPaymentResponse to PostPaymentRequest
    public GetPaymentResponse ToGetPaymentResponse()
    {
        return new GetPaymentResponse
        {
            Id = Id,
            Status = Status,
            CardNumberLastFour = CardNumberLastFour,
            ExpiryMonth = ExpiryMonth,
            ExpiryYear = ExpiryYear,
            Currency = Currency,
            Amount = Amount
        };
    }
}
