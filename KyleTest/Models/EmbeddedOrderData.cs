using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class EmbeddedOrderData
{
    [JsonPropertyName("http://api.3plCentral.com/rels/orders/item")]
    public List<OrderItem> OrderItems { get; set; }
}