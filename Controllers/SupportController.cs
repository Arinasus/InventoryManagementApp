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
            var flowUrl = "https://17ee28007c19e902965b69764815f7.54.environment.api.powerplatform.com:443/powerautomate/automations/direct/workflows/d304d765dc2a4f679b9af39e63db5508/triggers/manual/paths/invoke?api-version=1&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=-kHjs92RsRug4QwExd4JU87qj9Ry3NrpRE5_pkGsfms";

            var response = await client.PostAsync(
                flowUrl,
                new StringContent(json.ToString(), Encoding.UTF8, "application/json")
            );

            return Json(new { ok = response.IsSuccessStatusCode });
        }
    }
}
