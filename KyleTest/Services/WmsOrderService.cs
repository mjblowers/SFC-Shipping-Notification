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

        // Calculate the past 24 hours window in ISO 8601 format (UTC)
        var now = DateTime.UtcNow;
        var from = now.AddHours(-72);
        string fromStr = from.ToString("yyyy-MM-ddTHH:mm:ss");
        string toStr = now.ToString("yyyy-MM-ddTHH:mm:ss");

        // Build RQL for shipped orders in the past 24 hours
        var rql = $"readonly.processDate=gt={fromStr};readonly.processDate=lt={toStr}";

        var queryParams = new[]
        {
            "pgsiz=100",
            "pgnum=1",
            $"rql={rql}",
            "detail=All",
            "itemdetail=All"
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