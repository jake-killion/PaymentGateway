using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services.Interfaces;

namespace PaymentGateway.Api.Services
{
    public class PaymentsValidationService : IPaymentsValidationService
    {
        private readonly List<string> ValidCurrencyCodes =
        [
            "USD", "EUR", "GBP"
        ];

        // Would like to clean this up to make it prettier and more readable however this is a simple example
        public List<string> Validate(PostPaymentRequestMessage request)
        {
            List<string> errors = [];
            bool expiryMonthIsValid = ExpiryMonthIsValid(request.ExpiryMonth);
            if (expiryMonthIsValid == false)
            {
                errors.Add("Invalid Expiry Month");
            }

            bool expiryYearIsValid = ExpiryYearIsValid(request.ExpiryYear);
            if (expiryYearIsValid == false)
            {
                errors.Add("Invalid Expiry Year");
            }

            if (expiryMonthIsValid == true && expiryYearIsValid == true && ExpirationDateIsValid(request.ExpiryMonth, request.ExpiryYear) == false)
            {
                errors.Add("Card has expired");
            }

            if (CvvIsValid(request.Cvv.Trim()) == false)
            {
                errors.Add("Invalid CVV");
            }

            if (string.IsNullOrWhiteSpace(request.CardNumber))
            {
                errors.Add("Card Number is required.");
            }
            else if (!CardNumberIsValid(request.CardNumber.Trim()))
            {
                errors.Add("Invalid Card Number. It must be numeric and between 14 and 19 digits.");
            }

            if (AmountIsValid(request.Amount) == false)
            {
                errors.Add("Invalid Amount. It must be a positive integer.");
            }

            if (string.IsNullOrWhiteSpace(request.Currency))
            {
                errors.Add("Currency is required.");
            }
            else if (CurrencyIsValid(request.Currency) == false)
            {
                errors.Add("Invalid Currency. Must be a supported ISO currency code (USD, EUR, GBP)");
            }
            return errors;
        }

        // Ensures the month is between 1 and 12
        private bool ExpiryMonthIsValid(int month)
        {
            return month >= 1 && month <= 12;
        }

        // Ensures the year is a 4-digit number
        private bool ExpiryYearIsValid(int year)
        {
            return year >= 1000 && year <= 9999;
        }

        // Ensures the expiration date is in the future
        private bool ExpirationDateIsValid(int month, int year)
        {
            DateTime expirationDate = new DateTime(year, month, 1);
            return expirationDate.Date > DateTime.Now.Date;
        }

        // Ensures the CVV is a 3 or 4 digit number
        private bool CvvIsValid(string cvv)
        {
            return int.TryParse(cvv, out _) 
                && (cvv.Length == 3
                || cvv.Length == 4);
        }

        // Ensures the card number is numeric and between 14 and 19 digits
        private bool CardNumberIsValid(string cardNumber)
        {
            return long.TryParse(cardNumber, out _)
                && cardNumber.Length >= 14
                && cardNumber.Length <= 19;
        }

        // Ensures the amount is a positive integer
        private bool AmountIsValid(int amount)
        {
            return amount > 0;
        }

        // Ensures the currency is a valid ISO currency code
        private bool CurrencyIsValid(string currency)
        {
            return currency.Length == 3 && ValidCurrencyCodes.Contains(currency);
        }
    }
}
