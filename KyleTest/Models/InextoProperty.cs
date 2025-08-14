using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class InextoProperty
{
    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}