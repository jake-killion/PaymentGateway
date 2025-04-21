using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories.Interfaces;

namespace PaymentGateway.Api.Repositories;

public class PaymentsRepository : IPaymentsRepository
{  
    public async Task Add(PostPaymentResponse payment)
    {
        _payments.Add(payment);
    }

    public async Task<GetPaymentResponse> Get(Guid id)
    {
        // Did it this way only so i could reuse the same sample data
        PostPaymentResponse payment = _payments.FirstOrDefault(p => p.Id == id);
        return payment?.ToGetPaymentResponse();
    }

    // Sample data for testing purposes
    private List<PostPaymentResponse> _payments =
    [
        new PostPaymentResponse
        {
            Id = new Guid("e35c5102-3d9e-46d5-85a5-c793b9752995"),
            Amount = 1000,
            CardNumberLastFour = 1234,
            Currency = "USD",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Status = PaymentStatus.Authorized
        },
        new PostPaymentResponse
        {
            Id = new Guid("87236043-cea9-496d-97fd-b148f4775175"),
            Amount = 2000,
            CardNumberLastFour = 5678,
            Currency = "NZD",
            ExpiryMonth = 11,
            ExpiryYear = 2024,
            Status = PaymentStatus.Rejected
        },
        new PostPaymentResponse
        {
            Id = new Guid("96addc21-7cbe-4c81-9642-ae363ee4bfdb"),
            Amount = 1500,
            CardNumberLastFour = 9012,
            Currency = "GBP",
            ExpiryMonth = 10,
            ExpiryYear = 2023,
            Status = PaymentStatus.Declined
        }
    ];
}