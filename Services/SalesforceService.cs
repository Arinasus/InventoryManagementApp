using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace InventoryManagementApp.Services
{
    public class SalesforceService
    {
        private readonly string _accessToken;
        private readonly string _instanceUrl;

        public SalesforceService(IConfiguration config)
        {
            var auth = AuthenticateAsync(config).GetAwaiter().GetResult();
            _accessToken = auth.access_token;
            _instanceUrl = auth.instance_url;
        }

        private async Task<dynamic> AuthenticateAsync(IConfiguration config)
        {
            using var client = new HttpClient();

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"client_id", config["Salesforce:ClientId"]},
                {"client_secret", config["Salesforce:ClientSecret"]},
                {"username", config["Salesforce:Username"]},
                {"password", config["Salesforce:Password"]} 
            });

            var tokenUrl = "https://login.salesforce.com/services/oauth2/token";

            var response = await client.PostAsync(tokenUrl, content);
            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Salesforce Auth Response: {json}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка аутентификации Salesforce: {json}");
            }

            return JsonConvert.DeserializeObject<dynamic>(json);
        }

        public async Task<string> CreateAccountAsync(string name, string phone, string website, string industry, string description)
        {
            using var client = new HttpClient();
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

            Console.WriteLine($"Create Account Response: {json}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка создания Account: {json}");
            }

            dynamic result = JsonConvert.DeserializeObject(json);
            return result.id;
        }

        public async Task<string> CreateContactAsync(string email, string lastName, string accountId)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var body = new
            {
                LastName = lastName,
                Email = email,
                AccountId = accountId
            };

            var response = await client.PostAsJsonAsync($"{_instanceUrl}/services/data/v57.0/sobjects/Contact", body);
            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Create Contact Response: {json}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка создания Contact: {json}");
            }

            dynamic result = JsonConvert.DeserializeObject(json);
            return result.id;
        }
    }
}