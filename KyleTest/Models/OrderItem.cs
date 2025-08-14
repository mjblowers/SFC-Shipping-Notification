using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class OrderItem
{
    [JsonPropertyName("readOnly")]
    public ReadOnlyOrderItemData ReadOnly { get; set; }

    [JsonPropertyName("itemIdentifier")]
    public ItemIdentifier ItemIdentifier { get; set; }

    [JsonPropertyName("qualifier")]
    public string Qualifier { get; set; }

    [JsonPropertyName("externalId")]
    public string ExternalId { get; set; }

    [JsonPropertyName("qty")]
    public decimal Qty { get; set; }

    [JsonPropertyName("secondaryQty")]
    public decimal? SecondaryQty { get; set; }

    [JsonPropertyName("lotNumber")]
    public string LotNumber { get; set; }

    [JsonPropertyName("serialNumber")]
    public string SerialNumber { get; set; }

    [JsonPropertyName("fulfillInvSalePrice")]
    public decimal? FulfillInvSalePrice { get; set; }

    [JsonPropertyName("notes")]
    public string Notes { get; set; }
}