using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace InventoryManagementApp.Services
{
    public class SalesforceService
    {
        private readonly string _instanceUrl;
        private readonly string _accessToken;

        public SalesforceService(IConfiguration config)
        {
            var auth = Authenticate(config);
            _instanceUrl = auth.instance_url;
            _accessToken = auth.access_token;
        }

        private dynamic Authenticate(IConfiguration config)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
                {"client_id", config["Salesforce:ClientId"]},
                {"client_secret", config["Salesforce:ClientSecret"]}
            });

            var response = client.PostAsync("https://login.salesforce.com/services/oauth2/token", content).Result;
            var json = response.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject(json);
        }

        public async Task<string> CreateAccount(string name, string phone, string website, string industry, string description)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var body = new
            {
                Name = name,
                Phone = phone,
                Website = website,
                Industry = industry,
                Description = description
            };

            var response = await client.PostAsJsonAsync($"{_instanceUrl}/services/data/v57.0/sobjects/Account", body);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);

            return result.id;
        }

        public async Task CreateContact(string email, string name, string accountId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var body = new
            {
                LastName = name,
                Email = email,
                AccountId = accountId
            };

            await client.PostAsJsonAsync($"{_instanceUrl}/services/data/v57.0/sobjects/Contact", body);
        }
    }
}
