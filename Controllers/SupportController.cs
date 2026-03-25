using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net.Http.Headers;
namespace InventoryManagementApp.Controllers
{
    public class SupportController : Controller
    {
        private readonly IHttpClientFactory _http;

        public SupportController(IHttpClientFactory http)
        {
            _http = http;
        }
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> UploadTicket([FromBody] object json)
        {
            var client = _http.CreateClient();
            var flowUrl = "https://defaultaf150ef089b940d69d3fe34a2212b7.d3.environment.api.powerplatform.com:443/powerautomate/automations/direct/workflows/15097ad5d4d948198c086b7e577c129c/triggers/manual/paths/invoke?api-version=1";

            var response = await client.PostAsync(
                flowUrl,
                new StringContent(json.ToString(), Encoding.UTF8, "application/json")
            );

            return Json(new { ok = response.IsSuccessStatusCode });
        }
    }
}
