using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class OrdersResponse
{
    [JsonPropertyName("TotalResults")]
    public int TotalResults { get; set; }

    [JsonPropertyName("ResourceList")]
    public List<Order> Orders { get; set; } = new();

    [JsonPropertyName("_links")]
    public List<object> Links { get; set; } = new();
}