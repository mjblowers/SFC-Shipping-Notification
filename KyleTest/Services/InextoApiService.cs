using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using KyleTest.Models;

namespace KyleTest.Services;

public class InextoApiService : IInextoApiService
{
    private readonly HttpClient _inextoClient;

    public InextoApiService(HttpClient inextoClient)
    {
        _inextoClient = inextoClient;
    }

    public async Task<InextoShipmentResponse> SendShipmentEventAsync(InextoShipmentRequest request, string accessToken)
    {
        _inextoClient.DefaultRequestHeaders.Clear();
        var endpoint = "standard/event/shipmentEvent?Subscription-Key=a754b9e5ee024362b5cc4c52c54e3c72";
        var response = await _inextoClient.PostAsJsonAsync(endpoint, request);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return JsonSerializer.Deserialize<InextoShipmentResponse>(jsonResponse, options);
    }
}