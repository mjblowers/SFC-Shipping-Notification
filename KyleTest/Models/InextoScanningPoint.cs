using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class InextoScanningPoint
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}