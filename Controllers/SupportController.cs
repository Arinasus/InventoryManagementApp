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
        /*[IgnoreAntiforgeryToken]
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
        }*/
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> UploadTicket([FromBody] object json)
        {

            var client = _http.CreateClient();

            var accessToken = "sl.u.AGZEuj2qBGYb7CbpM1VrJoyJdEAhKSNbHfi6-qLkzmitzyNaT4rj5aJjUdRgUL9Uz6ShKXSrkNzbcyBZRgZM8CyNXcw7RRKmtbjxkLQcMfcWEjcQACOClA061WmDf4s4Ypa5o04kofp3tWggSPoSRSk8ZnZfFclgzUuKyujo-DQmgRqt1VYEh7l3UezBhUbNXBSFPEkz9WOnbNC1reZ_RbNm-4eCOe7p0tRTKSOj_P0zznGUnNprXgLIdphVqbpZAu8y6FzugGoQwIcjXq8b72cQSYt8JaYVgtxvjKssXffages7iSP5kjKfDjFNkEYIPs3P69XLWMBFCeWDjbYj3UQpd5ojUVX_phmxFBGVwWoDt6qTjgK4EHW8qMOhqoJ3WxLqe2GQuY54O2k_IBf9Q0iAjwbQ9nhMKFLyxMuepXX3cAXvOMKWNsIgrLQj6p0QLiIidjIsYz7dDCE_D692rzdM2wHQnd0n_3aE7WtX9Xg4m45V4qpfoPOrwKPJgCiUFjuKPCJ5MTykonoOe6MEMBjs3DVGSQLtddLcscBguXRHTyr-tmGHN_lCg2J-P5NsWKDDjFiesavzHBb9l8yEiIzx9XOV5T0yF7wObXy_Yx4cU0s-dQ9-nEuiT5QIFqv1d-_NyUunhoD2-zgPwfrQ-TtZd8IYRk83Co1pFuZWe_XCejYBRCzNyX0WiFvNMaPI4oFUJvDjQUDAiOkZ5KWL5fXE_buY2B3HvJaddQbSaQfcawGTLRVhO4Qg4fqZ_suZzY8cigO05vbmrBVR7SHdLpHIlZhSbqJuaP3YEmt8Yeqhs3ZZ3-f2_0vtvQ4syCbAKkTjs8kNBiLokk3zEmhT3ogvDVeos8OHFVkXkxCdJFDlJD-lnkrWD8r5_lP0oQDvqjpaA4MBEtxnicOWvYbMgTKL1s1ERzH0XdqCX2mTJLDL5bH3lWavKk36l2kNhGwp5k-NJN06CgEjwyCSabidbUINZV6PGsCye-YnmHuKL52gMA3IyBeceBiT1Wq1_L2ELJEj4QMJu7F4CmXzgN9VCtssAvLKF-kSn2V5VaXXtoJ5WGQsrozlU2IfCxusIqFQKiu6DuK9ZzJHTPTZSVY_VwsdFZJGS_RHlaeAWUIjj9rpwzaza-FCEfuV1X9iPMXUF9YEbvQ-z8ihyzx7eoa7DviN48u3pHHKxUbeNHgC3jR8K5LHHIRm9npIirT1C3q5Sc8EjOqzqyuGuH2PFXz1KpQ6-rjglysDn0JSGSv7VlfNM-RR4gM5wovayKF_wnpEfi5a_x0Mvm85Xpvd6OrpfTSsZ6H1sTz0K4HBwJFhmfy-bb1C8I-ecBVFJJJMq4STbmogJ4_H9CwYckDoEYMI1oNqT5eFaHiQAvEvYQfPL-e5mBiiNsHCbyGvMnKwdoi0UJLUVfLsze925w1uMjaK_NphkuJpmN_QMn3qSeCRsky2ieYO_sgwDh7l_bkL389C_QY";

            var fileName = $"/Приложения/InventoryManagmentApp/tickets/ticket_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            client.DefaultRequestHeaders.Add("Dropbox-API-Arg",
                $"{{\"path\": \"{fileName}\", \"mode\": \"add\", \"autorename\": true}}");

            var rawJson = json.ToString();
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(rawJson));
            content.Headers.ContentType = null;

            var response = await client.PostAsync(
                "https://content.dropboxapi.com/2/files/upload",
                content
            );

            var dropboxResponse = await response.Content.ReadAsStringAsync();

            return Json(new
            {
                ok = response.IsSuccessStatusCode,
                dropbox = dropboxResponse
            });
        }

    }
}
