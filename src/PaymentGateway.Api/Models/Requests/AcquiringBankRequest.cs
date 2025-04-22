using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Requests
{
    public class AcquiringBankRequest
    {
        // JsonPropertyName is used to map the C# property names to the JSON property names
        [JsonPropertyName("card_number")]
        public string CardNumber { get; set; }

        [JsonPropertyName("expiry_date")]
        public string ExpiryDate { get; set; }

        [JsonPropertyName("cvv")]
        public string Cvv { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }
    }
}
