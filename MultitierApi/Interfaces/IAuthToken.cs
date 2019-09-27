using System.Threading.Tasks;

namespace FrontendApi.Interfaces
{
    public enum TokenType
    {
        Id,
        Access,
        Refresh,
    }

    public interface IAuthToken
    {
        Task<string> Get(string clientId, string tenantId, string username, string password, string scope, TokenType tokenType);
    }
}
