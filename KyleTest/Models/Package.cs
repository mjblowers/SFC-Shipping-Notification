using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class Package
{
    [JsonPropertyName("packageId")]
    public int PackageId { get; set; }

    [JsonPropertyName("trackingNumber")]
    public string TrackingNumber { get; set; }

    [JsonPropertyName("weight")]
    public decimal? Weight { get; set; }

    [JsonPropertyName("length")]
    public decimal? Length { get; set; }

    [JsonPropertyName("width")]
    public decimal? Width { get; set; }

    [JsonPropertyName("height")]
    public decimal? Height { get; set; }

    [JsonPropertyName("packageContents")]
    public List<PackageContent> PackageContents { get; set; }
}