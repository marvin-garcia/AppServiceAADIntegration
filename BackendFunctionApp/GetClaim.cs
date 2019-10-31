using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;
using System.Linq;

namespace BackendFunctionApp
{
    public static class GetClaim
    {
        private static string[] _requiredScopes = new string[] { "access_as_user" };

        [FunctionName("GetClaim")]
        public static string Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "claim")] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            req.HttpContext.VerifyUserHasAnyAcceptedScope(_requiredScopes);
            ClaimsPrincipal identities = req.HttpContext.User;

            return string.Join(' ', identities.Identities.Select(i => i.Claims.Select(c => c.Value)));
        }
    }
}
