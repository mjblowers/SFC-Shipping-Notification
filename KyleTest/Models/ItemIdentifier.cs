using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class ItemIdentifier
{
    [JsonPropertyName("sku")]
    public string Sku { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }
}