using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Services
{
    public class PaymentsValidationServiceTests
    {
        private readonly PaymentsValidationService _service;

        public PaymentsValidationServiceTests()
        {
            _service = new PaymentsValidationService();
        }

        [Fact]
        public void Validate_ShouldReturnErrors_WhenRequestIsInvalid()
        {
            // Arrange
            var request = new PostPaymentRequestMessage
            {
                CardNumber = "123", // Invalid card number
                ExpiryMonth = 13, // Invalid month
                ExpiryYear = 999, // Invalid year
                Cvv = "12", // Invalid CVV
                Amount = -1, // Invalid amount
                Currency = "XYZ" // Invalid currency
            };

            // Act
            var errors = _service.Validate(request);

            // Assert
            Assert.Contains("Invalid Expiry Month", errors);
            Assert.Contains("Invalid Expiry Year", errors);
            Assert.Contains("Invalid Card Number. It must be numeric and between 14 and 19 digits.", errors);
            Assert.Contains("Invalid CVV", errors);
            Assert.Contains("Invalid Amount. It must be a positive integer.", errors);
            Assert.Contains("Invalid Currency. Must be a supported ISO currency code (USD, EUR, GBP)", errors);
        }

        [Fact]
        public void Validate_ShouldReturnNoErrors_WhenRequestIsValid()
        {
            // Arrange
            var request = new PostPaymentRequestMessage
            {
                CardNumber = "4111111111111111", // Valid card number
                ExpiryMonth = 12, // Valid month
                ExpiryYear = DateTime.Now.Year + 1, // Valid year
                Cvv = "123", // Valid CVV
                Amount = 100, // Valid amount
                Currency = "USD" // Valid currency
            };

            // Act
            var errors = _service.Validate(request);

            // Assert
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(13, false)]
        [InlineData(1, true)]
        [InlineData(12, true)]
        public void ExpiryMonthIsValid_ShouldValidateMonth(int month, bool expected)
        {
            // Act
            var result = _service.GetType()
                .GetMethod("ExpiryMonthIsValid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_service, new object[] { month });

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(999, false)]
        [InlineData(10000, false)]
        [InlineData(2025, true)]
        public void ExpiryYearIsValid_ShouldValidateYear(int year, bool expected)
        {
            // Act
            var result = _service.GetType()
                .GetMethod("ExpiryYearIsValid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_service, new object[] { year });

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ExpirationDateIsValid_ShouldReturnFalse_WhenDateIsInPast()
        {
            // Act
            var result = _service.GetType()
                .GetMethod("ExpirationDateIsValid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_service, new object[] { 1, 2020 });

            // Assert
            Assert.False((bool)result);
        }

        [Fact]
        public void ExpirationDateIsValid_ShouldReturnTrue_WhenDateIsInFuture()
        {
            // Act
            var result = _service.GetType()
                .GetMethod("ExpirationDateIsValid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_service, new object[] { 12, DateTime.Now.Year + 1 });

            // Assert
            Assert.True((bool)result);
        }

        [Theory]
        [InlineData("123", true)]
        [InlineData("1234", true)]
        [InlineData("12", false)]
        [InlineData("12345", false)]
        public void CvvIsValid_ShouldValidateCvv(string cvv, bool expected)
        {
            // Act
            var result = _service.GetType()
                .GetMethod("CvvIsValid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_service, new object[] { cvv });

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("4111111111111111", true)]
        [InlineData("123456789012", false)]
        [InlineData("12345678901234567890", false)]
        [InlineData("abcd1234", false)]
        public void CardNumberIsValid_ShouldValidateCardNumber(string cardNumber, bool expected)
        {
            // Act
            var result = _service.GetType()
                .GetMethod("CardNumberIsValid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_service, new object[] { cardNumber });

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(-1, false)]
        [InlineData(100, true)]
        public void AmountIsValid_ShouldValidateAmount(int amount, bool expected)
        {
            // Act
            var result = _service.GetType()
                .GetMethod("AmountIsValid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_service, new object[] { amount });

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("USD", true)]
        [InlineData("EUR", true)]
        [InlineData("GBP", true)]
        [InlineData("XYZ", false)]
        [InlineData("", false)]
        public void CurrencyIsValid_ShouldValidateCurrency(string currency, bool expected)
        {
            // Act
            var result = _service.GetType()
                .GetMethod("CurrencyIsValid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_service, new object[] { currency });

            // Assert
            Assert.Equal(expected, result);
        }

    }
}