using System;
using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class InextoArrivalEventRequest
{
    [JsonPropertyName("transmissionUid")]
    public string TransmissionUid { get; set; }

    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("eventDateTime")]
    public DateTimeOffset EventDateTime { get; set; }

    [JsonPropertyName("isReturned")]
    public bool IsReturned { get; set; }

    //[JsonPropertyName("comment")]
    //public string Comment { get; set; }

    //[JsonPropertyName("properties")]
    //public InextoProperty[] Properties { get; set; }

    //[JsonPropertyName("coordinates")]
    //public InextoCoordinates Coordinates { get; set; }

    [JsonPropertyName("documents")]
    public string[] Documents { get; set; }

    [JsonPropertyName("scanningLocation")]
    public InextoBusinessEntity ScanningLocation { get; set; }

    [JsonPropertyName("scanningPoint")]
    public InextoScanningPoint ScanningPoint { get; set; }

    [JsonPropertyName("items")]
    public InextoItem[] Items { get; set; }
}