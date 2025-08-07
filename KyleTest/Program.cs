using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

class Program
{
    // It's recommended to reuse HttpClient instances
    private static readonly HttpClient _authClient = new()
    {
        BaseAddress = new Uri("https://secure-wms.com/"), // Authentication server
    };

    private static readonly HttpClient _firstVendorClient = new()
    {
        BaseAddress = new Uri("https://secure-wms.com/"), // WMS API
    };

    private static readonly HttpClient _secondVendorClient = new()
    {
        BaseAddress = new Uri("https://api.secondvendor.com/"), // Replace with actual vendor URL
    };

    static async Task Main(string[] args)
    {
        try
        {
            // First, get authentication token
            var authToken = await GetAuthorizationTokenAsync();

            if (authToken == null || string.IsNullOrEmpty(authToken.AccessToken))
            {
                Console.WriteLine("Failed to get authentication token");
                return;
            }

            Console.WriteLine("Authentication successful!");

            // Call first vendor's API with authentication (get orders)
            var ordersResponse = await CallFirstVendorApiAsync(authToken.AccessToken);

            Console.WriteLine($"Retrieved {ordersResponse?.Orders?.Count ?? 0} orders");

            // Display some order details for verification
            if (ordersResponse?.Orders?.Any() == true)
            {
                foreach (var order in ordersResponse.Orders.Take(3)) // Show first 3 orders
                {
                    Console.WriteLine($"\nOrder Details:");
                    Console.WriteLine($"  Order ID: {order.ReadOnly?.OrderId}");
                    Console.WriteLine($"  Reference: {order.ReferenceNum}");
                    Console.WriteLine($"  Description: {order.Description}");
                    Console.WriteLine($"  Status: {order.ReadOnly?.Status}");
                    Console.WriteLine($"  Is Closed: {order.ReadOnly?.IsClosed}");
                    Console.WriteLine($"  Ship To: {order.ShipTo?.CompanyName} - {order.ShipTo?.City}, {order.ShipTo?.State}");

                    // Show order items
                    var orderItems = order.Embedded?.OrderItems;
                    if (orderItems?.Any() == true)
                    {
                        Console.WriteLine($"  Order Items ({orderItems.Count}):");
                        foreach (var item in orderItems.Take(2)) // Show first 2 items per order
                        {
                            Console.WriteLine($"    - SKU: {item.ItemIdentifier?.Sku}, Qty: {item.Qty}, Price: {item.FulfillInvSalePrice:C}");
                        }
                    }

                    // Show package info if available
                    var packages = order.ReadOnly?.Packages;
                    if (packages?.Any() == true)
                    {
                        Console.WriteLine($"  Packages ({packages.Count}):");
                        foreach (var package in packages.Take(2))
                        {
                            Console.WriteLine($"    - Package ID: {package.PackageId}, Tracking: {package.TrackingNumber}, Weight: {package.Weight}");
                        }
                    }
                }
            }

            // Process the response and prepare data for second vendor
            var processedData = ProcessDataForSecondVendor(ordersResponse);

            // Call second vendor's API with processed data
            var finalResult = await CallSecondVendorApiAsync(processedData, authToken.AccessToken);

            Console.WriteLine("Process completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static async Task<AuthenticationDTO> GetAuthorizationTokenAsync()
    {
        var responseObj = new AuthenticationDTO();

        var payload = new PayloadDTO
        {
            user_login = "1",
            grant_type = "client_credentials"
        };

        try
        {
            // Prepare authentication credentials
            string credentials = "a2c80ff1-7ce3-422e-987a-d899f3e03641:v1qLTJ2+yRuJa0UfCDK7FPUavf25cAMp";
            byte[] encodedBytes = System.Text.Encoding.UTF8.GetBytes(credentials);
            string encodedCredentials = System.Convert.ToBase64String(encodedBytes);

            // Set up headers
            _authClient.DefaultRequestHeaders.Clear();
            _authClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _authClient.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
            _authClient.DefaultRequestHeaders.Add("Host", "secure-wms.com");
            _authClient.DefaultRequestHeaders.Add("Accept-Language", "Content-Length");
            _authClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate,sdch");
            _authClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);

            // Make the authentication request
            var response = await _authClient.PostAsJsonAsync("AuthServer/api/Token", payload);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Auth response: {result}"); // Debug: see the actual JSON response

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                responseObj = JsonSerializer.Deserialize<AuthenticationDTO>(result, options);
                Console.WriteLine("Authentication token received successfully");
            }
            else
            {
                Console.WriteLine($"Authentication failed: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Authentication error: {ex.Message}");
        }

        return responseObj;
    }

    private static async Task<OrdersResponse> CallFirstVendorApiAsync(string accessToken)
    {
        // Add authentication header
        _firstVendorClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // Build the orders endpoint with parameters
        var queryParams = new List<string>
        {
            "pgsiz=100",           // Page size (max 1000)
            "pgnum=1",             // Page number (1-indexed)
            "detail=OrderItems",   // Include order items in response
            "itemdetail=None"      // Item detail level
            // Add other parameters as needed:
            // "rql=" + Uri.EscapeDataString("your RQL query here"),
            // "sort=" + Uri.EscapeDataString("your sort criteria"),
            // "markforlistid=123",
            // "skulist=" + Uri.EscapeDataString("SKU1,SKU2"),
            // "skucontains=" + Uri.EscapeDataString("partial SKU name")
        };

        var endpoint = "orders?" + string.Join("&", queryParams);
        Console.WriteLine($"Calling orders endpoint: {endpoint}");

        var response = await _firstVendorClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Orders response received (first 500 chars): {jsonResponse.Substring(0, Math.Min(500, jsonResponse.Length))}...");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        try
        {
            // Let's first try to parse as a generic object to understand the structure
            using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
            {
                var root = doc.RootElement;
                Console.WriteLine($"JSON Root Type: {root.ValueKind}");

                if (root.ValueKind == JsonValueKind.Object)
                {
                    Console.WriteLine("Root properties:");
                    foreach (JsonProperty property in root.EnumerateObject())
                    {
                        Console.WriteLine($"  - {property.Name}: {property.Value.ValueKind}");
                    }
                }
                else if (root.ValueKind == JsonValueKind.Array)
                {
                    Console.WriteLine($"Array with {root.GetArrayLength()} elements");
                }
            }

            // Try to deserialize as a direct array first
            if (jsonResponse.TrimStart().StartsWith("["))
            {
                var ordersArray = JsonSerializer.Deserialize<Order[]>(jsonResponse, options);
                return new OrdersResponse { Orders = ordersArray?.ToList() ?? new List<Order>() };
            }
            else
            {
                // Try as the actual wrapper object structure
                var response2 = JsonSerializer.Deserialize<OrdersResponse>(jsonResponse, options);
                Console.WriteLine($"Deserialized {response2?.TotalResults} total results with {response2?.Orders?.Count} orders in ResourceList");
                return response2 ?? new OrdersResponse();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Deserialization error: {ex.Message}");
            Console.WriteLine($"Full JSON response: {jsonResponse}");

            // Return empty response to continue execution
            return new OrdersResponse { Orders = new List<Order>() };
        }
    }

    private static SecondVendorRequest ProcessDataForSecondVendor(OrdersResponse ordersResponse)
    {
        // TODO: Transform orders response into second vendor's request format
        return new SecondVendorRequest
        {
            // Example: Map order data to second vendor format
            // ProcessedOrders = ordersResponse.Orders?.Select(o => new ProcessedOrder { ... }).ToList()
        };
    }

    private static async Task<SecondVendorResponse> CallSecondVendorApiAsync(SecondVendorRequest request, string accessToken)
    {
        // Add authentication header
        _secondVendorClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _secondVendorClient.PostAsJsonAsync("endpoint", request); // Replace with actual endpoint
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<SecondVendorResponse>();
    }
}

// DTO Classes (you'll need to add these to your project)
public class AuthenticationDTO
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    // Common alternative property names - add JsonPropertyName attributes as needed
    // [JsonPropertyName("refresh_token")]
    // public string RefreshToken { get; set; }

    // Add other properties as needed based on your API response
}

public class PayloadDTO
{
    public string user_login { get; set; }
    public string grant_type { get; set; }
}

// WMS Orders Response Models
public class OrdersResponse
{
    [JsonPropertyName("TotalResults")]
    public int TotalResults { get; set; }

    [JsonPropertyName("ResourceList")]
    public List<Order> Orders { get; set; } = new List<Order>();

    [JsonPropertyName("_links")]
    public List<object> Links { get; set; } = new List<object>();
}

public class Order
{
    [JsonPropertyName("readOnly")]
    public ReadOnlyOrderData ReadOnly { get; set; }

    [JsonPropertyName("referenceNum")]
    public string ReferenceNum { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("poNum")]
    public string PoNum { get; set; }

    [JsonPropertyName("externalId")]
    public string ExternalId { get; set; }

    [JsonPropertyName("earliestShipDate")]
    public DateTime? EarliestShipDate { get; set; }

    [JsonPropertyName("shipCancelDate")]
    public DateTime? ShipCancelDate { get; set; }

    [JsonPropertyName("notes")]
    public string Notes { get; set; }

    [JsonPropertyName("totalWeight")]
    public decimal? TotalWeight { get; set; }

    [JsonPropertyName("totalVolume")]
    public decimal? TotalVolume { get; set; }

    [JsonPropertyName("billingCode")]
    public string BillingCode { get; set; }

    [JsonPropertyName("shipTo")]
    public AddressInfo ShipTo { get; set; }

    [JsonPropertyName("soldTo")]
    public AddressInfo SoldTo { get; set; }

    [JsonPropertyName("billTo")]
    public AddressInfo BillTo { get; set; }

    [JsonPropertyName("routingInfo")]
    public RoutingInfo RoutingInfo { get; set; }

    [JsonPropertyName("billing")]
    public BillingInfo Billing { get; set; }

    [JsonPropertyName("_embedded")]
    public EmbeddedOrderData Embedded { get; set; }
}

public class ReadOnlyOrderData
{
    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }

    [JsonPropertyName("fullyAllocated")]
    public bool FullyAllocated { get; set; }

    [JsonPropertyName("isClosed")]
    public bool IsClosed { get; set; }

    [JsonPropertyName("processDate")]
    public DateTime? ProcessDate { get; set; }

    [JsonPropertyName("pickDoneDate")]
    public DateTime? PickDoneDate { get; set; }

    [JsonPropertyName("packDoneDate")]
    public DateTime? PackDoneDate { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("packages")]
    public List<Package> Packages { get; set; }
}

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

public class PackageContent
{
    [JsonPropertyName("packageContentId")]
    public int PackageContentId { get; set; }

