using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace InventoryManagementApp.Services
{
    public class SalesforceService
    {
        private readonly string _sessionId;
        private readonly string _instanceUrl;
        private readonly ILogger<SalesforceService> _logger;

        public SalesforceService(IConfiguration config, ILogger<SalesforceService> logger)
        {
            _logger = logger;
            _logger.LogInformation("=== SalesforceService constructor START ===");

            try
            {
                var login = LoginWithSoap(config).GetAwaiter().GetResult();
                _sessionId = login.sessionId;
                _instanceUrl = login.instanceUrl;

                _logger.LogInformation($"Instance URL: {_instanceUrl}");
                _logger.LogInformation("=== SalesforceService constructor SUCCESS ===");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=== SalesforceService constructor FAILED ===");
                throw;
            }
        }

        private async Task<(string sessionId, string instanceUrl)> LoginWithSoap(IConfiguration config)
        {
            var username = config["Salesforce:Username"];
            var password = config["Salesforce:Password"];
            var token = config["Salesforce:SecurityToken"];

            var soapBody = $@"
                <env:Envelope xmlns:xsd='http://www.w3.org/2001/XMLSchema'
                              xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'
                              xmlns:env='http://schemas.xmlsoap.org/soap/envelope/'>
                  <env:Body>
                    <n1:login xmlns:n1='urn:partner.soap.sforce.com'>
                      <n1:username>{username}</n1:username>
                      <n1:password>{password}{token}</n1:password>
                    </n1:login>
                  </env:Body>
                </env:Envelope>";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("SOAPAction", "login");
            var content = new StringContent(soapBody, Encoding.UTF8, "text/xml");

            _logger.LogInformation("=== Salesforce SOAP Login ===");

            var response = await client.PostAsync("https://login.salesforce.com/services/Soap/u/61.0", content);
            var xml = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"SOAP Response Status: {response.StatusCode}");
            _logger.LogInformation($"SOAP Response: {xml}");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Ошибка SOAP логина: " + xml);

            string sessionId = Extract(xml, "<sessionId>", "</sessionId>");
            string serverUrl = Extract(xml, "<serverUrl>", "</serverUrl>");

            string instanceUrl = serverUrl.Split("/services")[0];

            return (sessionId, instanceUrl);
        }

        private string Extract(string source, string start, string end)
        {
            int s = source.IndexOf(start) + start.Length;
            int e = source.IndexOf(end, s);
            return source.Substring(s, e - s);
        }

        public async Task<string> CreateAccount(string name, string phone, string website, string industry, string description)
        {
            _logger.LogInformation("=== Creating Account in Salesforce ===");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _sessionId);

            var body = new
            {
                Name = name,
                Phone = phone,
                Website = website,
                Industry = industry,
                Description = description
            };

            var response = await client.PostAsJsonAsync($"{_instanceUrl}/services/data/v61.0/sobjects/Account", body);
            var json = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Create Account Response: {json}");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Ошибка создания Account: " + json);

            dynamic result = JsonConvert.DeserializeObject(json);
            return result.id;
        }

        public async Task<string> CreateContact(string email, string lastName, string accountId)
        {
            _logger.LogInformation("=== Creating Contact in Salesforce ===");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _sessionId);

            var body = new
            {
                LastName = lastName,
                Email = email,
                AccountId = accountId
            };

            var response = await client.PostAsJsonAsync($"{_instanceUrl}/services/data/v61.0/sobjects/Contact", body);
            var json = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Create Contact Response: {json}");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Ошибка создания Contact: " + json);

            dynamic result = JsonConvert.DeserializeObject(json);
            return result.id;
        }
    }
}
