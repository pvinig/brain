﻿using System.Threading.Tasks;
using System.Net.Http;
using BRN.WebApp.MVC.Models;
using System.Text.Json;
using System.Text;

namespace BRN.WebApp.MVC.Services
{
    public class AuthenticationService : Service, IAuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserLoginResponse> Login(UserLogin userLogin)
        {
            var loginContent = new StringContent(
                JsonSerializer.Serialize(userLogin),  
                Encoding.UTF8,
                mediaType: "application/json"
                );

            var response = await _httpClient.PostAsync(requestUri:"https://localhost:44382/api/identidade/autenticar", loginContent);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (!HandleError(response))
            {
                return new UserLoginResponse
                {
                    ResponseResult = JsonSerializer.Deserialize<ResponseResult>(await response.Content.ReadAsStringAsync(), options)

                };
            }

            return JsonSerializer.Deserialize<UserLoginResponse>(await response.Content.ReadAsStringAsync(), options);
        }

        public async Task<UserLoginResponse> Register(UserRegister userRegister)
        {
            var registroContent = new StringContent(
                JsonSerializer.Serialize(userRegister),
                Encoding.UTF8,
                mediaType: "application/json"
                );

            var response = await _httpClient.PostAsync(requestUri: "https://localhost:44382/api/identidade/nova-conta", registroContent);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (!HandleError(response))
            {
                return new UserLoginResponse
                {
                    ResponseResult = JsonSerializer.Deserialize<ResponseResult>(await response.Content.ReadAsStringAsync(), options)

                };
            }

            return JsonSerializer.Deserialize<UserLoginResponse>(await response.Content.ReadAsStringAsync(), options);
        }


    }
}
