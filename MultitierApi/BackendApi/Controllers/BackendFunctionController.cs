using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web.Resource;
using Common.Interfaces;

namespace BackendApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BackendFunctionController : Controller
    {
        private IAuthToken _authToken;
        private IConfiguration _configuration;
        private IHttpClient _httpClient;
        private string _functionUrl;
        private TelemetryClient _telemetryClient;

        public BackendFunctionController(IConfiguration configuration, IHttpClient httpClient, IAuthToken authToken, TelemetryClient telemetryClient)
        {
            _authToken = authToken;
            _httpClient = httpClient;
            _configuration = configuration;
            _functionUrl = configuration["functionurl"];
            _telemetryClient = telemetryClient;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            HttpContext.VerifyUserHasAnyAcceptedScope(_configuration["AzureAd:RequiredScopes"].Split(','));
            //_httpClient.SetAuthenticationHeader("Bearer", Request.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"]);

            string token = string.Empty;
            if (Request.Headers.ContainsKey("Authorization") && Request.Headers["Authorization"][0].StartsWith("Bearer "))
                token = Request.Headers["Authorization"][0].Substring("Bearer ".Length);
            else
                throw new ArgumentNullException("Bearer token");

            var accessTokenResult = _authToken.GetOnBehalfOf(
                _configuration["AzureAd:TenantId"],
                _configuration["AzureAd:ClientId"],
                _configuration["AzureAd:ClientSecret"],
                token,
                new string[] { _configuration["AzureAd:BackendFunctionScope"] }
            ).ContinueWith((r) =>
            {
                return r.Result;
            }).Result;

            _httpClient.SetAuthenticationHeader("Bearer", accessTokenResult.AccessToken);
        }

        [HttpGet("userclaim", Name = "GetUserClaim")]
        public async Task<OkObjectResult> GetUserClaim()
        {
            var response = await _httpClient.GetAsync($"{_functionUrl}/api/userclaim");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Function.GetClaim failed with status code {response.StatusCode}. Message: {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(responseContent);
        }

        [HttpGet("serviceclaim", Name = "GetServiceClaim")]
        public async Task<OkObjectResult> GetServiceClaim()
        {
            var response = await _httpClient.GetAsync($"{_functionUrl}/api/serviceclaim");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Function.GetUPN failed with status code {response.StatusCode}. Message: {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(responseContent);
        }

        [HttpGet("echo/{message}", Name = "Echo")]
        public async Task<OkObjectResult> Echo(string message)
        {
            var response = await _httpClient.GetAsync($"{_functionUrl}/api/echo/{message}");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Function.Echo failed with status code {response.StatusCode}. Message: {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(responseContent);
        }
    }
}
