using InventoryManagementApp.DTOs;
using InventoryManagerApp.DTOs;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;
namespace InventoryManagerApp.Controllers
{
    public class AuthPageController : Controller
    {
        private readonly HttpClient _http;
        public AuthPageController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("api");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Auth/Login.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", dto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Inventory");
            }

            var error = await response.Content.ReadAsStringAsync();
            ViewBag.Error = error;
            return View(dto);
        }

    }
}
