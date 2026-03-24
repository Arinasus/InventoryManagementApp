using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace InventoryManagementApp.Services
{
    public class SalesforceService
    {
        private readonly string _accessToken;
        private readonly string _instanceUrl;
        private readonly ILogger<SalesforceService> _logger;

        public SalesforceService(IConfiguration config, ILogger<SalesforceService> logger)
        {
            _logger = logger;
            _logger.LogInformation("=== SalesforceService constructor START ===");
            try
            {
                var auth = AuthenticateAsync(config).GetAwaiter().GetResult();
                _accessToken = auth.access_token;
                _instanceUrl = auth.instance_url;
                _logger.LogInformation($"Instance URL: {_instanceUrl}");
                _logger.LogInformation("=== SalesforceService constructor SUCCESS ===");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=== SalesforceService constructor FAILED ===");
                throw;
            }
        }

        private async Task<dynamic> AuthenticateAsync(IConfiguration config)
        {
            using var client = new HttpClient();
            var clientId = config["Salesforce:ClientId"];
            var clientSecret = config["Salesforce:ClientSecret"];
            var username = config["Salesforce:Username"];
            var password = config["Salesforce:Password"];

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"client_id", clientId},
                {"client_secret", clientSecret},
                {"username", username},
                {"password", password}
            });

            _logger.LogInformation("=== Salesforce Authentication ===");
            _logger.LogInformation($"ClientId: {clientId?.Substring(0, 10)}...");
            _logger.LogInformation($"Username: {username}");

            var response = await client.PostAsync("https://login.salesforce.com/services/oauth2/token", content);
            var json = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Auth Response Status: {response.StatusCode}");
            _logger.LogInformation($"Auth Response: {json}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка аутентификации: {json}");
            }

            return JsonConvert.DeserializeObject<dynamic>(json);
        }

        public async Task<string> CreateAccount(string name, string phone, string website, string industry, string description)
        {
            _logger.LogInformation("=== Creating Account in Salesforce ===");
            _logger.LogInformation($"Account Name: {name}");

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

            _logger.LogInformation($"Create Account Response Status: {response.StatusCode}");
            _logger.LogInformation($"Create Account Response: {json}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка создания Account: {json}");
            }

            dynamic result = JsonConvert.DeserializeObject(json);
            return result.id;
        }

        public async Task<string> CreateContact(string email, string lastName, string accountId)
        {
            _logger.LogInformation("=== Creating Contact in Salesforce ===");
            _logger.LogInformation($"Contact Name: {lastName}, Email: {email}, AccountId: {accountId}");

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

            _logger.LogInformation($"Create Contact Response Status: {response.StatusCode}");
             _logger.LogInformation($"Create Contact Response: {json}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка создания Contact: {json}");
            }

            dynamic result = JsonConvert.DeserializeObject(json);
            return result.id;
        }
    }
}