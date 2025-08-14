using System.Threading.Tasks;
using KyleTest.Models;

namespace KyleTest.Services;

public interface IWmsAuthService
{
    Task<AuthenticationDTO> GetAuthorizationTokenAsync();
}