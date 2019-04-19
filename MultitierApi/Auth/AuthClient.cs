using FrontendApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FrontendApi.Auth
{
    public class AuthClient
    {
        private IHttpClient _httpClient { get; set; }

        private string _meUrl = "/.auth/me";
        private string _refreshUrl = "/.auth/refresh";

        public AuthClient(IHttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<string> GetMe()
        {
            try
            {
                return await _httpClient.GetStringAsync(_meUrl);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<string> RefreshToken()
        {
            try
            {
                return await _httpClient.GetStringAsync(_refreshUrl);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
