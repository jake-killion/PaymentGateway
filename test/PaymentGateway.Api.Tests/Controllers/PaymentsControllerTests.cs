using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services.Interfaces;

namespace PaymentGateway.Tests.Controllers
{
    public class PaymentsControllerTests
    {
        private readonly Mock<IPaymentsService> _mockService;
        private readonly PaymentsController _controller;

        public PaymentsControllerTests()
        {
            _mockService = new Mock<IPaymentsService>();
            _controller = new PaymentsController(_mockService.Object);
        }

        [Fact]
        public async Task PostPaymentAsync_ReturnsOk_WhenPaymentIsSuccessful()
        {
            // Arrange
            var request = GetValidPaymentRequest();
            var expectedResult = new PostPaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = PaymentStatus.Authorized,
                CardNumberLastFour = 1111,
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Currency = "USD",
                Amount = 100
            };
            _mockService.Setup(s => s.Process(request)).ReturnsAsync(new ProcessPaymentResponse
            {
                Result = expectedResult,
                ErrorMessages = new List<string>()
            });

            // Act
            var result = await _controller.PostPaymentAsync(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<PostPaymentResponse>(okResult.Value);
            Assert.Equal(expectedResult.Id, response.Id);
            Assert.Equal(expectedResult.Status, response.Status);
            Assert.Equal(expectedResult.CardNumberLastFour, response.CardNumberLastFour);
            Assert.Equal(expectedResult.ExpiryMonth, response.ExpiryMonth);
            Assert.Equal(expectedResult.ExpiryYear, response.ExpiryYear);
            Assert.Equal(expectedResult.Currency, response.Currency);
            Assert.Equal(expectedResult.Amount, response.Amount);
        }

        [Fact]
        public async Task PostPaymentAsync_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var request = GetValidPaymentRequest();
            var responseWithErrors = new ProcessPaymentResponse
            {
                ErrorMessages = new List<string> { "Invalid CVV" }
            };
            _mockService.Setup(s => s.Process(request)).ReturnsAsync(responseWithErrors);

            // Act
            var result = await _controller.PostPaymentAsync(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(responseWithErrors, badRequest.Value);
        }

        [Fact]
        public async Task PostPaymentAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = GetValidPaymentRequest();
            _mockService.Setup(s => s.Process(request)).ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.PostPaymentAsync(request);

            // Assert
            var serverError = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, serverError.StatusCode);
        }

        [Fact]
        public async Task GetPaymentAsync_ReturnsOk_WhenPaymentExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var payment = new GetPaymentResponse { Id = id };
            _mockService.Setup(s => s.Get(id)).ReturnsAsync(payment);

            // Act
            var result = await _controller.GetPaymentAsync(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPayment = Assert.IsType<GetPaymentResponse>(okResult.Value);
            Assert.Equal(id, returnedPayment.Id);
        }

        [Fact]
        public async Task GetPaymentAsync_ReturnsNotFound_WhenPaymentDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.Get(id)).ReturnsAsync((GetPaymentResponse)null);

            // Act
            var result = await _controller.GetPaymentAsync(id);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains(id.ToString(), notFound.Value.ToString());
        }

        [Fact]
        public async Task GetPaymentAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.Get(id)).ThrowsAsync(new Exception("Failure"));

            // Act
            var result = await _controller.GetPaymentAsync(id);

            // Assert
            var serverError = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, serverError.StatusCode);
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
