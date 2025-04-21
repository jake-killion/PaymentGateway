using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories.Interfaces;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Services.Interfaces;

namespace PaymentGateway.Tests.Services
{
    public class PaymentsServiceTests
    {
        private readonly Mock<IPaymentsRepository> _mockRepository;
        private readonly Mock<IPaymentsValidationService> _mockValidator;
        private readonly Mock<IAcquiringBankService> _mockBank;
        private readonly PaymentsService _service;

        public PaymentsServiceTests()
        {
            _mockRepository = new Mock<IPaymentsRepository>();
            _mockValidator = new Mock<IPaymentsValidationService>();
            _mockBank = new Mock<IAcquiringBankService>();

            _service = new PaymentsService(
                _mockRepository.Object,
                _mockValidator.Object,
                _mockBank.Object
            );
        }

        [Fact]
        public async Task Process_ReturnsRejected_WhenValidationFails()
        {
            // Arrange
            var request = GetValidPaymentRequest();
            _mockValidator.Setup(v => v.Validate(request))
                .Returns(new List<string> { "Invalid Card Number" });

            // Act
            var result = await _service.Process(request);

            // Assert
            Assert.Equal(PaymentStatus.Rejected, result.PaymentStatus);
            Assert.Contains("Invalid Card Number", result.ErrorMessages);
            _mockRepository.Verify(r => r.Add(It.IsAny<PostPaymentResponse>()), Times.Once);
        }

        [Fact]
        public async Task Process_ReturnsDeclined_WhenBankAuthorizationFails()
        {
            // Arrange
            var request = GetValidPaymentRequest();
            _mockValidator.Setup(v => v.Validate(request))
                .Returns(new List<string>());

            _mockBank.Setup(b => b.Authorize(It.IsAny<AcquiringBankRequest>()))
                .ReturnsAsync(new AcquiringBankResponse { Authorized = false });

            // Act
            var result = await _service.Process(request);

            // Assert
            Assert.Equal(PaymentStatus.Declined, result.PaymentStatus);
            Assert.Contains("Payment could not be processed.", result.ErrorMessages);
            _mockRepository.Verify(r => r.Add(It.IsAny<PostPaymentResponse>()), Times.Never);
        }

        [Fact]
        public async Task Process_ReturnsAuthorized_WhenValidRequestAndBankApproves()
        {
            // Arrange
            var request = GetValidPaymentRequest();
            _mockValidator.Setup(v => v.Validate(request))
                .Returns(new List<string>());

            _mockBank.Setup(b => b.Authorize(It.IsAny<AcquiringBankRequest>()))
                .ReturnsAsync(new AcquiringBankResponse { Authorized = true });

            // Act
            var result = await _service.Process(request);

            // Assert
            Assert.Equal(PaymentStatus.Authorized, result.PaymentStatus);
            Assert.Empty(result.ErrorMessages);
            _mockRepository.Verify(r => r.Add(It.IsAny<PostPaymentResponse>()), Times.Once);
        }

        [Fact]
        public async Task Get_ReturnsPayment_WhenExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = new GetPaymentResponse { Id = id };

            _mockRepository.Setup(r => r.Get(id)).ReturnsAsync(expected);

            // Act
            var result = await _service.Get(id);

            // Assert
            Assert.Equal(id, result.Id);
        }

        private PostPaymentRequestMessage GetValidPaymentRequest()
        {
            return new PostPaymentRequestMessage
            {
                CardNumber = "4111111111111111",
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Cvv = "123",
                Amount = 100,
                Currency = "USD"
            };
        }
    }
}
