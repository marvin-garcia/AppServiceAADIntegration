using FrontendApi.Interfaces;
using FrontendApi.Models;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace FrontendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FrontendAuthController : Controller
    {
        private IAuthToken _authToken;
        private IConfiguration _configuration;
        private TelemetryClient _telemetryClient;

        public FrontendAuthController(IAuthToken authToken, IConfiguration configuration, TelemetryClient telemetryClient)
        {
            _authToken = authToken;
            _configuration = configuration;
            _telemetryClient = telemetryClient;
        }

        [HttpGet("IdToken")]
        public string GetIdToken()
        {
            return Request.Headers["X-MS-TOKEN-AAD-ID-TOKEN"];
        }

        [HttpGet("AccessToken")]
        public string GetAccessToken()
        {
            return Request.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"];
        }

        [HttpGet("RefreshToken")]
        public string GetRefreshToken()
        {
            return Request.Headers["X-MS-TOKEN-AAD-REFRESH-TOKEN"];
        }

        [HttpGet("TokenExpiration")]
        public string GetTokenExpiration()
        {
            return Request.Headers["X-MS-TOKEN-AAD-EXPIRES-ON"];
        }

        [HttpGet("BackendTokenOnBehalfOf")]
        public async Task<AccessTokenResult> GetUserToken()
        {
            var accessTokenResult = await _authToken.GetOnBehalfOf(
                _configuration["AzureAd:TenantId"],
                _configuration["AzureAd:FrontendClientId"],
                _configuration["AzureAd:FrontendClientSecret"],
                Request.Headers["X-MS-TOKEN-AAD-ID-TOKEN"],
                new string[] { _configuration["AzureAd:BackendScope"] });

            return accessTokenResult;
        }
    }
}