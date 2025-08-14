using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class InextoBusinessEntityWithRelation : InextoBusinessEntity
{
    [JsonPropertyName("relation")]
    public string Relation { get; set; }

    [JsonPropertyName("address1")]
    public string Address1 { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("zip")]
    public string Zip { get; set; }
}