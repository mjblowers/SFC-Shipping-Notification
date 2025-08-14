using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class InextoItem
{
    [JsonPropertyName("machineReadableCode")]
    public string MachineReadableCode { get; set; }
}