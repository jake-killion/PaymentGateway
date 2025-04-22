using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Responses
{
    public class AcquiringBankResponse
    {
        // JsonPropertyName is used to map the C# property names to the JSON property names
        [JsonPropertyName("authorized")]
        public bool Authorized { get; set; }

        [JsonPropertyName("authorization_code")]
        public string AuthorizationCode { get; set; }
    }
}
