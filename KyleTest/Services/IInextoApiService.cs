using System.Threading.Tasks;
using KyleTest.Models;

namespace KyleTest.Services;

public interface IInextoApiService
{
    Task<InextoShipmentResponse> SendShipmentEventAsync(InextoShipmentRequest request, string accessToken);
}