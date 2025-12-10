using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class Order
{
    [JsonPropertyName("customerIdentifier")]
    public int CustomerIdentifier { get; set; }

    [JsonPropertyName("readOnly")]
    public ReadOnlyOrderData ReadOnly { get; set; }

    [JsonPropertyName("referenceNum")]
    public string ReferenceNum { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("poNum")]
    public string PoNum { get; set; }

    [JsonPropertyName("externalId")]
    public string ExternalId { get; set; }

    [JsonPropertyName("earliestShipDate")]
    public DateTime? EarliestShipDate { get; set; }

    [JsonPropertyName("shipCancelDate")]
    public DateTime? ShipCancelDate { get; set; }

    [JsonPropertyName("notes")]
    public string Notes { get; set; }

    [JsonPropertyName("totalWeight")]
    public decimal? TotalWeight { get; set; }

    [JsonPropertyName("totalVolume")]
    public decimal? TotalVolume { get; set; }

    [JsonPropertyName("billingCode")]
    public string BillingCode { get; set; }

    [JsonPropertyName("shipTo")]
    public AddressInfo ShipTo { get; set; }

    [JsonPropertyName("soldTo")]
    public AddressInfo SoldTo { get; set; }

    [JsonPropertyName("billTo")]
    public AddressInfo BillTo { get; set; }

    [JsonPropertyName("routingInfo")]
    public RoutingInfo RoutingInfo { get; set; }

    [JsonPropertyName("billing")]
    public BillingInfo Billing { get; set; }

    [JsonPropertyName("_embedded")]
    public EmbeddedOrderData Embedded { get; set; }
}