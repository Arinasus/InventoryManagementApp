using Microsoft.AspNetCore.Mvc;
using InventoryManagementApp.DTOs;
using System.Net.Http.Json;

namespace InventoryManagementApp.Controllers
{
    public class RegisterController : Controller
    {
        private readonly HttpClient _http;

        public RegisterController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("api");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new RegisterDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Index(RegisterDTO dto)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", dto);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Message = "Пользователь успешно зарегистрирован!";
                return View(new RegisterDTO());
            }

            var errorText = await response.Content.ReadAsStringAsync();
            ViewBag.Error = errorText;
            return View(dto);
        }
    }
}
