using System.Text.Json.Serialization;

namespace KyleTest.Models;

public class AddressInfo
{
    [JsonPropertyName("companyName")]
    public string CompanyName { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("address1")]
    public string Address1 { get; set; }

    [JsonPropertyName("address2")]
    public string Address2 { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    //[JsonPropertyName("state")]
    //public string State { get; set; }

    [JsonPropertyName("zip")]
    public string Zip { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; }

    [JsonPropertyName("emailAddress")]
    public string EmailAddress { get; set; }
}