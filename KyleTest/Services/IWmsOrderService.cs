using System.Threading.Tasks;
using KyleTest.Models;

namespace KyleTest.Services;

public interface IWmsOrderService
{
    Task<OrdersResponse> GetOrdersAsync(string accessToken);
}