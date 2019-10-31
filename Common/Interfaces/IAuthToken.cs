using Common.Models;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IAuthToken
    {
        Task<AccessTokenResult> GetOnBehalfOf(string tenantId, string clientId, string clientSecret, string accessToken, string[] scopes);
    }
}
