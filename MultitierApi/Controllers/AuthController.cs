using Microsoft.AspNetCore.Mvc;

namespace FrontendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
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
    }
}