using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class InextoBusinessEntity
{
    [JsonPropertyName("keys")]
    public string[] Keys { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("properties")]
    public InextoProperty[] Properties { get; set; }
}