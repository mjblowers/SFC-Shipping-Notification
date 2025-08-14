using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using KyleTest.Models;

namespace KyleTest.Services;

public class WmsOrderService : IWmsOrderService
{
    private readonly HttpClient _orderClient;

    public WmsOrderService(HttpClient orderClient)
    {
        _orderClient = orderClient;
    }

    public async Task<OrdersResponse> GetOrdersAsync(string accessToken)
    {
        _orderClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var queryParams = new[]
        {
            "pgsiz=100",
            "pgnum=1",
            "detail=OrderItems",
            "itemdetail=None"
        };

        var endpoint = "orders?" + string.Join("&", queryParams);
        var response = await _orderClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        if (jsonResponse.TrimStart().StartsWith("["))
        {
            var ordersArray = JsonSerializer.Deserialize<Order[]>(jsonResponse, options);
            return new OrdersResponse { Orders = ordersArray?.ToList() ?? new List<Order>() };
        }
        else
        {
            return JsonSerializer.Deserialize<OrdersResponse>(jsonResponse, options) ?? new OrdersResponse();
        }
    }
}