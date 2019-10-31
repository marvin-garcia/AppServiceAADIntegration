using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Common.Interfaces;

namespace FrontendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FrontendFunctionController : Controller
    {
        private IAuthToken _authToken;
        private IHttpClient _httpClient;
        private IConfiguration _configuration;
        private string _backendUrl;

        public FrontendFunctionController(IConfiguration configuration, IHttpClient httpClient, IAuthToken authToken)
        {
            _authToken = authToken;
            _httpClient = httpClient;
            _configuration = configuration;
            _backendUrl = $"{configuration["backendurl"]}/api/BackendFunction";
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            //_httpClient.SetAuthenticationHeader("Bearer", Request.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"]);

            var accessTokenResult = _authToken.GetOnBehalfOf(
                _configuration["AzureAd:TenantId"],
                _configuration["AzureAd:FrontendClientId"],
                _configuration["AzureAd:FrontendClientSecret"],
                Request.Headers["X-MS-TOKEN-AAD-ID-TOKEN"],
                new string[] { _configuration["AzureAd:BackendScope"] }
            ).ContinueWith((r) =>
            {
                return r.Result;
            }).Result;

            _httpClient.SetAuthenticationHeader("Bearer", accessTokenResult.AccessToken);
        }

        [HttpGet("claim", Name = "GetClaim")]
        public async Task<OkObjectResult> GetClaim()
        {
            var response = await _httpClient.GetAsync($"{_backendUrl}/claim");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Function.GetClaim failed with status code {response.StatusCode}. Message: {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(responseContent);
        }

        [HttpGet("upn", Name = "GetUPN")]
        public async Task<OkObjectResult> GetUPN()
        {
            var response = await _httpClient.GetAsync($"{_backendUrl}/upn");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Function.GetUPN failed with status code {response.StatusCode}. Message: {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(responseContent);
        }

        [HttpGet("echo/{message}", Name = "Echo")]
        public async Task<OkObjectResult> Echo(string message)
        {
            var response = await _httpClient.GetAsync($"{_backendUrl}/echo/{message}");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Function.Echo failed with status code {response.StatusCode}. Message: {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(responseContent);
        }
    }
}
