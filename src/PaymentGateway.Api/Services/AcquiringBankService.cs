using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Options;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Models.Settings;
using PaymentGateway.Api.Services.Interfaces;

namespace PaymentGateway.Api.Services
{
    public class AcquiringBankService : IAcquiringBankService
    {
        private readonly string _apiUrl;
        private readonly IHttpClientFactory _httpClientFactory;

        public AcquiringBankService(IOptions<AcquiringBankSettings> options, IHttpClientFactory httpClientFactory)
        {
            _apiUrl = options.Value.ApiUrl;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<AcquiringBankResponse> Authorize(AcquiringBankRequest request)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await httpClient.PostAsync(
                _apiUrl + "/payments",
                new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/Json"));

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error calling Acquiring Bank API: " + response.ReasonPhrase);
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AcquiringBankResponse>(jsonResponse);
        }
    }
}