    [JsonPropertyName("orderItemId")]
    public int OrderItemId { get; set; }

    [JsonPropertyName("qty")]
    public decimal Qty { get; set; }

    [JsonPropertyName("serialNumber")]
    public string SerialNumber { get; set; }

    [JsonPropertyName("lotNumber")]
    public string LotNumber { get; set; }
}

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

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("zip")]
    public string Zip { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; }

    [JsonPropertyName("emailAddress")]
    public string EmailAddress { get; set; }
}

public class RoutingInfo
{
    [JsonPropertyName("carrier")]
    public string Carrier { get; set; }

    [JsonPropertyName("mode")]
    public string Mode { get; set; }

    [JsonPropertyName("trackingNumber")]
    public string TrackingNumber { get; set; }

    [JsonPropertyName("scacCode")]
    public string ScacCode { get; set; }

    [JsonPropertyName("isCod")]
    public bool IsCod { get; set; }
}

public class BillingInfo
{
    [JsonPropertyName("billingCharges")]
    public List<BillingCharge> BillingCharges { get; set; }
}

public class BillingCharge
{
    [JsonPropertyName("chargeType")]
    public int ChargeType { get; set; }

    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; }
}

public class EmbeddedOrderData
{
    [JsonPropertyName("http://api.3plCentral.com/rels/orders/item")]
    public List<OrderItem> OrderItems { get; set; }
}

