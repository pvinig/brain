using System.Threading.Tasks;
using System.Net.Http;
using BRN.WebApp.MVC.Models;
using System.Text.Json;
using System.Text;

namespace BRN.WebApp.MVC.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> Login(UserLogin userLogin)
        {
            var loginContent = new StringContent(
                JsonSerializer.Serialize(userLogin),  
                Encoding.UTF8,
                mediaType: "application/json"
                );

            var response = await _httpClient.PostAsync(requestUri:"https://localhost:44382/api/identidade/autenticar", loginContent);

            var test = await response.Content.ReadAsStringAsync();

            var deserialize = JsonSerializer.Deserialize<string>(await response.Content.ReadAsStringAsync());
            return deserialize;
            // throw new NotImplementedException();
        }

        public async Task<string> Register(UserRegister userRegister)
        {
            var registroContent = new StringContent(
                JsonSerializer.Serialize(userRegister),
                Encoding.UTF8,
                mediaType: "application/json"
                );

            var response = await _httpClient.PostAsync(requestUri: "https://localhost:44382/api/identidade/nova-conta", registroContent);

            return JsonSerializer.Deserialize<string>(await response.Content.ReadAsStringAsync());
        }


    }
}
