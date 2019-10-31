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
    public static class Functions
    {
        [FunctionName("Functions")]
        public static string Echo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "echo/{message}")] HttpRequest req,
            string message,
            ILogger log)
        {
            return $"echo '{message}'";
        }
    }
}
