using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using BackendFunctionApp.Services;

namespace BackendFunctionApp
{
    public static class Echo
    {
        private static string _tenantId;
        private static string _clientId;
        private static string _aadInstance;
        private static string[] _requiredScopes = new string[] { "app.echo" };

        [FunctionName("Echo")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "echo/{message}")] HttpRequest req,
            string message,
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
            _aadInstance = config["AzureAd:AADInstance"];
            string audience = $"api://{_clientId}";

            await req.ValidateTokenAndScope(_aadInstance, _tenantId, _clientId, audience, _requiredScopes, new System.Threading.CancellationToken());

            return $"Backend function echo response: '{message}'";
        }
    }
}
