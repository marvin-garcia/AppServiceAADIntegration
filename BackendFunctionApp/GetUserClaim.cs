using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BackendFunctionApp.Services;
using Microsoft.Extensions.Configuration;

namespace BackendFunctionApp
{
    public static class GetUserClaim
    {
        private static string _tenantId;
        private static string _clientId;
        private static string _aadInstance;
        private static string[] _requiredScopes = new string[] { "access_as_user" };

        [FunctionName("GetUserClaim")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "userclaim")] HttpRequest req,
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
