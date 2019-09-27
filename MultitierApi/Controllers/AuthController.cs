using FrontendApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FrontendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        //private IAuthToken _authToken;

        //public AuthController(IAuthToken authToken)
        //{
        //    _authToken = authToken;
        //}

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

        //[HttpGet("clientid/{clientId}/tenant/{tenantId}/username/{username}/password/{password}/scope/{scope}/tokenType/{tokenType}")]
        //public async Task<string> GetUserToken(string clientId, string tenantId, string username, string password, string scope, TokenType tokenType)
        //{
        //    string token = await _authToken.Get(clientId, null, tenantId, username, password, scope, tokenType);
        //    return token;
        //}
    }
}