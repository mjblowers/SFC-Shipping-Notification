using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class BillingCharge
{
    [JsonPropertyName("chargeType")]
    public int ChargeType { get; set; }

    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; }
}