using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class PackageContent
{
    [JsonPropertyName("packageContentId")]
    public int PackageContentId { get; set; }

    [JsonPropertyName("orderItemId")]
    public int OrderItemId { get; set; }

    [JsonPropertyName("qty")]
    public decimal Qty { get; set; }

    [JsonPropertyName("serialNumber")]
    public string SerialNumber { get; set; }

    [JsonPropertyName("lotNumber")]
    public string LotNumber { get; set; }
}