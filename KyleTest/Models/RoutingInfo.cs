using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class RoutingInfo
{
    [JsonPropertyName("carrier")]
    public string Carrier { get; set; }

    [JsonPropertyName("mode")]
    public string Mode { get; set; }

    [JsonPropertyName("trackingNumber")]
    public string TrackingNumber { get; set; }

    [JsonPropertyName("scacCode")]
    public string ScacCode { get; set; }

    [JsonPropertyName("isCod")]
    public bool IsCod { get; set; }
}