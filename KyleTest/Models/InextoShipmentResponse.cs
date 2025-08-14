using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class InextoShipmentResponse
{
    [JsonPropertyName("transmissionUid")]
    public string TransmissionUid { get; set; }

    [JsonPropertyName("internalUid")]
    public string InternalUid { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("errors")]
    public Dictionary<string, string[]> Errors { get; set; }
}