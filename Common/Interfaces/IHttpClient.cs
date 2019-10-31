using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Common.Interfaces
{
    public interface IHttpClient
    {
        void SetAuthenticationHeader(string scheme, string accessToken);
        Task<HttpResponseMessage> GetAsync(string url);
        Task<string> GetStringAsync(string uri);
        Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string requestId = null);
        Task<HttpResponseMessage> DeleteAsync(string uri, string requestId = null);
        Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string requestId = null);
        Task<HttpResponseMessage> SendFormUrlEncodedAsync(string uri, Dictionary<string, string> parameters);
    }
}
