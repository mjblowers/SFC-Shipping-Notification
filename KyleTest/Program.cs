using System.Security.Cryptography.X509Certificates;
using KyleTest.Models;
using KyleTest.Services;

class Program
{
    static async Task Main(string[] args)
    {
        // Setup HttpClients
        var authClient = new HttpClient { BaseAddress = new Uri("https://secure-wms.com/") };
        var orderClient = new HttpClient { BaseAddress = new Uri("https://secure-wms.com/") };
        var receiverClient = new HttpClient { BaseAddress = new Uri("https://secure-wms.com/") };
        var inextoClient = CreateInextoHttpClient();

        // Setup services
        var authService = new WmsAuthService(authClient);
        var orderService = new WmsOrderService(orderClient);
        var receiverService = new WmsReceiverService(receiverClient);
        var inextoService = new InextoApiService(inextoClient);

        // Authenticate
        var authToken = await authService.GetAuthorizationTokenAsync();
        if (authToken == null || string.IsNullOrEmpty(authToken.AccessToken))
        {
            Console.WriteLine("Failed to get authentication token");
            return;
        }
        Console.WriteLine("Authentication successful!");
        var amReceiving = true;
        if (amReceiving)
        {
            var receiversResponse = await receiverService.GetReceiversAsync(authToken.AccessToken);
            Console.WriteLine($"Retrieved {receiversResponse?.Receivers?.Count ?? 0} receivers");

            // Display some receiver details for verification
            if (receiversResponse?.Receivers?.Any() == true)
            {
                foreach (var receiver in receiversResponse.Receivers.Take(3))
                {
                    Console.WriteLine($"\nReceiver Details:");
                    Console.WriteLine($"  Receiver ID: {receiver.ReadOnly?.ReceiverId}");
                    Console.WriteLine($"  Reference: {receiver.ReferenceNum}");
                    Console.WriteLine($"  Description: {receiver.Description}");
                    Console.WriteLine($"  Status: {receiver.ReadOnly?.Status}");
                    Console.WriteLine($"  Is Closed: {receiver.ReadOnly?.IsClosed}");
                    Console.WriteLine($"  Ship From: {receiver.ShipFrom?.CompanyName} - {receiver.ShipFrom?.City}, {receiver.ShipFrom?.State}");

                    var receiverItems = receiver.Embedded?.ReceiverItems;
                    if (receiverItems?.Any() == true)
                    {
                        Console.WriteLine($"  Receiver Items ({receiverItems.Count}):");
                        foreach (var item in receiverItems.Take(2))
                        {
                            Console.WriteLine($"    - SKU: {item.ItemIdentifier?.Sku}, Qty: {item.Qty}");
                        }
                    }
                }
            }
        }
        // Get orders
        if (!amReceiving)
        {
            var ordersResponse = await orderService.GetOrdersAsync(authToken.AccessToken);
            Console.WriteLine($"Retrieved {ordersResponse?.Orders?.Count ?? 0} orders");

            // Display some order details for verification
            if (ordersResponse?.Orders?.Any() == true)
            {
                foreach (var order in ordersResponse.Orders.Take(3))
                {
                    Console.WriteLine($"\nOrder Details:");
                    Console.WriteLine($"  Order ID: {order.ReadOnly?.OrderId}");
                    Console.WriteLine($"  Reference: {order.ReferenceNum}");
                    Console.WriteLine($"  Description: {order.Description}");
                    Console.WriteLine($"  Status: {order.ReadOnly?.Status}");
                    Console.WriteLine($"  Is Closed: {order.ReadOnly?.IsClosed}");
                    Console.WriteLine($"  Ship To: {order.ShipTo?.CompanyName} - {order.ShipTo?.City}, {order.ShipTo?.State}");

                    var orderItems = order.Embedded?.OrderItems;
                    if (orderItems?.Any() == true)
                    {
                        Console.WriteLine($"  Order Items ({orderItems.Count}):");
                        foreach (var item in orderItems.Take(2))
                        {
                            Console.WriteLine($"    - SKU: {item.ItemIdentifier?.Sku}, Qty: {item.Qty}, Price: {item.FulfillInvSalePrice:C}");
                        }
                    }

                    var packages = order.ReadOnly?.Packages;
                    if (packages?.Any() == true)
                    {
                        Console.WriteLine($"  Packages ({packages.Count}):");
                        foreach (var package in packages.Take(2))
                        {
                            Console.WriteLine($"    - Package ID: {package.PackageId}, Tracking: {package.TrackingNumber}, Weight: {package.Weight}");
                        }
                    }
                    var mountainTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");

                    var dateTimeNow = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, mountainTimeZone);

                    var dateTimeNowString = dateTimeNow.ToString("yyyyMMdd-HHmmsszzz");
                    var eventUid = "WMS" + Guid.NewGuid().ToString();
                    // Hardcoded dummy InextoShipmentRequest for API testing
                    var testShipmentRequest = new InextoShipmentRequest
                    {
                        TransmissionUid = $"WMSPNSUS" + eventUid,
                        Key = $"urn:inexto:id:evt:tobacco:std:SHIPMENT.ADD.{dateTimeNowString}.WMSPNSUS.USID1.SMDFO.1BL0238617",
                        EventDateTime = dateTimeNow,
                        Documents = new[] { $"urn:inexto:tobacco:doc:dn:uid:SMDFO.1BL0238617" },
                        ScanningLocation = new InextoBusinessEntity
                        {
                            Keys = new[] { "urn:inexto:tobacco:be:sc:WMSPNSUS.USID1" },
                            Code = "USID1",
                            Name = "Specialty Fulfillment Center",
                            Country = "US",
                            Address1 = "Specialty Fulfillment Center Swedish Match 3 - 17th Avenue South",
                            Zip = "83651",
                            City = "Nampa"
                            //        Properties = new[]
                            //        {
                            //    new InextoProperty { Key = "urn:inexto:tobacco:be:eueoid", Value = "EO0000001" },
                            //    new InextoProperty { Key = "urn:inexto:tobacco:be:eufid", Value = "FI0000001" }
                            //}
                        },
                        ScanningPoint = new InextoScanningPoint
                        {
                            Code = "USID1",
                            Description = "Nampa Specialty Fulfillment Center"
                        },
                        BusinessEntities = new[]
                        {
                new InextoBusinessEntityWithRelation
                {
                    Relation = "destination",
                    Keys = new[] { "urn:inexto:tobacco:be:sc:SMDNFO.002290001" },
                    Code = "002290001",
                    Name = "SWEDISH MATCH NORTH AMERICA LLC",
                    Country = "US",
                    Address1 = "1021 EAST CARY STREET, SUITE 1600",
                    City = "RICHMOND",
                    Zip = "23219"
                },
                new InextoBusinessEntityWithRelation
                    {
                        Relation = "stockowner",
                        Keys = new[] { "urn:inexto:tobacco:be:sc:SMDFO.NA01" }, //should be smdfo, stock owner?
                        Code = "NA01",
                        Name = "Swedish Match North America LLC" ?? order.ShipTo.Name ?? "Unknown",
                        Country = order.ShipTo.Country ?? "US",
                        Address1 = "1021 E CARY ST, SUITE 1600",
                        City = "RICHMOND",
                        Zip = "23219"
                    }
            },
                        Items = new[]
                        {
                new InextoItem { MachineReadableCode = "01006092499075232100250813U201164101240NP000240.0010U2-0024001" },
                new InextoItem { MachineReadableCode = "01006092499075232100250813U201164103240NP000240.0010U2-0024001" },
                new InextoItem { MachineReadableCode = "01006092499075232100250813U201164104240NP000240.0010U2-0024001" }
            },
                        Properties = new[]
                        {
                new InextoProperty
                {
                    Key = "urn:inexto:core:mda:destinationType",
                    Value = "1"
                },
                new InextoProperty
                {
                    Key = "urn:inexto:core:mda:eventDateTime",
                    Value = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")
                }
            }
                    };

                    var shipmentRequest = MapOrderToInextoShipmentRequest(order);

                    if (shipmentRequest.Items == null || shipmentRequest.Items.Count() <= 0) continue;

                    //var result = await inextoService.SendShipmentEventAsync(shipmentRequest, authToken.AccessToken);
                    //Console.WriteLine($"Order {order.ReadOnly?.OrderId} INEXTO API result status: {result?.Status}");

                    // Uncomment below to test with hardcoded dummy shipment
                    var testResult = await inextoService.SendShipmentEventAsync(testShipmentRequest, authToken.AccessToken);
                    //Console.WriteLine($"Test API result status: {testResult?.Status}");
                }
            }
        }
    }

    //// Hardcoded dummy InextoShipmentRequest for API testing
    //var testShipmentRequest = new InextoShipmentRequest
    //{
    //    TransmissionUid = "WMSPNSUS1234567891",
    //    Key = "urn:inexto:id:evt:tobacco:std:Shipment.ADD.20250813-143000+0000.WMSPNSUS.USID1.SMDFO.",
    //    EventDateTime = DateTime.Parse("2025-08-13T14:30:00+00:00"),
    //    Documents = new[] { "urn:inexto:tobacco:doc:dn:uid:SMDFO.1234567891" },
    //    ScanningLocation = new InextoBusinessEntity
    //    {
    //        Keys = new[] { "urn:inexto:tobacco:be:sc:WMSPNSUS.USID1" },
    //        Code = "USID1",
    //        Name = "Nampa Specialty Fulfillment Center",
    //        Country = "US",
    //        Properties = new[]
    //        {
    //            new InextoProperty { Key = "urn:inexto:tobacco:be:eueoid", Value = "EO0000001" },
    //            new InextoProperty { Key = "urn:inexto:tobacco:be:eufid", Value = "FI0000001" }
    //        }
    //    },
    //    ScanningPoint = new InextoScanningPoint
    //    {
    //        Code = "USID1",
    //        Description = "Nampa Specialty Fulfillment Center"
    //    },
    //    BusinessEntities = new[]
    //    {
    //        new InextoBusinessEntityWithRelation
    //        {
    //            Relation = "destination",
    //            Keys = new[] { "urn:inexto:tobacco:be:sc:WMSPNSUS.2309" },
    //            Code = "2309",
    //            Name = "SWEDISH MATCH NORTH AMERICA LLC",
    //            Country = "US",
    //            Address1 = "1021 EAST CARY STREET, SUITE 1600",
    //            City = "RICHMOND",
    //            Zip = "23219",
    //            Properties = new[]
    //            {
    //                new InextoProperty { Key = "urn:inexto:tobacco:be:eueoid", Value = "EO0000002" },
    //                new InextoProperty { Key = "urn:inexto:tobacco:be:eufid", Value = "FI0000002" }
    //            }
    //        }
    //    },
    //    Items = new[]
    //    {
    //        new InextoItem { MachineReadableCode = "01006092499075232100250813U201164101240NP000240.0010U2-0024001" },
    //        new InextoItem { MachineReadableCode = "01006092499075232100250813U201164103240NP000240.0010U2-0024001" },
    //        new InextoItem { MachineReadableCode = "01006092499075232100250813U201164104240NP000240.0010U2-0024001" }
    //    },
    //    Properties = new[]
    //    {
    //        new InextoProperty
    //        {
    //            Key = "urn:inexto:core:mda:destinationType",
    //            Value = "2"
    //        },
    //        new InextoProperty
    //        {
    //            Key = "urn:inexto:core:mda:eventDateTime",
    //            Value = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")
    //        }
    //    }
    //};

    //// Map each order to an InextoShipmentRequest and send to INEXTO
    //if (ordersResponse?.Orders?.Any() == true)
    //{
    //    foreach (var order in ordersResponse.Orders)
    //    {
    //        var shipmentRequest = MapOrderToInextoShipmentRequest(order);

    //        if (shipmentRequest.Items == null || shipmentRequest.Items.Count() <= 0) continue;

    //        var result = await inextoService.SendShipmentEventAsync(shipmentRequest, authToken.AccessToken);
    //        Console.WriteLine($"Order {order.ReadOnly?.OrderId} INEXTO API result status: {result?.Status}");
    //    }
    //}

    //// Uncomment below to test with hardcoded dummy shipment
    // var testResult = await inextoService.SendShipmentEventAsync(testShipmentRequest, authToken.AccessToken);
    // //Console.WriteLine($"Test API result status: {testResult?.Status}");

    private static InextoShipmentRequest MapOrderToInextoShipmentRequest(Order order)
    {
        // Map Order to InextoShipmentRequest
        var dateTimeNow = DateTime.UtcNow;
        var dateTimeNowString = dateTimeNow.ToString("yyyyMMdd-HHmmsszzz");
        var eventUid = "WMS" + Guid.NewGuid().ToString();
        var shipmentRequest = new InextoShipmentRequest
        {
            TransmissionUid = eventUid,
            Key = $"urn:inexto:id:evt:tobacco:std:Shipment.ADD.{dateTimeNowString}.WMSPNSUS.USID1.SMDFO.",
            EventDateTime = dateTimeNow,
            //Comment = $"Shipment for order {order.ReadOnly?.OrderId}",
            //Documents = new[]
            //{
            //    $"urn:inexto:tobacco:doc:dn:uid:SMDFO.{order.ReadOnly?.OrderId}"
            //},
            Documents = new[] { "urn:inexto:tobacco:doc:dn:uid:SMDFO.1234567891" },
            ScanningLocation = new InextoBusinessEntity
            {
                Keys = new[] { "urn:inexto:tobacco:be:sc:WMSPNSUS.USID1" },
                Code = "USID1",
                Address1 = "Specialty Fulfillment Center Swedish Match 3 - 17th Avenue South",
                Zip = "83651",
                City = "Nampa",
                Name = "Specialty Fulfillment Center",
                Country = "US"
            },
            ScanningPoint = new InextoScanningPoint
            {
                Code = "SP001",
                Description = "Primary scanning point"
            },
            BusinessEntities = order.ShipTo != null
                ? new[]
                {
                    new InextoBusinessEntityWithRelation
                    {
                        Relation = "stockowner",
                        Keys = new[] { "rn:inexto:tobacco:be:sc:SMDFO.NA01" }, //should be smdfo, stock owner?
                        Code = "NA01",
                        Name = "Swedish Match North America LLC" ?? order.ShipTo.Name ?? "Unknown",
                        Country = order.ShipTo.Country ?? "US",
                        Address1 = "1021 E CARY ST, SUITE 1600",
                        City = "RICHMOND",
                        Zip = "23219"
                    }
                }
                : null,
            //temp hard code
            Items = new[]
                    {
                new InextoItem { MachineReadableCode = "01006092499075232100250813U201164101240NP000240.0010U2-0024001" },
                new InextoItem { MachineReadableCode = "01006092499075232100250813U201164103240NP000240.0010U2-0024001" },
                new InextoItem { MachineReadableCode = "01006092499075232100250813U201164104240NP000240.0010U2-0024001" }
            },
            //Items = order.Embedded?.OrderItems?.Select(item => new InextoItem
            //{
            //    MachineReadableCode = $"01{item.ItemIdentifier?.Sku ?? "unknown"}21{DateTime.UtcNow:yyMMdd}"
            //}).ToArray(),
            Properties = new[]
            {
                new InextoProperty
                {
                    Key = "urn:inexto:core:mda:destinationType", //not needed unless we have DynFo code?
                    Value = "2"
                },
                new InextoProperty
                {
                    Key = "urn:inexto:core:mda:eventDateTime",
                    Value = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")
                },
                new InextoProperty
                {
                    //Key = "urn:inexto:core:mda:eventDateTime",
                    //Value = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") //use transaction uid w/ timezone -7
                    Key = $"urn:inexto:tobacco:backward:{eventUid}",
                    Value = $"{dateTimeNow}.USID1.SMDFO.1BL0238617"
                }
            }
        };

        return shipmentRequest;
    }

    private static HttpClient CreateInextoHttpClient()
    {
        var certPath = "INEXTRACK-PMI-WMSPNSUS-20290503.pfx";
        var certPassword = "!ZPeOE!65XfKd9Pi";
        var cert = new X509Certificate2(certPath, certPassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(cert);
        return new HttpClient(handler) { BaseAddress = new Uri("https://pmiqas-api.inextrack.biz/") };
    }
}