using FrontendApi.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FrontendApi.Services
{
    public class AuthToken : IAuthToken
    {
        private IHttpClient _httpClient;

        public AuthToken(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> Get(string clientId, string tenantId, string username, string password, string scope, TokenType tokenType)
        {
            try
            {
                string url = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

                string scopeOpt  = tokenType == TokenType.Id ? "openid" : "user.read,openid";
                var parameters = new Dictionary<string, string>();
                parameters.Add("tenant", tenantId);
                parameters.Add("client_id", clientId);
                parameters.Add("gran_type", "password");
                parameters.Add("username", username);
                parameters.Add("password", password);
                parameters.Add("scope", scopeOpt);

                var response = await _httpClient.SendFormUrlEncodedAsync(url, parameters);
                var responseContent = await response.Content.ReadAsStringAsync();
                dynamic content = JsonConvert.DeserializeObject(responseContent);

                if (tokenType == TokenType.Id)
                    return content.id_token;
                else
                    return content.access_token;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
