using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class BillingInfo
{
    [JsonPropertyName("billingCharges")]
    public List<BillingCharge> BillingCharges { get; set; }
}