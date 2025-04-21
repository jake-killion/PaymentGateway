using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories;

namespace PaymentGateway.Tests.Repositories
{
    public class PaymentsRepositoryTests
    {
        private readonly PaymentsRepository _repository;

        public PaymentsRepositoryTests()
        {
            _repository = new PaymentsRepository();
        }

        [Fact]
        public async Task Get_ReturnsPayment_WhenIdExists()
        {
            // Arrange
            var existingId = Guid.Parse("e35c5102-3d9e-46d5-85a5-c793b9752995");

            // Act
            var result = await _repository.Get(existingId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingId, result.Id);
        }

        [Fact]
        public async Task Get_ReturnsNull_WhenIdDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.Get(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Add_AddsPaymentSuccessfully()
        {
            // Arrange
            var newPayment = new PostPaymentResponse
            {
                Id = Guid.NewGuid(),
                Amount = 3000,
                CardNumberLastFour = 4321,
                Currency = "EUR",
                ExpiryMonth = 8,
                ExpiryYear = 2026,
                Status = PaymentStatus.Authorized
            };

            // Act
            await _repository.Add(newPayment);
            var retrieved = await _repository.Get(newPayment.Id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal(newPayment.Id, retrieved.Id);
            Assert.Equal(newPayment.Amount, retrieved.Amount);
            Assert.Equal(newPayment.CardNumberLastFour, retrieved.CardNumberLastFour);
            Assert.Equal(newPayment.Currency, retrieved.Currency);
            Assert.Equal(newPayment.ExpiryMonth, retrieved.ExpiryMonth);
            Assert.Equal(newPayment.ExpiryYear, retrieved.ExpiryYear);
            Assert.Equal(PaymentStatus.Authorized, retrieved.Status);
        }
    }
}
