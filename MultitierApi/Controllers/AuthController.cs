using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FrontendApi.Interfaces;

namespace FrontendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IHttpClient _httpClient { get; set; }
        private string _backendUrl { get; set; }

        public AuthController(IConfiguration configuration, IHttpClient httpClient)
        {
            this._httpClient = httpClient;
            this._backendUrl = $"{configuration["backendurl"]}/api/auth";
        }

        [HttpGet("IdToken")]
        public async Task<string> GetIdToken(bool backend = false)
        {
            if (backend)
                return await _httpClient.GetStringAsync($"{_backendUrl}/IdToken");
            else
                return Request.Headers["X-MS-TOKEN-AAD-ID-TOKEN"];
        }

        [HttpGet("AccessToken")]
        public async Task<string> GetAccessToken(bool backend = false)
        {
            if (backend)
                return await _httpClient.GetStringAsync($"{_backendUrl}/AccessToken");
            else
                return Request.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"];
        }

        [HttpGet("RefreshToken")]
        public async Task<string> GetRefreshToken(bool backend = false)
        {
            if (backend)
                return await _httpClient.GetStringAsync($"{_backendUrl}/RefreshToken");
            else
                return Request.Headers["X-MS-TOKEN-AAD-REFRESH-TOKEN"];
        }

        [HttpGet("TokenExpiration")]
        public async Task<string> GetTokenExpiration(bool backend = false)
        {
            if (backend)
                return await _httpClient.GetStringAsync($"{_backendUrl}/TokenExpiration");
            else
                return Request.Headers["X-MS-TOKEN-AAD-EXPIRES-ON"];
        }
    }
}
