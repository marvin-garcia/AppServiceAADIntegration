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
using System.Security.Claims;
using System.Linq;

namespace BackendFunctionApp
{
    public static class GetServiceClaim
    {
        private static string _tenantId;
        private static string _clientId;
        private static string _aadInstance;
        private static string[] _requiredScopes = new string[] { "access_as_application" };

        [FunctionName("GetServiceClaim")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "serviceclaim")] HttpRequest req,
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

            ClaimsPrincipal identities = req.HttpContext.User;

            return string.Join(' ', identities.Identities.Select(i => string.Join(", ", i.Claims.Select(c => c.Value))));
        }
    }
}
