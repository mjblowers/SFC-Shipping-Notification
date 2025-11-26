using System.Threading.Tasks;
using KyleTest.Models;

namespace KyleTest.Services;

public interface IWmsReceiverService
{
    Task<ReceiverResponse> GetReceiversAsync(string accessToken);
}