using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class ReadOnlyOrderItemData
{
    [JsonPropertyName("orderItemId")]
    public int OrderItemId { get; set; }

    [JsonPropertyName("fullyAllocated")]
    public bool FullyAllocated { get; set; }
}