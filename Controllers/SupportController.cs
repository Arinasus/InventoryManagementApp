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
        /*[IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> UploadTicket([FromBody] object json)
        {

            var client = _http.CreateClient();

            var accessToken = "sl.u.AGbPJRg2jZUOcChu11yv3aWQGhMWBjeiBZxGIqc7qP8IXEx6xePI_awzBHrQFDnldDiDxHVj2-rW1iQ1Z_Ulk39tY5XzJJdOcxSorrRk4JaizQg2XFL4Mc77rLVhCuXg3628MCvCeOWigLFw9FBkvBpe5LUfmXQBga89zUMlgsaMjwUYFmOANrUeJCKJ_ZycL2LZsPKDB3ILgo1Sbbz1rpOt_HB60CVeABCjGO4LaBfedprV5-PLZ_1kW-KbAfaSaUlB8PUKScgXtIeGOOAqtBh9QMomXeePI9rSLfSkLly-ttvT1Lr8Y9K3uNW0rgSoGfB0ecm-QRqtOXtJBPPv4HtsuW3J4d-BgvOSOkVFMU5sEMZdOBirli3Fv3JqML8_EPouuuAwlhhJAmVXwvuajCopWTh_MdtW2QEEGrJ7u8mWEQtpeInUtxTzLwHif3vf28Mwni9ZoDoE1QUxxEJnIQWAlD9hHGEzq4h7XYjcUY4TMTKtRaksUNVfwjdAura39oQxxU-v-w9ss7NAXV2eo6k0wP_idBOPG02V2xnzAeA2Q8VI6f8vr7cXyIWJoZdF4UPxLkZjYJ8bZlX_hDNNaVdmesxHnLJIg5Zf5sUPKtJh5MLBmig1h2kH0msupMPqExgfbfsoBYPIpkSHE0L7vaMj4uhgScp-UjpRPgtA26WwaseF_dU_EQs29CC3V9Nd0KQCNHgwnhm-dwPowKhGONCgCQE_LCQhLA-pSUPhAof0WIEt6TrBj0lFrAMp5lk-RilPbwnFh0ZyGKN9PQpUSeNekxWOBS3YEM4DvAUNqQGi3kqIyhiUBx4ZIYRbjoC_5fvLFB4KdqlrYwGfDQ3-9aSh9BcgLvNnepWQQnNGNoHlOmzWOp65Mks0Dl-8qAiRQdGzXkWSYAzxJdry-f--E_PljDUEleTAH4TVOqKZM7E4LBAmiCoZ9MCz6f_ZS0I6hmHaLDSfodSKH8rpIiflFAtYaknzK_pfrWhdSnwZLkwGxYUV3LC30QroEwEqoM9DuUq01x4y-2b-CxdmnYFm-E46oTDb2iF5uL7Htueol1OMfRBJuZXqKNmp99bwdfJZWhraQCSv94OiNFhkd0_WodTPtTUdxJZOeKoDKZ3LAN1ijW6wT5I_ujrFtwrZILNRtEUcNd2UiuWFBZUTsGPWIvtGSld2ENTGod6YOd5-Qy8kgDb1eP54o9d2AAphZAo-e4FGKaDpnsHb5E9NB6p61233ag74ovswp869MXVdiJb8Jc3boU3o2T8CP0vAc5V19fKkwlLqL-kEHYojXF37tKpSMd8p8dzodfeIKxfRfP1pNcw9GreF8hfYqJRTu5dKJ6GygJFyBiuhdDUFEVMlCM0i-IYaI1knQPV7w8gmMpEr1_Jpd75T5e83ngEploj_H6YqU31v1THIZOnBgTKLABi8yu7RmvrJ1TgzswhWVfUQ6Ge_zrK4cR_HTAzwHS1tkh4";

            var fileName = "/Apps/InventoryManagmentApp/tickets/ticket_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".json";

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            client.DefaultRequestHeaders.Add(
                "Dropbox-API-Arg",
                "{\"path\": \"" + fileName + "\", \"mode\": \"add\", \"autorename\": true}"
            );


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
*/
    }
}
