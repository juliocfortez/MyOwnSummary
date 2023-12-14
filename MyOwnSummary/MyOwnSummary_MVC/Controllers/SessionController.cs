using Microsoft.AspNetCore.Mvc;
using MyOwnSummary_MVC.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace MyOwnSummary_MVC.Controllers
{
    public class SessionController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:44321/api");
        private readonly HttpClient _client;
        private readonly ISessionManager _sessionManager;

        public SessionController(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;  
        }
        public async Task<IActionResult> Login()
        {
            LoginViewModel loginViewModel = new LoginViewModel { UserName = "User1", Password = "User1" };  
            ApiResponse response = new ApiResponse();
            HttpResponseMessage responseMessage = await _client.PostAsJsonAsync(_client.BaseAddress+ "/Authentication", loginViewModel);
            if (responseMessage.IsSuccessStatusCode) 
            {
                string data = responseMessage.Content.ReadAsStringAsync().Result;
                response = JsonConvert.DeserializeObject<ApiResponse>(data);
                _sessionManager.SetToken(response.Result.ToString());
            }
            return View(response);
        }
        [HttpGet]
        public async Task<IActionResult> Notes()
        {
            
                ApiResponse response = new ApiResponse();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _sessionManager.GetToken());
                HttpResponseMessage responseMessage = await _client.GetAsync(_client.BaseAddress + "/Note");
                if (responseMessage.IsSuccessStatusCode)
                {
                    string data = responseMessage.Content.ReadAsStringAsync().Result;
                    response = JsonConvert.DeserializeObject<ApiResponse>(data);
                return View(response);
                }
                response.Errors.Add("Inicie Sesion");
                response.StatusCode = HttpStatusCode.BadRequest;
                return View(response);
        }
    }
}
