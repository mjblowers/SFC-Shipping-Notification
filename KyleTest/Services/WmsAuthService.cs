using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using KyleTest.Models;

namespace KyleTest.Services;

public class WmsAuthService : IWmsAuthService
{
    private readonly HttpClient _authClient;

    public WmsAuthService(HttpClient authClient)
    {
        _authClient = authClient;
    }

    public async Task<AuthenticationDTO> GetAuthorizationTokenAsync()
    {
        var payload = new PayloadDTO
        {
            user_login = "1",
            grant_type = "client_credentials"
        };

        string credentials = "a2c80ff1-7ce3-422e-987a-d899f3e03641:v1qLTJ2+yRuJa0UfCDK7FPUavf25cAMp";
        byte[] encodedBytes = System.Text.Encoding.UTF8.GetBytes(credentials);
        string encodedCredentials = System.Convert.ToBase64String(encodedBytes);

        _authClient.DefaultRequestHeaders.Clear();
        _authClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _authClient.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
        _authClient.DefaultRequestHeaders.Add("Host", "secure-wms.com");
        _authClient.DefaultRequestHeaders.Add("Accept-Language", "Content-Length");
        _authClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate,sdch");
        _authClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);

        var response = await _authClient.PostAsJsonAsync("AuthServer/api/Token", payload);

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return JsonSerializer.Deserialize<AuthenticationDTO>(result, options);
        }
        return null;
    }
}