using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using KyleTest.Models;

namespace KyleTest.Services;

public class WmsReceiverService : IWmsReceiverService
{
    private readonly HttpClient _receiverClient;

    public WmsReceiverService(HttpClient receiverClient)
    {
        _receiverClient = receiverClient;
    }

    public async Task<ReceiverResponse> GetReceiversAsync(string accessToken)
    {
        // Match the orders call: clear/set only the same headers (no subscription key)
        _receiverClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var now = DateTime.UtcNow;
        var from = now.AddHours(-72);
        string fromStr = from.ToString("yyyy-MM-ddTHH:mm:ss");
        string toStr = now.ToString("yyyy-MM-ddTHH:mm:ss");
        var rql = $"readonly.processDate=gt={fromStr};readonly.processDate=lt={toStr}";

        var queryParams = new[]
        {
            "pgsiz=1",
            "pgnum=1",
            "detail=All",
            "itemdetail=All"
        };
        //var queryParams = new[]
        //{
        //    "receiver_id=100",
        //};

        var endpoint = "inventory/receivers?" + string.Join("&", queryParams);

        var response = await _receiverClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        if (jsonResponse.TrimStart().StartsWith("["))
        {
            var receiversArray = JsonSerializer.Deserialize<Receiver[]>(jsonResponse, options);
            return new ReceiverResponse { Receivers = receiversArray?.ToList() ?? new List<Receiver>() };
        }
        else
        {
            return JsonSerializer.Deserialize<ReceiverResponse>(jsonResponse, options) ?? new ReceiverResponse();
        }
    }
}