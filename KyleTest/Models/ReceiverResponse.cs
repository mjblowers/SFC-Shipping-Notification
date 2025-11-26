using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class ReceiverResponse
{
    [JsonPropertyName("TotalResults")]
    public int TotalResults { get; set; }

    [JsonPropertyName("ResourceList")]
    public List<Receiver> Receivers { get; set; } = new();

    [JsonPropertyName("_links")]
    public List<object> Links { get; set; } = new();
}

public class Receiver
{
    [JsonPropertyName("readOnly")]
    public ReadOnlyReceiverData ReadOnly { get; set; }

    [JsonPropertyName("referenceNum")]
    public string ReferenceNum { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("poNum")]
    public string PoNum { get; set; }

    [JsonPropertyName("externalId")]
    public string ExternalId { get; set; }

    [JsonPropertyName("earliestExpectedReceiptDate")]
    public DateTime? EarliestExpectedReceiptDate { get; set; }

    [JsonPropertyName("expectedReceiptDate")]
    public DateTime? ExpectedReceiptDate { get; set; }

    [JsonPropertyName("notes")]
    public string Notes { get; set; }

    [JsonPropertyName("totalWeight")]
    public decimal? TotalWeight { get; set; }

    [JsonPropertyName("totalVolume")]
    public decimal? TotalVolume { get; set; }

    [JsonPropertyName("shipFrom")]
    public AddressInfo ShipFrom { get; set; }

    [JsonPropertyName("billTo")]
    public AddressInfo BillTo { get; set; }

    [JsonPropertyName("_embedded")]
    public EmbeddedReceiverData Embedded { get; set; }
}

public class ReadOnlyReceiverData
{
    [JsonPropertyName("receiverId")]
    public int ReceiverId { get; set; }

    [JsonPropertyName("isClosed")]
    public bool IsClosed { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("processDate")]
    public DateTime? ProcessDate { get; set; }
}

public class EmbeddedReceiverData
{
    [JsonPropertyName("http://api.3plCentral.com/rels/receivers/item")]
    public List<ReceiverItem> ReceiverItems { get; set; }
}

public class ReceiverItem
{
    [JsonPropertyName("readOnly")]
    public ReadOnlyReceiverItemData ReadOnly { get; set; }

    [JsonPropertyName("itemIdentifier")]
    public ItemIdentifier ItemIdentifier { get; set; }

    [JsonPropertyName("qty")]
    public decimal Qty { get; set; }

    [JsonPropertyName("lotNumber")]
    public string LotNumber { get; set; }

    [JsonPropertyName("serialNumber")]
    public string SerialNumber { get; set; }
}

public class ReadOnlyReceiverItemData
{
    [JsonPropertyName("receiverItemId")]
    public int ReceiverItemId { get; set; }
}