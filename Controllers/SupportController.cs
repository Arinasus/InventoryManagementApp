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

            var accessToken = "sl.u.AGbkyqnu_DmpzPAvxh43sEOx-8gfNHTNjVESUGnsapo3JvpxL4EdKNnKxFLgU9kcS-Yvwsk8_zFVc9S1lQD5iF3dIyxGdAnfqyo_-0NWGDI3kAMggfl-bsMGTA_2lnPB2vzTeLPRSI61fJyVBRTXT_yNm6IavCfj4Ss-8N0ujgUbvPBD5j4zam4r5LCe-2m426oACYAsJgdL9UayuyB5dNOaDEVPQsuKRK-a1MnV400H2QShGRcxh0koeqtkG2O9ZHYX76uCHwtpmI03BJMj86P7iZwX8a9TLMh7DV6bIFaX9FMQuZZu2wiTh4dQ1dQyywGtoY4wwpUFdb7qzC68fQ6ZyJH03e8feV9fEtzph5RH2ZcR9OwciD6bdvbfsvE4DA7xARHipncCY2DPMZjeVI47f7-fJKsB8W2uRKaB_DUhGCswew-9IWz8TDBAD_r69tH6-TvZoQFv5XrxKZPlipTbjm1okVh6n9NiXfRXKaEI6T42LmVGxrprkmGpTnfhQ33nr8pPGnPi_4XON4NQqfyADpbKBhSmkUBEGxYZCAEqPlZUZkBUDHmO7S0HJ0TtX7H0sAMfRQcS9ZCIjtXdQnbcPlkOjust-8sYq16kMcXczd4Gn2vbyX6IYUyDHDVM6EdOOciT9Qkev6CtZF7a3Kw_eDPDIDCbDwAlLFi9Eq4FnVFDiP1SottxkzcFmJepI4hXwgmQx8m7UmgztkFA04199jHXhvBi2VrRZCDYCkWkb2tSJRN38AsU5SDKUdxIyDoisLdkGGEzgkYSYFS4D9Py3qzTbE6-VGxdaRyycS7m-llQ5QTSLUPVWBBdZ_aGyYwdS3OchN4Q2CXIe0O0BVMD4QkoSCr9fQKnWbtpYtryJYBhiau58dBqQkD6kz7tUdxs3MLmMvGy0XTSSLL4X5NQ5GvS7WmUNPpDPT04WBgf8-vNlkTUQ6lWGzykMGd_mDApF7NY8sMsM-EiWKrp8hft4Bf0Gk9cH_uP8tUupHgaoYxbn72B5_Ok8H0iFUxDa7GCtyZQ428FBr9E0XEWqG4-tQgXivKinZEcZTB2NbgY8DfRtYZlXUDsctIW1GI59gkE73uH45sPxZCzPUhHqrRKt0DJ9Ws78uQ9Y_tLvOcxW9mtdX5ENGHfgJDgIXWPVJq53JMGT1smAkKiAqWmsTi6ARFYi7RSEyOWgjmx9i2NNaQ56xXqpNTuOd2LAktdGz3YobeIxUYLQt5xcBrfGPw4vfNS2roavPHbwqZSRCFWSYJ5AqqAOx3K3m3UXV0rWp7-XHa9yGZGQsL28ThelVW6FONHxptz7duzuRYlBzlNPyTcNSIWlq9rjQcKDfuZqKVvLkHh-qF-SH4HHZ12HYDCE2Erj9hKX0UOigFDBFs_CoHSmJK8-NBm8fHprUhpJ2VH52wODZNozdhpaJT5GaSPJFdeWysykVTsGuV_JpFCDek0l86pFhUb9znfNYBGPzI";

            var fileName = $"/tickets/ticket_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            client.DefaultRequestHeaders.Add("Dropbox-API-Arg",
                $"{{\"path\": \"{fileName}\", \"mode\": \"add\", \"autorename\": true}}");

            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/octet-stream");

            var response = await client.PostAsync(
                "https://content.dropboxapi.com/2/files/upload",
                content
            );

            return Json(new { ok = response.IsSuccessStatusCode });
        }

    }
}
