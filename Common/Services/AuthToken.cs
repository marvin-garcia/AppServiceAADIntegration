using Common.Interfaces;
using Common.Models;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Common.Services
{
    public class AuthToken : IAuthToken
    {
        private IHttpClient _httpClient;
        private TelemetryClient _telemetryClient;

        public AuthToken(IHttpClient httpClient, TelemetryClient telemetryClient)
        {
            _httpClient = httpClient;
            _telemetryClient = telemetryClient;
        }

        public async Task<AccessTokenResult> GetOnBehalfOf(string tenantId, string clientId, string clientSecret, string accessToken, string[] scopes)
        {
            if (string.IsNullOrEmpty(tenantId))
                throw new ArgumentNullException("Tenant ID");

            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentNullException("Client Id");

            if (string.IsNullOrEmpty(clientSecret))
                throw new ArgumentNullException("Client secret");

            if (string.IsNullOrEmpty(scopes[0]))
                throw new ArgumentNullException("Scopes");

            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentNullException("Access token");

            string url = string.Empty;
            try
            {
                url = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

                var parameters = new Dictionary<string, string>();
                parameters.Add("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer");
                parameters.Add("client_id", clientId);
                parameters.Add("client_secret", clientSecret);
                parameters.Add("assertion", accessToken);
                parameters.Add("scope", string.Join(' ', scopes));
                parameters.Add("requested_token_use", "on_behalf_of");

                var response = await _httpClient.SendFormUrlEncodedAsync(url, parameters);

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Token on behalf of the user failed with status {response.StatusCode}. Message: {response.ReasonPhrase}");

                var responseContent = await response.Content.ReadAsStringAsync();
                var accessTokenResult = JsonConvert.DeserializeObject<AccessTokenResult>(responseContent);

                return accessTokenResult;
            }
            catch (Exception e)
            {
                _telemetryClient.TrackEvent(
                    "AuthToken.GetOnBehalfOf",
                    new Dictionary<string, string>()
                    {
                        { "clientId", clientId },
                        { "clientSecret", clientSecret },
                        { "accessToken", accessToken },
                        { "scopes", string.Join(' ', scopes) },
                        { "url", url },
                    });

                throw e;
            }
        }
    }
}
