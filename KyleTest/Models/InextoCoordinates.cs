using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class InextoCoordinates
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
}