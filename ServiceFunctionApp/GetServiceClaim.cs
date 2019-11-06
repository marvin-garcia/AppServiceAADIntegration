using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ServiceFunctionApp
{
    public static class GetServiceClaim
    {
        private static string _tenantId;
        private static string _clientId;
        private static string _clientSecret;
        private static string _aadInstance;
        private static string[] _requiredScopes;
        private static string _functionUrl;
        private static HttpClient _httpClient = new HttpClient();

        [FunctionName("GetServiceClaim")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            _tenantId = config["AzureAd:TenantId"];
            _clientId = config["AzureAd:ClientId"];
            _clientSecret = config["AzureAd:ClientSecret"];
            _aadInstance = config["AzureAd:AADInstance"];
            _requiredScopes = new string[] { config["AzureAd:Scope"] };
            _functionUrl = config["functionurl"];

            //string authority = string.Format(CultureInfo.InvariantCulture, _aadInstance, _tenantId);

            var app = ConfidentialClientApplicationBuilder.Create(_clientId)
               .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
               .WithClientSecret(_clientSecret)
               .Build();

            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenForClient(_requiredScopes)
                    .ExecuteAsync();
                
            }
            catch (MsalServiceException ex)
            {
                // Case when ex.Message contains:
                // AADSTS70011 Invalid scope. The scope has to be of the form "https://resourceUrl/.default"
                // Mitigation: change the scope to be as expected
                throw ex;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            var response = await _httpClient.GetAsync($"{_functionUrl}/api/serviceclaim");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"GetServiceClaim service function failed with status code {response.StatusCode}. Reason phrase: {response.ReasonPhrase}");

            string responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
    }
}
