using System.Text.Json.Serialization;

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
    [JsonPropertyName("ReadOnly")]
    public ReadOnlyReceiverData ReadOnly { get; set; }

    [JsonPropertyName("ReferenceNum")]
    public string ReferenceNum { get; set; }

    [JsonPropertyName("ArrivalDate")]
    public DateTime? ArrivalDate { get; set; }

    [JsonPropertyName("ReceiveItems")]
    public List<ReceiverItem> ReceiveItems { get; set; } = new();

    [JsonPropertyName("Billing")]
    public object Billing { get; set; }

    [JsonPropertyName("SavedElements")]
    public List<object> SavedElements { get; set; } = new();

    [JsonPropertyName("_links")]
    public List<object> Links { get; set; } = new();
}

public class ReadOnlyReceiverData
{
    [JsonPropertyName("ReceiverId")]
    public int ReceiverId { get; set; }

    [JsonPropertyName("ReceiverType")]
    public int ReceiverType { get; set; }

    [JsonPropertyName("DeferNotification")]
    public bool DeferNotification { get; set; }

    [JsonPropertyName("ReceiptAdviceSendInfo")]
    public ReceiptAdviceSendInfo ReceiptAdviceSendInfo { get; set; }

    [JsonPropertyName("CustomerIdentifier")]
    public Identifier CustomerIdentifier { get; set; }

    [JsonPropertyName("FacilityIdentifier")]
    public Identifier FacilityIdentifier { get; set; }

    [JsonPropertyName("WarehouseTransactionSourceType")]
    public int WarehouseTransactionSourceType { get; set; }

    [JsonPropertyName("TransactionEntryType")]
    public int TransactionEntryType { get; set; }

    [JsonPropertyName("CreationDate")]
    public DateTime CreationDate { get; set; }

    [JsonPropertyName("CreatedByIdentifier")]
    public Identifier CreatedByIdentifier { get; set; }

    [JsonPropertyName("LastModifiedDate")]
    public DateTime LastModifiedDate { get; set; }

    [JsonPropertyName("LastModifiedByIdentifier")]
    public Identifier LastModifiedByIdentifier { get; set; }

    [JsonPropertyName("Status")]
    public int Status { get; set; }
}

public class ReceiptAdviceSendInfo
{
    [JsonPropertyName("SentInfo")]
    public SentInfo SentInfo { get; set; }
}

public class SentInfo
{
    [JsonPropertyName("Sent")]
    public bool Sent { get; set; }
}

public class Identifier
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Id")]
    public int Id { get; set; }
}

public class ReceiverItem
{
    [JsonPropertyName("ReadOnly")]
    public ReadOnlyReceiverItemData ReadOnly { get; set; }

    [JsonPropertyName("ItemIdentifier")]
    public ItemIdentifier ItemIdentifier { get; set; }

    [JsonPropertyName("Qualifier")]
    public string Qualifier { get; set; }

    [JsonPropertyName("Qty")]
    public decimal Qty { get; set; }

    [JsonPropertyName("LotNumber")]
    public string LotNumber { get; set; }

    [JsonPropertyName("ExpirationDate")]
    public DateTime? ExpirationDate { get; set; }

    [JsonPropertyName("WeightImperial")]
    public decimal? WeightImperial { get; set; }

    [JsonPropertyName("WeightMetric")]
    public decimal? WeightMetric { get; set; }

    [JsonPropertyName("OnHold")]
    public bool OnHold { get; set; }

    [JsonPropertyName("SavedElements")]
    public List<object> SavedElements { get; set; } = new();

    [JsonPropertyName("_links")]
    public List<object> Links { get; set; } = new();
}

public class ReadOnlyReceiverItemData
{
    [JsonPropertyName("ReceiveItemId")]
    public int ReceiveItemId { get; set; }

    [JsonPropertyName("FullyShippedDate")]
    public DateTime? FullyShippedDate { get; set; }

    [JsonPropertyName("UnitIdentifier")]
    public Identifier UnitIdentifier { get; set; }

    [JsonPropertyName("FacilityIdentifier")]
    public Identifier FacilityIdentifier { get; set; }

    [JsonPropertyName("ReferenceNumber")]
    public string ReferenceNumber { get; set; }

    [JsonPropertyName("TransactionID")]
    public int TransactionID { get; set; }

    [JsonPropertyName("RowVersion")]
    public string RowVersion { get; set; }
}

public class ItemIdentifier
{
    [JsonPropertyName("Sku")]
    public string Sku { get; set; }

    [JsonPropertyName("Id")]
    public int Id { get; set; }
}