public class OrderItem
{
    [JsonPropertyName("readOnly")]
    public ReadOnlyOrderItemData ReadOnly { get; set; }

    [JsonPropertyName("itemIdentifier")]
    public ItemIdentifier ItemIdentifier { get; set; }

    [JsonPropertyName("qualifier")]
    public string Qualifier { get; set; }

    [JsonPropertyName("externalId")]
    public string ExternalId { get; set; }

    [JsonPropertyName("qty")]
    public decimal Qty { get; set; }

    [JsonPropertyName("secondaryQty")]
    public decimal? SecondaryQty { get; set; }

    [JsonPropertyName("lotNumber")]
    public string LotNumber { get; set; }

    [JsonPropertyName("serialNumber")]
    public string SerialNumber { get; set; }

    [JsonPropertyName("fulfillInvSalePrice")]
    public decimal? FulfillInvSalePrice { get; set; }

    [JsonPropertyName("notes")]
    public string Notes { get; set; }
}

public class ReadOnlyOrderItemData
{
    [JsonPropertyName("orderItemId")]
    public int OrderItemId { get; set; }

    [JsonPropertyName("fullyAllocated")]
    public bool FullyAllocated { get; set; }
}

public class ItemIdentifier
{
    [JsonPropertyName("sku")]
    public string Sku { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }
}

// TODO: Replace these with your actual response/request models
public class FirstVendorResponse
{
    // Legacy - use OrdersResponse instead
}

public class SecondVendorRequest
{
    // Add properties needed for second vendor's API request
}

public class SecondVendorResponse
{
    // Add properties based on second vendor's API response
}