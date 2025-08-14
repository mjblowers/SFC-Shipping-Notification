using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class ReadOnlyOrderData
{
    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }

    [JsonPropertyName("fullyAllocated")]
    public bool FullyAllocated { get; set; }

    [JsonPropertyName("isClosed")]
    public bool IsClosed { get; set; }

    [JsonPropertyName("processDate")]
    public DateTime? ProcessDate { get; set; }

    [JsonPropertyName("pickDoneDate")]
    public DateTime? PickDoneDate { get; set; }

    [JsonPropertyName("packDoneDate")]
    public DateTime? PackDoneDate { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("packages")]
    public List<Package> Packages { get; set; }
}