using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BackendFunctionApp
{
    public static class Function1
    {
        [FunctionName("Function")]
        public static string Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return $"The message '{id}' was received by the Backend Azure function";
        }
    }
}
