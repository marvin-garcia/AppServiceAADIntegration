using FrontendApi.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace FrontendApi.Interfaces
{
    public interface IAuthToken
    {
        Task<AccessTokenResult> GetOnBehalfOf(string tenantId, string clientId, string clientSecret, string accessToken, string[] scopes);
    }
}
